using BlApi;
using BlImplementation;
using BO;
using DalApi;
using DO;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

namespace Helpers;


internal static class VolunteerManager
{
    private static IDal s_dal = DalApi.Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 
    private static readonly Random s_rand = new();
    private static int s_simulatorCounter = 0;

    internal static int CalculateTotalCallsHandled(int id)
    {
        IEnumerable<DO.Assignment> assignmentsForVolunteer;
        // Fetch all assignments for the volunteer where the ending type is "Solved"
        lock (AdminManager.BlMutex)
            assignmentsForVolunteer = s_dal.Assignment.ReadAll(asm =>
            asm.VolunteerId == id &&
            asm.EType.HasValue &&
            (BO.EndingType)asm.EType == BO.EndingType.Solved);

        // Return the count of such assignments
        return assignmentsForVolunteer.Count();
    }

    internal static int CalculateTotalCallsCancelled(int id)
    {
        IEnumerable<DO.Assignment> assignmentsForVolunteer;
        // Fetch all assignments for the volunteer where the ending type is "CanceledByManager" or "SelfCanceld"
        lock (AdminManager.BlMutex)
            assignmentsForVolunteer = s_dal.Assignment.ReadAll(asm =>
            asm.VolunteerId == id &&
            asm.EType.HasValue &&
            ((BO.EndingType)asm.EType == BO.EndingType.CanceledByManager || (BO.EndingType)asm.EType == BO.EndingType.SelfCanceld));

        // Return the count of such assignments
        return assignmentsForVolunteer.Count();
    }

    internal static int CalculateTotalCallsExpired(int id)
    {
        IEnumerable<DO.Assignment> assignmentsForVolunteer;
        // Fetch all assignments for the volunteer where the ending type is "Expired"
        lock (AdminManager.BlMutex)
            assignmentsForVolunteer = s_dal.Assignment.ReadAll(asm =>
            asm.VolunteerId == id &&
            asm.EType.HasValue &&
            (BO.EndingType)asm.EType == BO.EndingType.Expired);

        // Return the count of such assignments
        return assignmentsForVolunteer.Count();
    }

    internal static CallStatus CalculateCallInProgressStatus(DO.Call currentDoCall)
    {
        if (AdminManager.Now + s_dal.Config.RiskRange >= currentDoCall.MaxTime)
            return CallStatus.InProgressInRisk;
        return CallStatus.InProgress;
    }

    internal static DO.Assignment? GetCurrentDoAssignment(int volunteerId)
    {
        DO.Assignment? currentAssignment;
        // Use the Assignment.Find function to find the current assignment
        lock (AdminManager.BlMutex)
            currentAssignment = s_dal.Assignment.Read(asm => asm.VolunteerId == volunteerId && !asm.EndTime.HasValue);
        return currentAssignment;
    }

    internal static DO.Volunteer? GetDoVolunteer(int volunteerId)
    {
        // Use the Assignment.Find function to find the current assignment
        lock (AdminManager.BlMutex)
            return s_dal.Volunteer.Read(v => v.Id == volunteerId);
    }

    internal static DO.Call? GetCurrentDoCall(int CallId)
    {
        // Use the Assignment.Find function to find the current assignment
        lock (AdminManager.BlMutex)
            return s_dal.Call.Read(c => c.Id == CallId);
    }

    /// <summary>
    /// Returns the call id of the call the volunteer is dealing with
    /// </summary>
    internal static int? GetCurrentCallId(int volunteerId)
    {

        var currentAssignment = GetCurrentDoAssignment(volunteerId);

        // Return the ID of the assignment if found, otherwise return null
        return currentAssignment == null ? null : currentAssignment.CallId;
    }

    /// <summary>
    /// returns the current call the volunteer is dealing with
    /// </summary>
    internal static BO.CallInProgress? GetCurrentCall(int volunteerId)
    {
        DO.Assignment? currentDoAssignment = GetCurrentDoAssignment(volunteerId);

        if (currentDoAssignment == null)
            return null;

        int currentCallId = currentDoAssignment.CallId;

        DO.Call? currentDoCall = GetCurrentDoCall(currentCallId);

        if (currentDoCall == null)
            return null;

        // Map the current call to the BO.CallInProgress entity
        return new BO.CallInProgress
        {
            AsmId = currentDoAssignment.Id,
            CallId = (int)currentCallId,
            CType = (BO.CallType)currentDoCall.CType,
            Description = currentDoCall.Description,
            FullAddress = currentDoCall.FullAddress,
            Opening = currentDoCall.Opening,
            MaxTime = currentDoCall.MaxTime,
            EnterTime = currentDoAssignment.EnterTime,
            Distance = Tools.getDistance(volunteerId, currentDoCall),
            Status = CalculateCallInProgressStatus(currentDoCall) // Placeholder for status calculation
        };
    }

    /// <summary>
    /// returns the current call type of certain volunteer
    /// </summary>
    internal static BO.CallType GetVolunteerCurrentCallType(int id)
    {
        DO.Assignment? currentAssignment;
        // Use the Assignment.Find function to find the current assignment
        lock (AdminManager.BlMutex)
            currentAssignment = s_dal.Assignment.Read(assignment => assignment.VolunteerId == id && !assignment.EndTime.HasValue);

        // If didnt find a current assignment return None
        if (currentAssignment == null)
            return BO.CallType.None;

        DO.Call? currentCall;
        lock (AdminManager.BlMutex)
            currentCall = s_dal.Call.Read(c => c.Id == currentAssignment.CallId);

        if (currentCall == null)
            return BO.CallType.None;

        return (BO.CallType)currentCall.CType;

    }

    /// <summary>
    /// checks if the certain id is a manger 
    /// </summary>
    internal static bool IsManager(int requesterId)
    {
        DO.Volunteer? requester;
        lock (AdminManager.BlMutex)
            requester = s_dal.Volunteer.Read(v => v.Id == requesterId);
        if (requester == null)
            throw new BO.BlDoesNotExistException($"Volunteer with ID {requesterId} not found.");
        return (BO.Role)requester.VRole == BO.Role.Manager;
    }

    /// <summary>
    /// The "main" valid function - calls the for each function
    /// </summary>
    internal static void ValidateVolunteerFieldsExceptAddress(BO.Volunteer volunteer)
    {
        if (!IsValidId(volunteer.Id))
        {
            throw new BlInvalidInputException("Invalid ID format.");
        }

        if (!IsValidEmail(volunteer.Email))
        {
            throw new BlInvalidInputException("Invalid email format.");
        }

        if (!IsValidPhoneNumber(volunteer.PhoneNumber))
        {
            throw new BlInvalidInputException("Invalid phone number format.");
        }
        if (RatePassword(volunteer.Password) == 0)
        {
            throw new BlInvalidInputException("This password is too weak, please try again with a stronger password");
        }
    }

    internal static int RatePassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 5 || password.Length>10)
            return 0;

        bool hasUpperCase = password.Any(char.IsUpper);
        bool hasSpecialChar = password.Any(ch => "!@#$%^&*()-_=+[]{}".Contains(ch));
        bool hasDigit = password.Any(char.IsDigit);

        int strengthScore = 0;
        if (hasUpperCase) strengthScore++;
        if (hasSpecialChar) strengthScore++;
        if (hasDigit) strengthScore++;

        if (strengthScore == 3) return 2; //2 = strong
        if (strengthScore == 2) return 1; //1 = meduim
        return 0; //weak
    }

    /// <summary>
    /// Check if id is valid
    /// </summary>
    internal static bool IsValidId(int id)
    {
        // Ensure the ID has exactly 9 digits
        if (id < 100000000 || id > 999999999)
        {
            return false;
        }

        int sum = 0;  // To accumulate the sum of processed digits
        bool isMultiplierOne = true;  // Start with multiplier 1

        // Process each digit from left to right
        for (int i = 8; i >= 0; i--) // Start from the leftmost digit
        {
            int digit = (id / (int)Math.Pow(10, i)) % 10; // Extract each digit
            int product = digit * (isMultiplierOne ? 1 : 2);  // Multiply by 1 or 2

            // If the product is greater than 9, sum the digits
            if (product > 9)
            {
                product = product / 10 + product % 10;
            }

            sum += product;

            // Alternate multiplier between 1 and 2
            isMultiplierOne = !isMultiplierOne;
        }

        // Check if the sum is divisible by 10
        return sum % 10 == 0;
    }

    /// <summary>
    /// Check if the email is valid or not
    /// </summary>
    internal static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // try to parse email using MailAddress(), if email is not a valid email the parsing fails and throw an exception
            var mailAddress = new MailAddress(email);
            return true; // if reach here email cotain a valid email
        }
        catch (FormatException)
        {
            return false; // Invalid email
        }
    }

    /// <summary>
    /// Check if the phone number is valid
    /// </summary>
    internal static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Validate that the phone number starts with '0' and is exactly 10 digits long
        return Regex.IsMatch(phoneNumber, @"^0\d{9}$");
    }

    /// <summary>
    /// The function updates the volunteer details (in the manger control)
    /// </summary>
    public static void UpdateVolunteerDetailsInManager(int requesterId, BO.Volunteer updatedVolunteer)
    {
        try
        {
            if (updatedVolunteer == null)
            {
                throw new BlInvalidInputException("Error - Failed to update volunteer details: updated volunteer is not valid");
            }

            DO.Volunteer? existingVolunteer;
            // Retrieve the volunteer record from the DAL
            lock (AdminManager.BlMutex)
                existingVolunteer = s_dal.Volunteer.Read(v => v.Id == updatedVolunteer.Id);

            if (existingVolunteer == null)
            {
                throw new BlDoesNotExistException($"volunteer with Id {updatedVolunteer.Id} does not exists and therefore can not be updated");
            }

            // Check permissions
            if (requesterId != updatedVolunteer.Id && !VolunteerManager.IsManager(requesterId))
            {
                throw new BlCanNotUpdateException($"Only a manager or the volunteer itself can update its details");
            }

            // Identify changes and check if they are permissible
            if (IsManager(requesterId) && (BO.Role)existingVolunteer.VRole != updatedVolunteer.VRole) // If requester is not a manager and the role is being changed throw an exception
            {
                throw new BlCanNotUpdateException("Only a manager can modify the volunteer role");
            }

            // Validate all fields except for the address that requires a call to the external API so we do it separately to save time
            ValidateVolunteerFieldsExceptAddress(updatedVolunteer);

            // Map updated BO volunteer to DO volunteer
            var doVolunteer = new DO.Volunteer
            {
                Id = updatedVolunteer.Id,
                FullName = updatedVolunteer.FullName,
                PhoneNumber = updatedVolunteer.PhoneNumber,
                Email = updatedVolunteer.Email,
                Password = updatedVolunteer.Password.Length <= 10 ? getHashPassword(updatedVolunteer.Password) : updatedVolunteer.Password, //the max password length is 10- if its more it means it already been hashed
                FullAddress = string.Empty,
                Active = updatedVolunteer.Active,
                Distance = updatedVolunteer.Distance,
                VRole = (DO.Role)updatedVolunteer.VRole,
                VDisType = (DO.DistanceType)updatedVolunteer.VDisType,
                Latitude = 0.0,
                Longitude = 0.0
            };

            // if the volunteer is updated to an inactive state, and has a call in progress, the assignment of the volunteer to this call should be canceled
            // in this scenario we cancell the assignment before updating the volunterr.
            // this is because the second operation we perform might fail while the first one succeeded,
            // and in that case we prefer that the cancellation will succeed and the update to inactive will fail, than the other way around
            // because having an inactive volunteer with a call in progress is a more severe problem in our eyes than having an active volunteer with a call that was canceled by mistake

            if (updatedVolunteer.Active == false && updatedVolunteer.CallInProgress != null)
            {
                // if the volunteer updates to an inactive state, and has a call in progress, the assignment of the volunteer to this call should be canceled
                Tools.CancelAssignmentInTools(requesterId, updatedVolunteer.CallInProgress.AsmId);
            }

            // Update the record in the DAL
            lock (AdminManager.BlMutex)
                s_dal.Volunteer.Update(doVolunteer);



            VolunteerManager.Observers.NotifyItemUpdated(updatedVolunteer.Id);  // notify observers that the logical state of the BO.Volunteer with that id has changed and may not match the current visual representation
            VolunteerManager.Observers.NotifyListUpdated();  // notify observers that the logical state of the list of BO.Volunteers has changed and may not match the current visual representation
            CallManager.Observers.NotifyListUpdated();    // notify observers that the logical state of the lists of objects of call related BO entitis has changed and may not match the current visual representation


            // after we done with updating the volunteer fields except for the address and coordinates, and also notified the observers, we can now update the address and coordinates

            // creating a new object of DO.Volunteer wth the new address (unlike doVolunteer which had the old address since we didnt know if the new address is valid when sending him in the update function)
            DO.Volunteer doVolunteerWithNewAddress = doVolunteer with { FullAddress = updatedVolunteer.FullAddress };

            //compute the coordinates asynchronously without waiting for the results
            _ = VolunteerManager.validateAddressAndUpdateAddressAndCoordinatesForVolunteerAsync(doVolunteerWithNewAddress)
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    var taskException = task.Exception?.InnerException ?? task.Exception;
                    if (taskException != null)
                        throw new BlInvalidInputException("error updating address, the updated volunteer does not have an address for now. error desails:", taskException);
                    else
                        throw new BlInvalidInputException("error updating address, the updated volunteer does not have an address for now");
                }
            });
        }
        catch (Exception ex)
        {
            // Log the error or handle it as needed
            throw new BlCanNotUpdateException("Failed to update details.", ex);
        }
    }

    /// <summary>
    /// The function checks if the address is valid (Async)
    /// </summary> 
    /// <param name="doVolunteer">The volunteer to check his address</param>
    /// <returns></returns>
    /// <exception cref="BlCanNotUpdateException"></exception>
    internal static async Task validateAddressAndUpdateAddressAndCoordinatesForVolunteerAsync(DO.Volunteer doVolunteer)
    {
        try
        {
            // Validate the address and get the real coordinates if its valid
            (double? latitude, double? longitude) = await Tools.ValidateAddressAndGetAddressCoordinates(doVolunteer.FullAddress);

            // setting doVolunteer latitude and longitude to be the real latitude and longitude of its address
            doVolunteer = doVolunteer with { Latitude = latitude, Longitude = longitude };

            // Update the record in the DAL
            lock (AdminManager.BlMutex)
                s_dal.Volunteer.Update(doVolunteer);

            VolunteerManager.Observers.NotifyItemUpdated(doVolunteer.Id);  // notify observers that the logical state of the BO.Volunteer with that id has changed and may not match the current visual representation
            VolunteerManager.Observers.NotifyListUpdated();  // notify observers that the logical state of the list of BO.Volunteers has changed and may not match the current visual representation
            CallManager.Observers.NotifyListUpdated();    // notify observers that the logical state of the lists of objects of call related BO entitis has changed and may not match the current visual representation
        }
        catch (Exception ex)
        {
            // Log the error or handle it as needed
            throw new BlCanNotUpdateException("Failed to update details.", ex);
        }
    }

    /// <summary>
    /// Estimates (guesses) a time to take care of the call (for the simolator)
    /// </summary>
    /// <param name="distanceFromVolunteerToCall">The distance between the volunteer and the call</param>
    /// <returns>the time he have to finish(TimeSpan)</returns>
    internal static TimeSpan timeOfHandalingEstimation(double distanceFromVolunteerToCall)
    {
        int volunteerSpeedInKmPerHour = s_rand.Next(5, 80);
        TimeSpan arrivingTime = TimeSpan.FromHours(distanceFromVolunteerToCall / volunteerSpeedInKmPerHour);
        TimeSpan timeOfHnadalingOtherThanArrivingTime = TimeSpan.FromMinutes(s_rand.Next(10, 181));
        return arrivingTime + timeOfHnadalingOtherThanArrivingTime;
    }


    /// <summary>
    /// The simulation for the volunteer 
    /// </summary>
    internal static void volunteerSimulation()
    {
        Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";
        List<DO.Volunteer> doVolunteerList;

        lock (AdminManager.BlMutex) //stage 7
            doVolunteerList = s_dal.Volunteer.ReadAll(v => v.Active == true).ToList();

        foreach (var doVolunteer in doVolunteerList)
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                BO.CallInProgress? callInProgress;
                lock (AdminManager.BlMutex) //stage 7
                    callInProgress = VolunteerManager.GetCurrentCall(doVolunteer.Id);
                if (callInProgress == null)
                {
                    IEnumerable<BO.OpenCallInList> openCallsOfVolunteer = CallManager.GetOpenCallsOfVolunteerInManager(doVolunteer.Id, null, null);
                    int numOfOpenCalls = openCallsOfVolunteer.Count();
                    if (numOfOpenCalls != 0 && s_rand.Next(0, 5) == 0)   // 20% chance to choose a call if there are open calls
                    {
                        int callId = openCallsOfVolunteer.Skip(s_rand.Next(0, numOfOpenCalls)).First().Id;
                        CallManager.ChooseCallInManager(doVolunteer.Id, callId);
                    }
                }
                else
                {
                    TimeSpan timeOfHandaling = timeOfHandalingEstimation(callInProgress.Distance);

                    if (callInProgress.EnterTime + timeOfHandaling > AdminManager.Now)
                        CallManager.EndCallInManager(doVolunteer.Id, callInProgress.AsmId);

                    else if (s_rand.Next(0, 10) == 0)   // 10% chance to cancel the call if not enough time has passed to finish the call
                        Tools.CancelAssignmentInTools(doVolunteer.Id, callInProgress.AsmId);
                }
            }
        }
    }
    /// <summary>
    /// The function returns the hash of the string
    /// </summary>
    /// <param name="input">To encrypt (string)</param>
    /// <returns>encrypted password</returns>
    internal static string getHashPassword(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2")); // Convert byte to hex format
            }
            return builder.ToString();
        }
    }

    /// <summary>
    /// The function generate password for volunteer - strong password
    /// </summary>
    /// <returns>The new password(string)</returns>
    internal static string GeneratePassword()
    {
        int length = 8;

        Random random = new Random();

        // atleast one of each
        char upperCase = (char)random.Next(65, 91); // (A-Z)
        char specialChar = (char)new[] { 33, 35, 36, 38, 42, 64 }[random.Next(6)]; //special char
        char digit = (char)random.Next(48, 58); // 0 - 9

        string allCharacters = string.Concat(Enumerable.Range(97, 26).Select(i => (char)i));
        string remainingChars = new string(Enumerable.Repeat(allCharacters, length - 3)
                                                 .Select(s => s[random.Next(s.Length)])
                                                 .ToArray());

        string password = upperCase + specialChar.ToString() + digit + remainingChars;
        return new string(password.ToCharArray().OrderBy(c => random.Next()).ToArray());
    }
}
