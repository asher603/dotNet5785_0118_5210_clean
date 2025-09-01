namespace Helpers;

using DalApi;
using BO;
using DO;
using System.Net.Mail;
using System.Net;
using Secrets;




internal static class Tools
{
    private static IDal s_dal = Factory.Get;

    private static readonly HttpClient client = new HttpClient();

    private const double EarthRadiusKm = 6371.0;


    public static void CancelAssignmentInTools(int requestorId, int assignmentId)
    {
        try
        {
            DO.Volunteer requestorVolunteer;
            DO.Assignment? assignment;
            lock (AdminManager.BlMutex)
                requestorVolunteer = s_dal.Volunteer.Read(vol => vol.Id == requestorId) ?? throw new BlDoesNotExistException($"Volunteer does not exists with id = {requestorId}");
            lock (AdminManager.BlMutex)
                assignment = s_dal.Assignment.Read(asm => asm.Id == assignmentId) ?? throw new BlDoesNotExistException($"Assignment does not exists with id = {assignmentId}");
            int handaligVolunteerId = assignment.VolunteerId;

            if (requestorVolunteer == null)
                throw new BlDoesNotExistException($"Your id ({requestorId}) does not exist in the system");

            if (assignment == null)
                throw new BlDoesNotExistException($"Assignment with id = {assignmentId} does not exist");

            CallStatus status = CallManager.getStatus(assignment.CallId);
            if (assignment.EndTime != null || assignment.EType != null || (status == CallStatus.Expired && status == CallStatus.Closed))
                throw new BlCanNotCanceleException($"The assignment with id = {assignmentId} has already ended and therefore can not be canceled");

            // if requestor is not a manager and he is also not the handaling volunteer in the assignment he wish to cancel then he can not cancel that assignment
            if (requestorVolunteer.VRole != DO.Role.Manager && handaligVolunteerId != requestorId)
                throw new BlCanNotCanceleException($"Failed to cancel the assignment with id = {assignmentId}, you must be a manager or the handaling volunteer to cancel this assignment");

            // reach here only if the requestor is a manager or the handaling volunteer, so we can compare the requestor id with the handaling volunteer id to determine the ending type
            DO.EndingType endingType = requestorId == handaligVolunteerId ? DO.EndingType.SelfCanceld : DO.EndingType.CanceledByManager;


            lock (AdminManager.BlMutex)
                s_dal.Assignment.Update(new Assignment()
                {
                    Id = assignment.Id,
                    CallId = assignment.CallId,
                    VolunteerId = assignment.VolunteerId,
                    EnterTime = assignment.EnterTime,
                    EType = endingType,
                    EndTime = AdminManager.Now,

                });

            VolunteerManager.Observers.NotifyItemUpdated(handaligVolunteerId);  // notify observers that the logical state of the BO.Volunteer with that id has changed and may not match the current visual representation
            VolunteerManager.Observers.NotifyListUpdated();  // notify observers that the logical state of the BO.Voluntter list has changed and may not match the current visual representation
            CallManager.Observers.NotifyItemUpdated(assignment.CallId);  // notify observers that the logical state of the BO.Call with that id has changed and may not match the current visual representation
            CallManager.Observers.NotifyListUpdated();    // notify observers that the logical state of the lists of objects of call related BO entitis has changed and may not match the current visual representation

            // if the requestorVolunteer is a manager we send mail to notify the handling volunteer about the cancellation
            if (requestorVolunteer.VRole == DO.Role.Manager)
            {
                _ = Tools.SendingMailUponAssignmentCancellation(assignment);
            }

            DO.Call callInCanelledAssignment;
            lock (AdminManager.BlMutex)
                callInCanelledAssignment = s_dal.Call.Read(c => c.Id == assignment.CallId) ?? throw new BlDoesNotExistException($"Call with id = {assignment.CallId} does not exist");

            // sending mail to notify relevent volunteers about the call in the cancelled assignment which is open now
            _ = CallManager.SendingMailUponOpeningOfACall(callInCanelledAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"Assignment with ID={assignmentId} does Not exists", ex);
        }
    }


    internal static async Task SendingMailUponAssignmentCancellation(DO.Assignment asm)
    {
        var systemMail = new MailAddress(Secrets.SYSTEM_MAIL_PASSWORD, "Help On Road");    // the mail we use to send the emails from
        const string systemMailPassword = Secrets.SYSTEM_MAIL_PASSWORD;
        MailAddress? volunteerMail;
        DO.Volunteer? volunteer;
        DO.Call? call;
        const string subject = "Assignment Cancellation Notification";
        var smtpClient = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(systemMail.Address, systemMailPassword)
        };

        lock (AdminManager.BlMutex)
            volunteer = s_dal.Volunteer.Read(v => v.Id == asm.VolunteerId) ?? throw new BlDoesNotExistException($"Volunteer with id = {asm.VolunteerId} does not exist");
        lock (AdminManager.BlMutex)
            call = s_dal.Call.Read(c => c.Id == asm.CallId) ?? throw new BlDoesNotExistException($"Call with id = {asm.CallId} does not exist");

        string body = "Your assignment to the call with the following details has been cancelled:\n\n" + call.ToString();

        volunteerMail = new MailAddress(volunteer.Email, volunteer.FullName);
        
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



    public static double calculateDistance(double? lat1, double? lon1, double? lat2, double? lon2)
    {
        if (lat1 == null || lon1 == null || lat2 == null || lon2 == null) throw new BlNullReferenceException("Latitude or Longitude is null");

        var dLat = ToRadians((double)(lat2 - lat1));
        var dLon = ToRadians((double)(lon2 - lon1));

        // Haversine formula
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c; // Distance in kilometers
    }
    private static double ToRadians(double degree) => degree * (Math.PI / 180.0);
    internal static double getDistance(int volunteerId, DO.Call call)
    {  
            DO.Volunteer? volunteer = s_dal.Volunteer.Read(vol => vol.Id == volunteerId);

            if (volunteer == null) 
                throw new BlDoesNotExistException("No such volunteer with this id");

            return Tools.calculateDistance(call.Latitude, call.Longitude, volunteer.Latitude, volunteer.Longitude);
    }

    // get the real latitude and longitude of an address
    public static async Task<(double? Latitude, double? Longitude)> GetAddressCoordinatesAsync(string? address)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(address))
                return (null, null);


            // API URL with the key and the encoded address
            string apiKey = Secrets.GEOCODE_API_KEY;
            string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={apiKey}";
            
            // Sending the GET request
            HttpResponseMessage response = await client.GetAsync(url);

            // Ensure the response indicates success
            response.EnsureSuccessStatusCode();
            
            // Reading the response
            string responseBody = await response.Content.ReadAsStringAsync();

            // Deserializing the JSON response using System.Text.Json
            var results = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object>>>(responseBody);
            

            // Extracting latitude and longitude
            if (results != null && results.Count > 0)
            {
                var firstResult = results[0];
                double? latitude = firstResult.ContainsKey("lat") ? Convert.ToDouble(firstResult["lat"].ToString()) : (double?)null;
                double? longitude = firstResult.ContainsKey("lon") ? Convert.ToDouble(firstResult["lon"].ToString()) : (double?)null;

                return (latitude, longitude);
            }

            return (null, null); // Return (null, null) if no results found
        }
        catch (HttpRequestException) // we don't need to use ex itself, we only need to return (null, null)
        {
            return (null, null);
        }
        catch (Exception)       // we don't need to use ex itself, we only need to return (null, null)
        {   
            return (null, null);
        }     
    }

    internal static async Task<(double? Latitude, double? Longitude)> ValidateAddressAndGetAddressCoordinates(string? address)
    {
        (double? latitude, double? longitude) = await Tools.GetAddressCoordinatesAsync(address);

        if (longitude == null || latitude == null)
        {
            throw new BlInvalidInputException("Invalid address.");
        }
        return (latitude, longitude);
    }

    public static string ToStringProperty<T>(this T t)
    {
        if (t == null) return "null";

        var properties = typeof(T).GetProperties();
        var propertyStrings = properties
            .Select(p =>
            {
                var value = p.GetValue(t);
                if (value is IEnumerable<object> collection && !(value is string))
                {
                    var collectionValues = string.Join(", ", collection.Cast<object>().Select(item => item?.ToString() ?? "null"));
                    return $"{p.Name}: [{collectionValues}]";
                }
                return $"{p.Name}: {value?.ToString() ?? "null"}";
            })
            .ToArray();

        return string.Join(", ", propertyStrings);
    }
}
