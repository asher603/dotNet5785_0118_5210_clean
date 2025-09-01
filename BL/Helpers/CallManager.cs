namespace Helpers;

using BlApi;
using BO;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Secrets;

internal static class CallManager
{
    private static IDal s_dal = DalApi.Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 
    private static int s_periodicCounter = 0;


    internal static CallStatus getStatus(int cId)
    {
        DO.Assignment? asm;
        DO.Call call;
        lock (AdminManager.BlMutex)
            asm = s_dal.Assignment.ReadAll(asm => asm.CallId == cId)?.LastOrDefault();
        lock (AdminManager.BlMutex)
            call = s_dal.Call.Read(c => c.Id == cId) ?? throw new BlDoesNotExistException($"The call with id= {cId} doesn't exist");

        bool inRiskRange = AdminManager.Now >= call.MaxTime - AdminManager.RiskRange && AdminManager.Now <= call.MaxTime;

        if (asm == null || asm.EType == DO.EndingType.SelfCanceld || asm.EType == DO.EndingType.CanceledByManager)
        {
            if (call.MaxTime > AdminManager.Now)
            {
                if (inRiskRange)
                    return CallStatus.OpenInRisk;
                return CallStatus.Open;
            }
            return CallStatus.Expired;
        }

        // if reach here asm != null so we can safely use asm.EType from now on in this method

        if (asm.EType == DO.EndingType.Solved)
            return CallStatus.Closed;

        if (asm.EType == DO.EndingType.Expired)
            return CallStatus.Expired;

        // if reach here the call is in progress or in progress in risk
        if (inRiskRange)
            return CallStatus.InProgressInRisk;
        return CallStatus.InProgress;
    }


    internal static void ValidateCallFieldsExceptAddress(BO.Call call)
    {

        if (call.Opening > call.MaxTime)
            throw new BlInvalidInputException("Invalid opening and max times, opening time can not be greater than max time");
    }

    internal static IEnumerable<CallInList> returnCallInList()
    {
        IEnumerable<DO.Call> IEnCalls;
        lock (AdminManager.BlMutex)
            IEnCalls = s_dal.Call.ReadAll();
        IEnumerable<DO.Assignment> allAsmOfCall;
        string? lastVolunteer;
        IEnumerable<CallInList?> CollectionWithPossibleNull = IEnCalls.Select(call =>
        {

            lock (AdminManager.BlMutex)
                allAsmOfCall = s_dal.Assignment.ReadAll(asm => asm.CallId == call.Id); // Fetch all related assignments
            DO.Assignment? asm = allAsmOfCall.Any() ? allAsmOfCall.Last() : null; // Check if the list is empty before using .Last()

            lock (AdminManager.BlMutex)
                lastVolunteer = asm != null ? s_dal.Volunteer.Read(vol => vol.Id == asm.VolunteerId)?.FullName : null;

            return new CallInList
            {
                Id = asm != null ? asm.Id : null,
                CallId = call.Id,
                CType = (BO.CallType)(int)call.CType,
                Opening = call.Opening,
                TimeLeft = (call.MaxTime > AdminManager.Now) ? call.MaxTime - AdminManager.Now : null,
                Status = getStatus(call.Id),
                HandlingTime = (asm != null && asm.EndTime.HasValue) ? asm.EndTime - asm.EnterTime : null,
                LastVolunteer = lastVolunteer,
                TotalAssignments = allAsmOfCall.Count()
            };
        });

        IEnumerable<CallInList> CollectionToReturn = CollectionWithPossibleNull.Where(call => call != null).Select(call => call!);
        return CollectionToReturn;
    }

    internal static List<BO.CallAssignInList> listOfCallAssign(int callId)
    {
        IEnumerable<DO.Assignment> listAssign;

        lock (AdminManager.BlMutex)
            listAssign = s_dal.Assignment.ReadAll(c => c.CallId == callId);

        lock (AdminManager.BlMutex)
        {
            return listAssign.Select(asm => new BO.CallAssignInList()
            {
                VolunteerId = asm.VolunteerId,
                FullName = (s_dal.Volunteer.Read(v => v.Id == asm.VolunteerId) ?? throw new BlDoesNotExistException($"Volunteer does not exists with id = {asm.VolunteerId}")).FullName,
                EnterTime = asm.EnterTime,
                EndTime = asm.EndTime,
                EType = asm.EType != null ? (BO.EndingType)(int)asm.EType : null
            }).ToList();
        }
    }

    internal static IEnumerable<BO.ClosedCallInList> listOfClosedCall(int volunteerId)
    {
        lock (AdminManager.BlMutex)
        {
            IEnumerable<DO.Assignment> listVolunteerAss = s_dal.Assignment.ReadAll(asm => asm.VolunteerId == volunteerId);
            return (from asm in listVolunteerAss
                    let call = s_dal.Call.Read(c => c.Id == asm.CallId) ?? throw new BlDoesNotExistException($"Call does not exists with id = {asm.CallId}")
                    where asm.EndTime != null   // check if the assignment ended (from any type of ending not just solved)
                    select new BO.ClosedCallInList()
                    {
                        Id = call.Id,
                        CType = (BO.CallType)(int)call.CType,
                        FullAddress = call.FullAddress,
                        Opening = call.Opening,
                        EnterTime = asm.EnterTime,
                        EndTime = asm.EndTime,
                        EType = (BO.EndingType)(int)asm.EType!
                    }
            );
        }
    }

    internal static IEnumerable<BO.OpenCallInList> listOfOpenCall(int volunteerId)
    {
        IEnumerable<DO.Call> Calls;
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex)
        {
            Calls = s_dal.Call.ReadAll();
            volunteer = s_dal.Volunteer.Read(vol => vol.Id == volunteerId) ?? throw new BlDoesNotExistException($"Volunteer does not exists with id = {volunteerId}");
        }

        // return an empty list if the volunteer has empty address (empty address is possible when the validation of the volunteer address was not done yet, or if his address was found to be invalid) 
        if (volunteer.FullAddress == string.Empty)
            return new List<BO.OpenCallInList>();

        return (from call in Calls
                let status = getStatus(call.Id)
                let volunteerToCallDistance = Tools.getDistance(volunteerId, call)

                // the call address can be empty if the validation of the call address was not done yet, or if the address was found to be invalid. in those cases we dont want to show that call to the volunteer because we cant calculate the distance to it from the volunteer
                where (status == BO.CallStatus.Open || status == BO.CallStatus.OpenInRisk) && call.FullAddress != string.Empty && volunteerToCallDistance < volunteer.Distance
                select new BO.OpenCallInList()
                {
                    Id = call.Id,
                    CType = (BO.CallType)(int)call.CType,
                    Description = call.Description,
                    FullAddress = call.FullAddress,
                    Opening = call.Opening,
                    MaxTime = call.MaxTime,
                    Distance = volunteerToCallDistance
                });
    }

    public static bool canBeDeleted(int callId)
    {
        DO.Assignment? currentAssignment;
        lock (AdminManager.BlMutex)
            currentAssignment = s_dal.Assignment.Read(asm => asm.CallId == callId);

        if (CallManager.getStatus(callId) == CallStatus.Open && currentAssignment == null)
            return true;

        return false;
    }


    internal static void UpdateCallInManager(BO.Call? updatedCall)
    {
        try
        {
            if (updatedCall == null)
            {
                throw new BlInvalidInputException("Error - Failed to update volunteer details: updated volunteer is not valid");
            }
            DO.Call? existingCall;
            // Retrieve the call record from the DAL
            lock (AdminManager.BlMutex)
                existingCall = s_dal.Call.Read(c => c.Id == updatedCall.Id);

            if (existingCall == null)
            {
                throw new BlDoesNotExistException($"volunteer with Id {updatedCall.Id} does not exists and therefore can not be updated");
            }


            // if invalid call fields (except for address wich is not checked at this point), an exception will be thrown here
            CallManager.ValidateCallFieldsExceptAddress(updatedCall);

            DO.Call doCall = new DO.Call()
            {
                Id = updatedCall.Id,
                FullAddress = string.Empty,
                CType = (DO.CallType)(int)updatedCall.CType,
                Description = updatedCall.Description,
                Latitude = 0.0,
                Longitude = 0.0,
                Opening = updatedCall.Opening,
                MaxTime = updatedCall.MaxTime
            };

            lock (AdminManager.BlMutex)
                s_dal.Call.Update(doCall);

            CallManager.Observers.NotifyItemUpdated(updatedCall.Id);  // notify observers that the logical state of the BO.Call with that id has changed and may not match the current visual representation
            CallManager.Observers.NotifyListUpdated();    // notify observers that the logical state of the lists of objects of call related BO entitis has changed and may not match the current visual representation

            // after we done with updating the call fields except for the address and coordinates, and also notified the observers, we can now update the address and coordinates

            // creating a new object of DO.Call wth the new address (unlike doCall which had the old address since we didnt know if the new address is valid when sending him in the update function)
            DO.Call doCallWithNewAddress = doCall with { FullAddress = updatedCall.FullAddress };

            //compute the coordinates asynchronously without waiting for the results
            _ = CallManager.validateAddressAndUpdateAddressAndCoordinatesForCallAsync(doCallWithNewAddress);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlCanNotUpdateException("Failed to update call details", ex);
        }
    }


    public static IEnumerable<OpenCallInList> GetOpenCallsOfVolunteerInManager(int volunteerId, BO.CallType? filterBy, BO.OpenCallInListField? sortBy)
    {
        IEnumerable<OpenCallInList> openCalls = CallManager.listOfOpenCall(volunteerId);

        if (filterBy != null)
            openCalls = openCalls.Where(call => call.CType == filterBy);

        openCalls = sortBy switch
        {
            OpenCallInListField.Id => openCalls.OrderBy(call => call.Id),
            OpenCallInListField.CType => openCalls.OrderBy(call => call.CType),
            OpenCallInListField.Description => openCalls.OrderBy(call => call.Description),
            OpenCallInListField.FullAddress => openCalls.OrderBy(call => call.FullAddress),
            OpenCallInListField.Opening => openCalls.OrderBy(call => call.Opening),
            OpenCallInListField.MaxTime => openCalls.OrderBy(call => call.MaxTime),
            OpenCallInListField.Distance => openCalls.OrderBy(call => call.Distance),
            null => openCalls.OrderBy(call => call.Id), // if sortBy is null - sort by Id
            _ => throw new BlInvalidInputException("Error - invalid field to sort by.")
        };

        return openCalls;
    }


    public static BO.Call GetCallDetailsInManager(int callId)
    {
        DO.Call? cl;
        lock (AdminManager.BlMutex)
            cl = s_dal.Call.Read(c => c.Id == callId) ?? throw new BlDoesNotExistException($"Call does not exists with id = {callId}");
        return new BO.Call()
        {
            Id = callId,
            CType = (BO.CallType)(int)cl.CType,
            Description = cl.Description,
            FullAddress = cl.FullAddress,
            Longitude = cl.Longitude,
            Latitude = cl.Latitude,
            Opening = cl.Opening,
            MaxTime = cl.MaxTime,
            CallAssignInList = CallManager.listOfCallAssign(callId),
            Status = CallManager.getStatus(callId)
        };
    }


    public static void ChooseCallInManager(int volunteerId, int callId)
    {
        //not the way they wanted - dont know if this is good.
        BO.Call call = GetCallDetailsInManager(callId);
        if (!(call.Status == BO.CallStatus.Open || call.Status == BO.CallStatus.OpenInRisk))    // if the call is not open
            throw new BlAlreadyExistsException("this call is already assigned to a volunteer");
        if (AdminManager.Now >= call.MaxTime)
            throw new BlCallExpiredException("this call is expired");

        // create a new Assignment without specifying the Id, as it will be auto-generated by the Create method (it will be set to a running number)
        lock (AdminManager.BlMutex)
            s_dal.Assignment.Create(new DO.Assignment()
            {
                CallId = call.Id,
                VolunteerId = volunteerId,
                EnterTime = AdminManager.Now,
                EType = null,
                EndTime = null
            });
        VolunteerManager.Observers.NotifyItemUpdated(volunteerId);  // notify observers that the logical state of the BO.Volunteer with that id has changed and may not match the current visual representation
        VolunteerManager.Observers.NotifyListUpdated();  // notify observers that the logical state of the BO.Voluntter list has changed and may not match the current visual representation
        CallManager.Observers.NotifyItemUpdated(callId);  // notify observers that the logical state of the BO.Call with that id has changed and may not match the current visual representation
        CallManager.Observers.NotifyListUpdated();    // notify observers that the logical state of the lists of objects of call related BO entitis has changed and may not match the current visual representation
    }


    public static void EndCallInManager(int volunteerId, int assignmentId)
    {
        DO.Assignment assignment;
        lock (AdminManager.BlMutex)
            assignment = s_dal.Assignment.Read(asm => asm.Id == assignmentId) ?? throw new BlDoesNotExistException($"Assignment does not exists with id = {assignmentId}");
        if (assignment.VolunteerId != volunteerId)
            throw new BlCanNotEndCallException($"The volunteer with id = {volunteerId} does not handle this call and therefore can not end it");
        if (assignment.EndTime != null || assignment.EType != null)
            throw new BlCanNotEndCallException($"The call with id = {assignment.Id} is not in progress");
        try
        {
            lock (AdminManager.BlMutex)
                s_dal.Assignment.Update(new DO.Assignment()
                {
                    Id = assignmentId,
                    CallId = assignment.CallId,
                    VolunteerId = assignment.VolunteerId,
                    EnterTime = assignment.EnterTime,
                    EndTime = AdminManager.Now,
                    EType = DO.EndingType.Solved
                });
            VolunteerManager.Observers.NotifyItemUpdated(volunteerId);  // notify observers that the logical state of the BO.Volunteer with that id has changed and may not match the current visual representation
            VolunteerManager.Observers.NotifyListUpdated();  // notify observers that the logical state of the BO.Voluntter list has changed and may not match the current visual representation
            CallManager.Observers.NotifyItemUpdated(assignment.CallId);  // notify observers that the logical state of the BO.Call with that id has changed and may not match the current visual representation
            CallManager.Observers.NotifyListUpdated();    // notify observers that the logical state of the lists of objects of call related BO entitis has changed and may not match the current visual representation

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"Assignment with ID={assignmentId} does Not exists", ex);
        }
    }


    internal static async Task SendingMailUponOpeningOfACall(DO.Call call)
    {
        var systemMail = new MailAddress(Secrets.SYSTEM_MAIL_ADDRESS, "Help On Road");    // the mail we use to send the emails from
        const string systemMailPassword = Secrets.SYSTEM_MAIL_PASSWORD; // the password of the mail we use to send the emails from
        const string subject = "Call Opend in Your Area";
        IEnumerable<DO.Volunteer> doVolunteersToNotify;

        // retrieve all volunteers that should get an email about the call that was opened
        lock (AdminManager.BlMutex)
            doVolunteersToNotify = s_dal.Volunteer.ReadAll(v => Tools.getDistance(v.Id, call) <= v.Distance && v.Active == true);

        // initialize SMTP client
        var smtpClient = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(systemMail.Address, systemMailPassword)
        };

        // send email to each volunteer
        foreach (var volunteer in doVolunteersToNotify)
        {         
            MailAddress volunteerMail;

            lock (AdminManager.BlMutex)
                volunteerMail = new MailAddress(volunteer.Email, volunteer.FullName);

            string body = $"We can use your help! \nA call with the following details has been opened in your area: \n\n" + call.ToString();

            using (var message = new MailMessage(systemMail, volunteerMail)
            {
                Subject = subject,
                Body = body
            })
            try
            {
                await smtpClient.SendMailAsync(message);
            }
            catch (SmtpException smtpEx)
            {
                throw new BlCanNotUpdateException($"SMTP error while sending email to {volunteer.FullName}: {smtpEx.Message}");
            }
            catch (InvalidOperationException invOpEx)
            {
                throw new BlCanNotUpdateException($"Invalid operation while sending email to {volunteer.FullName}: {invOpEx.Message}");
            }
            catch (Exception ex)
            {
                throw new BlCanNotUpdateException($"Unexpected error while sending email to {volunteer.FullName}: {ex.Message}");
            }
        }
    }


    internal static async Task ValidationAndCalculationOfNetRelatedDetailsInAddedCallAndSendingMailUponCallOpening(DO.Call addedCall)
    {
        try
        {
            //compute the coordinates asynchronously ans wait for the results (they are needed for the SendingMailUponOpeningOfACall func which we call after this line)
            await CallManager.validateAddressAndUpdateAddressAndCoordinatesForCallAsync(addedCall);

            // reciving the updated call from the DAL, the call now contains the real latitude and longitude of the address (or 0.0 and 0.0 if the address is invalid)
            DO.Call? addedCallWithRealNetRecivedFields = s_dal.Call.Read(c => c.Id == addedCall.Id) ?? throw new BlDoesNotExistException($"The call with id= {addedCall.Id} doesn't exist");

            // sending mail to notify relevent volunteers about the call in the cancelled assignment which is open now (no need to wait for the results)
            _ = CallManager.SendingMailUponOpeningOfACall(addedCallWithRealNetRecivedFields);
        }
        catch(Exception ex)
        {
            throw new BlCanNotUpdateException("Failed to update details.", ex);
        }
    }

    internal static async Task validateAddressAndUpdateAddressAndCoordinatesForCallAsync(DO.Call doCall)
    {
        try
        {
            // Validate the address and get the real coordinates if its valid
            (double? latitude, double? longitude) = await Tools.ValidateAddressAndGetAddressCoordinates(doCall.FullAddress);

            // setting doCall latitude and longitude to be the real latitude and longitude of its address
            // latitude and longitude are not null if reached here if they were null an exception would have been thrown in while in the ValidateAddressAndGetAddressCoordinates function that we called earlier
            doCall = doCall with { Latitude = (double)latitude!, Longitude = (double)longitude! };

            // Update the record in the DAL
            lock (AdminManager.BlMutex)
                s_dal.Call.Update(doCall);

            CallManager.Observers.NotifyItemUpdated(doCall.Id);  // notify observers that the logical state of the BO.Call with that id has changed and may not match the current visual representation
            CallManager.Observers.NotifyListUpdated();    // notify observers that the logical state of the lists of objects of call related BO entitis has changed and may not match the current visual representation
        }
        catch (Exception ex)
        {
            // Log the error or handle it as needed
            throw new BlCanNotUpdateException("Failed to update details.", ex);
        }
    }




    // updating the assignments of expierd calls
    public static void PeriodicAssignmentsUpdates(DateTime newClock)
    {
        Thread.CurrentThread.Name = $"Periodic{++s_periodicCounter}"; //stage 7 (optional)

        bool assignmentUpdated = false; 

        List<DO.Assignment> assignmentsCollection;
        lock (AdminManager.BlMutex)
            assignmentsCollection = s_dal.Assignment.ReadAll().ToList();

        DO.Call callOfAsm;
        foreach (DO.Assignment asm in assignmentsCollection)
        {
            lock (AdminManager.BlMutex)
                callOfAsm = s_dal.Call.Read(c => c.Id == asm.CallId) ?? throw new BlDoesNotExistException($"The call with id= {asm.CallId} doesn't exist");
            
            if (asm.EndTime == null && newClock > callOfAsm.MaxTime)
            {
                DO.Assignment updatedAsm = asm with { EndTime = callOfAsm.MaxTime, EType = DO.EndingType.Expired };
                lock (AdminManager.BlMutex)
                    s_dal.Assignment.Update(updatedAsm);

                assignmentUpdated = true;

                CallManager.Observers.NotifyItemUpdated(asm.CallId);  // notify observers that the logical state of the BO.Call with that id has changed and may not match the current visual representation
                VolunteerManager.Observers.NotifyItemUpdated(asm.VolunteerId);  // notify observers that the logical state of the BO.Volunteer with that id has changed and may not match the current visual representation
            }

            if (assignmentUpdated)
            {
                VolunteerManager.Observers.NotifyListUpdated();  // notify observers that the logical state of the BO.Voluntter list has changed and may not match the current visual representation       
                CallManager.Observers.NotifyListUpdated();    // notify observers that the logical state of the lists of objects of call related BO entitis has changed and may not match the current visual representation
            }
        }
    }


}
