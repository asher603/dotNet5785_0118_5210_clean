
namespace BlTest;
using BO;
using DO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;

internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    static private void AdminMenu()
    {
        AdminMenuEn inp;
        int inpt;
        do
        {
            Console.WriteLine("0 - EXIT \n1 - Get Clock\n2 - Advance Clock\n3 - Get Risk Range\n4 - Set Risk Range\n5 - Reset Data Base\n6 - Initialize Data Base");
            int input;
            int.TryParse(Console.ReadLine(), out input);
            inp = (AdminMenuEn)(input);
            switch (inp)
            {
                case AdminMenuEn.EXIT: break;
                case AdminMenuEn.GetClock:
                    Console.WriteLine(s_bl.Admin.GetClock());
                    break;
                case AdminMenuEn.AdvanceClock:
                    Console.WriteLine("0 - Minute, 1 - Hour, 2 - Day, 3 - Month, 4 - Year");
                    int.TryParse(Console.ReadLine(), out inpt);
                    s_bl.Admin.AdvanceClock((BO.TimeUnit)(inpt));
                    break;
                case AdminMenuEn.GetRiskRange:
                    Console.WriteLine(s_bl.Admin.GetRiskRange());
                    break;
                case AdminMenuEn.SetRiskRange:
                    //how do i get an time span
                    TimeSpan newRisk = getNewRisk();
                    s_bl.Admin.SetRiskRange(newRisk);
                    break;
                case AdminMenuEn.ResetDB:
                    s_bl.Admin.ResetDB();
                    break;
                case AdminMenuEn.InitializeDB:
                    s_bl.Admin.InitializeDB();
                    break;
                default: break;
            }

        }
        while (inp != AdminMenuEn.EXIT);
    }

    static private void VolunteerMenu()
    {
        VolunteerMenuEn inp;
        int inpt;
        int input;
        int volId;
        int requestorVolId;
        BO.Volunteer vol;
        do
        {
            Console.WriteLine("0 - EXIT \n1 - Volunteer Registration\n2 - Get Volunteers List\n3 - Get Volunteer Details\n4 - Update Volunteer Details" +
                "\n5 - Delete Volunteer\n6 - AddVolunteer");
            int.TryParse(Console.ReadLine(), out input);
            inp = (VolunteerMenuEn)(input);
            switch (inp)
            {
                case VolunteerMenuEn.EXIT: break;
                case VolunteerMenuEn.VolunteerRegistration:
                    Console.WriteLine("Please enter an id of the new volunteer: ");
                    Console.WriteLine("Please enter the volunteer ID: ");
                    if (!int.TryParse(Console.ReadLine(), out volId))
                        throw new BlInvalidInputException("Invalid input. id must be a number.");
                    Console.WriteLine(s_bl.Volunteer.GetVolunteerRole(volId));
                    break;
                case VolunteerMenuEn.GetVolunteersList:
                    VolunteerFilterOptions? fieldToFilterBy;
                    OnScreenVolunteerFilterOptions? valueToFilterBy;
                    VolunteerInListFields? fieldToSortBy;

                    Console.WriteLine("Whould you like to filter? and if yes by what?\n 0 - No\n 1 - filter by active\n 2- flter by call type");

                    if (!int.TryParse(Console.ReadLine(), out input))
                        throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");

                    fieldToFilterBy = input switch
                    {
                        0 => null,
                        1 => VolunteerFilterOptions.Active,
                        2 => VolunteerFilterOptions.CType,
                        _ => throw new BlInvalidInputException("Error - invalid field to filter by.")
                    };
                    if (fieldToFilterBy == VolunteerFilterOptions.Active)
                    {
                        Console.WriteLine("Enter 1 for active or 0 for inactive");
                        if (!int.TryParse(Console.ReadLine(), out input))
                            throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");
                        valueToFilterBy = input switch
                        {
                            0 => OnScreenVolunteerFilterOptions.Inactive,
                            1 => OnScreenVolunteerFilterOptions.Active,
                            _ => throw new BlInvalidInputException("Error - invalid field to filter by.")
                        };
                    }
                    else if (fieldToFilterBy == VolunteerFilterOptions.CType)
                    {
                        Console.WriteLine("Enter the call type: \n0 - TireChange\n1 - JumpStart\n2 - FluidRefill\n3 - LightFix\n4 - LostKey");

                        if (!int.TryParse(Console.ReadLine(), out input))
                            throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");

                        if (!Enum.IsDefined(typeof(BO.CallType), input))
                            throw new BlInvalidInputException("Invalid input. Please enter a number corresponding to a valid call type.");

                        valueToFilterBy = (OnScreenVolunteerFilterOptions)input;
                    }
                    else
                    {
                        valueToFilterBy = null;
                    }

                    Console.WriteLine("by what field would u like to sort? \n0 - Id\n1 - Full Name\n2 - Active\n3 - Total Calls Handled\n4 -  Total Calls Cancelled\n5 - Total Calls Chosen Which Expired\n6 - Call In ProgressId\n7 - CType");
                    
                    int.TryParse(Console.ReadLine(), out inpt);

                    if (!Enum.IsDefined(typeof(BO.VolunteerInListFields), inpt))
                        throw new BlInvalidInputException("Invalid input. Please enter a number corresponding to a valid field.");

                    fieldToSortBy = (BO.VolunteerInListFields)inpt;

                    IEnumerable<BO.VolunteerInList> volunteersList = s_bl.Volunteer.GetVolunteersList(fieldToFilterBy, valueToFilterBy, fieldToSortBy);
                    
                    foreach (BO.VolunteerInList volL in volunteersList)
                    {
                        Console.WriteLine(volL);
                    }
                    break;
                case VolunteerMenuEn.GetVolunteerDetails:
                    Console.WriteLine("Please enter an id of the volunteer: ");
                    if (!int.TryParse(Console.ReadLine(), out volId))
                        throw new BlInvalidInputException("Invalid input. id must be a number.");
                    Console.WriteLine(s_bl.Volunteer.GetVolunteerDetails(volId));
                    break;
                case VolunteerMenuEn.UpdateVolunteerDetails:
                    Console.WriteLine("Please enter the id of the requstor volunteer: ");
                    if (!int.TryParse(Console.ReadLine(), out requestorVolId))
                        throw new BlInvalidInputException("Invalid input. id must be a number.");
                    vol = getNewBoVolunteer();
                    s_bl.Volunteer.UpdateVolunteerDetails(requestorVolId, vol);
                    break;
                case VolunteerMenuEn.DeleteVolunteer:
                    Console.WriteLine("Please enter an id of the volunteer: ");
                    if (!int.TryParse(Console.ReadLine(), out volId))
                        throw new BlInvalidInputException("Invalid input. id must be a number.");
                    s_bl.Volunteer.DeleteVolunteer(volId);
                    break;
                case VolunteerMenuEn.AddVolunteer:
                    vol = getNewBoVolunteer();
                    s_bl.Volunteer.AddVolunteer(vol);
                    break;
                default: break;
            }
        }
        while (inp != VolunteerMenuEn.EXIT);
    }

    static private void CallMenu()
    {
        CallMenuEn inp;
        int num, volunteerId, assignmentId, requestorId, callId;
        string str;
        object? filterValue;
        BO.CallInListField? filterBy;
        BO.CallType cType;
        BO.Call call;
        do
        {
            Console.WriteLine("0 - EXIT \n1 - Get Call QuantitiesByStatus \n2 - Get Filtered And Sorted Calls " +
                "\n3 - Get Call Details \n4 - UpdateCall \n5 - DeleteCall \n6 - AddCall " +
                "\n7 - GetClosedCallsHandledByVolunteer \n8 - GetOpenCallsOfVolunteer \n9 - EndCall" +
                "\n10 - CancelAssignment \n11 - ChooseCall");
            
            int input;
            int.TryParse(Console.ReadLine(), out input);
            inp = (CallMenuEn)(input);
            switch (inp)
            {
                case CallMenuEn.EXIT: break;
                case CallMenuEn.GetCallQuantitiesByStatus:
                    int callStatusAsInt = 0;   // variable to store the integer representation of the call status
                    foreach (int i in s_bl.Call.GetCallQuantitiesByStatus())
                    {
                        Console.WriteLine($"{(BO.CallStatus)callStatusAsInt}: {i}");
                        ++callStatusAsInt;
                    }
                    break;
                case CallMenuEn.GetFilteredAndSortedCalls:
                    filterBy = getEnumCallInList("filter");
                    if (filterBy == null)   // If user chose not to filter
                        filterValue = null;
                    else
                    {                        
                        filterValue = RequestAndValidateCallInListFieldValue(filterBy);
                    }
                    BO.CallInListField? sortBy = getEnumCallInList("sort");
                    IEnumerable<BO.CallInList> listCalls = s_bl.Call.GetFilteredAndSortedCalls(filterBy, filterValue, sortBy);
                    foreach(BO.CallInList callL in listCalls)
                    {
                        Console.WriteLine(callL);
                    }
                    break;
                case CallMenuEn.GetCallDetails:
                    Console.WriteLine("Enter Call id");
                    int.TryParse(Console.ReadLine(), out input);
                    Console.WriteLine(s_bl.Call.GetCallDetails(input));
                    break;
                case CallMenuEn.UpdateCall:
                    call = getNewBoCall();
                    s_bl.Call.UpdateCall(call);
                    break;
                case CallMenuEn.DeleteCall:
                    Console.WriteLine("Enter Call id");
                    int.TryParse(Console.ReadLine(), out input);
                    s_bl.Call.DeleteCall(input);
                    break;
                case CallMenuEn.AddCall:
                    call = getNewBoCall();
                    s_bl.Call.AddCall(call);
                    break;
                case CallMenuEn.GetClosedCallsHandledByVolunteer:
                    Console.WriteLine("Enter Volunteer id: ");
                    int.TryParse(Console.ReadLine(), out volunteerId);
                    Console.WriteLine("Enter the call type:");
                    Console.WriteLine("0: TireChange");
                    Console.WriteLine("1: JumpStart");
                    Console.WriteLine("2: FluidRefill");
                    Console.WriteLine("3: LightFix");
                    Console.WriteLine("4: LostKey");
                    if (!int.TryParse(Console.ReadLine(), out num))
                        throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");

                    if (!Enum.IsDefined(typeof(BO.CallType), num))
                        throw new BlInvalidInputException("Invalid input. Please enter a number corresponding to a valid call type.");

                    cType = (BO.CallType)num;

                    Console.WriteLine("Selct A field to filter by:\n0 - Id,\n1 -  CType\n2 - FullAddress\n3 - Opening\n4 - EnterTime\n5 - EndTime\n6 - EType");
                    // Validate that the input is an integer
                    if (!int.TryParse(Console.ReadLine(), out num))
                        throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");

                    // Check if the integer corresponds to a valid enum value
                    if (!Enum.IsDefined(typeof(BO.ClosedCallInListField), num))
                        throw new BlInvalidInputException("Invalid input. Please enter a number corresponding to a valid call type.");

                    IEnumerable<BO.ClosedCallInList> closedList = s_bl.Call.GetClosedCallsHandledByVolunteer(volunteerId, cType, (BO.ClosedCallInListField?)num);
                    foreach (BO.ClosedCallInList closed in closedList){
                        Console.WriteLine(closed);
                    }
                    break;
                case CallMenuEn.GetOpenCallsOfVolunteer:
                    Console.WriteLine("Enter Volunteer id: ");
                    int.TryParse(Console.ReadLine(), out volunteerId);

                    Console.WriteLine("Enter the call type:");
                    Console.WriteLine("0: TireChange");
                    Console.WriteLine("1: JumpStart");
                    Console.WriteLine("2: FluidRefill");
                    Console.WriteLine("3: LightFix");
                    Console.WriteLine("4: LostKey");
                    str = Console.ReadLine();
                    
                    if (!int.TryParse(str, out num))
                        throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");

                    if (!Enum.IsDefined(typeof(BO.CallType), num))
                        throw new BlInvalidInputException("Invalid input. Please enter a number corresponding to a valid call type.");

                    cType = (BO.CallType)num;

                    Console.WriteLine("Select a field to filter by:");
                    Console.WriteLine("0: Id");
                    Console.WriteLine("1: CType");
                    Console.WriteLine("2: Description");
                    Console.WriteLine("3: FullAddress");
                    Console.WriteLine("4: Opening");
                    Console.WriteLine("5: MaxTime");
                    Console.WriteLine("6: Distance");

                    str = Console.ReadLine();

                    // Validate that the input is a valid integer
                    if (!int.TryParse(str, out num))
                        throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");

                    // Validate that the integer corresponds to a valid enum value
                    if (!Enum.IsDefined(typeof(OpenCallInListField), num))
                        throw new BlInvalidInputException("Invalid input. Please enter a number corresponding to a valid field.");

                    // Cast the validated integer to the enum
                    OpenCallInListField selectedField = (OpenCallInListField)num;

                    IEnumerable<BO.OpenCallInList> openList = s_bl.Call.GetOpenCallsOfVolunteer(volunteerId, cType, selectedField);
                    foreach (BO.OpenCallInList closed in openList)
                    {
                        Console.WriteLine(closed);
                    }
                    break;
                case CallMenuEn.EndCall:
                    Console.WriteLine("Please enter the volunteer ID: ");
                    int.TryParse(Console.ReadLine(), out volunteerId);
                    Console.WriteLine("Please enter the assignment ID: ");
                    int.TryParse(Console.ReadLine(), out assignmentId);
                    s_bl.Call.EndCall(volunteerId, assignmentId);
                    break;
                case CallMenuEn.CancelAssignment:
                    Console.WriteLine("Please enter the requestor ID: ");
                    int.TryParse(Console.ReadLine(), out requestorId);
                    Console.WriteLine("Please enter the assignment ID: ");
                    int.TryParse(Console.ReadLine(), out assignmentId);
                    s_bl.Call.CancelAssignment(requestorId, assignmentId);
                    break;
                case CallMenuEn.ChooseCall:
                    Console.WriteLine("Please enter the volunteer ID: ");
                    int.TryParse(Console.ReadLine(), out volunteerId);
                    Console.WriteLine("Please enter the call ID: ");
                    int.TryParse(Console.ReadLine(), out callId);
                    s_bl.Call.ChooseCall(volunteerId, callId);
                    break;
            }
        }
        while (inp != CallMenuEn.EXIT);
    }
    static private BO.Volunteer getNewBoVolunteer()
    {
        int num, id, distance;
        string str, fullName, phoneNumber, email, FullAddress, distance_str;
        bool active = false;
        BO.Role vRole;
        BO.CallInProgress? callInProgress;

        Console.WriteLine("enter id:");
        str = Console.ReadLine();
        //check
        if (!int.TryParse(str, out id))
            throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");

        Console.WriteLine("enter full name:");
        fullName = Console.ReadLine();

        Console.WriteLine("enter phone number:");
        phoneNumber = Console.ReadLine();


        Console.WriteLine("enter email:");
        email = Console.ReadLine();

        Console.WriteLine("enter full adress:");
        FullAddress = Console.ReadLine();

        Console.WriteLine("if active enter 1, if not enter 0:");
        str = Console.ReadLine();

        if (!int.TryParse(str, out num))
            throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");
        if (num == 0)
            active = false;
        else if (num == 1)
            active = true;

        Console.WriteLine("Enter the distance: ");
        distance_str = Console.ReadLine();
        //check
        if (!int.TryParse(distance_str, out distance))
            throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");

        Console.WriteLine("if manager enter 1, if volunteer enter 0:");
        str = Console.ReadLine();
        vRole = BO.Role.Volunteer;
        if (!int.TryParse(str, out num))
            throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");
        if (num == 0)
            vRole = BO.Role.Volunteer;
        else if (num == 1)
            vRole = BO.Role.Manager;

        Console.WriteLine("Enter the distance type:");
        Console.WriteLine("0: Air");
        Console.WriteLine("1: Walking");
        Console.WriteLine("2: Driving");

        str = Console.ReadLine();

        BO.DistanceType vDisType = BO.DistanceType.Air;
        if (!int.TryParse(str, out num))
            throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");

        if (!Enum.IsDefined(typeof(BO.DistanceType), num))
            throw new BlInvalidInputException("Invalid input. Please enter a number corresponding to a valid distance type.");

        vDisType = (BO.DistanceType)num;

        return new BO.Volunteer()
        {
            Id = id,
            FullName = fullName,
            PhoneNumber = phoneNumber,
            Email = email,
            FullAddress = FullAddress,
            Latitude = null,    // for now it is null it will be calculated in another function based on the address
            Longitude = null,   // for now it is null it will be calculated in another function based on the address
            Active = active,
            Distance = distance,
            VRole = vRole,
            VDisType = vDisType,
            TotalCallsHandled = 0,
            TotalCallsCancelled = 0,
            TotalCallsExpired = 0,
            CallInProgress = null
        };
    }

    static private BO.Call getNewBoCall()
    {
        string str;
        int num;
        string format = "MM-dd-yyyy HH:mm:ss";

        Console.WriteLine("Please enter the ID of the call: ");
        if (!int.TryParse(Console.ReadLine(), out int callId))
            throw new BlInvalidInputException("Invalid input. id must be a number.");

        Console.WriteLine("enter description of the call:");
        string description = Console.ReadLine();

        Console.WriteLine("enter adress of the call:");
        string adress = Console.ReadLine();

        Console.WriteLine("Enter the call type:");
        Console.WriteLine("0: TireChange");
        Console.WriteLine("1: JumpStart");
        Console.WriteLine("2: FluidRefill");
        Console.WriteLine("3: LightFix");
        Console.WriteLine("4: LostKey");

        str = Console.ReadLine();

        BO.CallType cType;
        if (!int.TryParse(str, out num))
            throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");

        if (!Enum.IsDefined(typeof(BO.CallType), num))
            throw new BlInvalidInputException("Invalid input. Please enter a number corresponding to a valid call type.");

        cType = (BO.CallType)num;

        // Example usage:
        Console.WriteLine($"Selected call type: {cType}");



        Console.WriteLine($"Please enter the date and time when the call open in the format {format}:");

        // Read the input from the user
        str = Console.ReadLine();

        DateTime opening;
        DateTime.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out opening);

        // Prompt the user with the specified format
        Console.WriteLine($"Please enter the date and time when the call end in the format {format}:");

        // Read the input from the user
        str = Console.ReadLine();

        DateTime maxTime;
        DateTime.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out maxTime);


        Console.WriteLine("Enter the call status:");
        Console.WriteLine("0: Open");
        Console.WriteLine("1: InProgress");
        Console.WriteLine("2: Closed");
        Console.WriteLine("3: Expired");
        Console.WriteLine("4: OpenInRisk");
        Console.WriteLine("5: InProgressInRisk");

        str = Console.ReadLine();

        BO.CallStatus status;
        if (!int.TryParse(str, out num))
            throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");

        if (!Enum.IsDefined(typeof(BO.CallStatus), num))
            throw new BlInvalidInputException("Invalid input. Please enter a number corresponding to a valid call status.");

        status = (BO.CallStatus)num;

        // Example usage:
        Console.WriteLine($"Selected call status: {status}");


        return new BO.Call()
        {
            Id = callId,
            Description = description,
            FullAddress = adress,
            Latitude = 0,    // for now it is 0 it will be calculated in another function based on the address
            Longitude = 0,   // for now it is 0 it will be calculated in another function based on the address
            Opening = opening,
            CType = cType,
            MaxTime = maxTime,
            Status = status,
            CallAssignInList = null
        };
    }

    static private TimeSpan getNewRisk()
    {
        Console.WriteLine("Enter the risk range in the format HH:mm:ss (hours, minutes, and seconds):");
        string riskRangeInput = Console.ReadLine();

        if (!TimeSpan.TryParse(riskRangeInput, out TimeSpan riskRange))
            throw new BlInvalidInputException("Invalid input. Please enter a valid TimeSpan in the format HH:mm:ss.");

        if (riskRange <= TimeSpan.Zero)
            throw new BlInvalidInputException("Risk range must be greater than zero.");

        return riskRange;
    }
    static private BO.CallInListField? getEnumCallInList(string filterOrSort)
    {
        int inp = 0;
        if (filterOrSort != "filter" && filterOrSort != "sort")
            throw new BlInvalidInputException("Invalid input. Please enter either 'filter' or 'sort'.");
        
        Console.WriteLine($"Whould you like to {filterOrSort}? 0 - No, 1 - Yes ");
        if (filterOrSort == "sort")
            Console.WriteLine("(If not will calls will be sorted by Id)");

        // Validate that the input is an integer
        if (!int.TryParse(Console.ReadLine(), out inp))
            throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");


        if (inp == 1)
        {
            Console.WriteLine("By which? \n0 - Id\n1 - CallId\n2 - CType\n3 - Opening\n4 - TimeLeft\n5 - LastVolunteer\n6 - HandlingTime\n7 - Status\n8 - TotalAssignments");

            if (!int.TryParse(Console.ReadLine(), out inp))
                throw new BlInvalidInputException("Invalid input. Please enter a valid integer.");

            if (!Enum.IsDefined(typeof(BO.CallInListField), inp))
                throw new BlInvalidInputException("Invalid input. Please enter a number corresponding to a valid field.");

            if (inp >= 0 && inp <= 8)
                return (BO.CallInListField)(inp);
        }

        return null;
    }
    public static object RequestAndValidateCallInListFieldValue(CallInListField? field)
    {
        object? fieldValue = null; // Initialize fieldValue to store the result

        switch (field)
        {
            // Case for integer fields like Id, CallId, and TotalAssignments
            case CallInListField.Id:
            case CallInListField.CallId:
            case CallInListField.TotalAssignments:
                Console.WriteLine($"Please enter a valid {field}:");

                // Define a variable to hold the parsed integer value
                int intValue;
                if (!int.TryParse(Console.ReadLine(), out intValue))
                    throw new BlInvalidInputException($"{field} must be a number.");

                // Assign the parsed integer to fieldValue
                fieldValue = intValue;
                break;

            // Case for the CallType enum
            case CallInListField.CType:
                Console.WriteLine("Please select a CallType:");

                // Display available enum values for CallType
                foreach (int i in Enum.GetValues(typeof(BO.CallType)))
                {
                    Console.WriteLine($"{i}: {Enum.GetName(typeof(BO.CallType), i)}");
                }

                // Parse user input as an integer, ensure it's a valid CallType
                int callTypeChoice;
                if (!int.TryParse(Console.ReadLine(), out callTypeChoice) || !Enum.IsDefined(typeof(BO.CallType), callTypeChoice))
                    throw new BlInvalidInputException("Invalid input. Please choose a valid CallType.");

                // Assign the valid CallType to fieldValue
                fieldValue = (BO.CallType)callTypeChoice;
                break;

            // Case for the CallStatus enum
            case CallInListField.Status:
                Console.WriteLine("Please select a CallStatus:");

                // Display available enum values for CallStatus
                foreach (int i in Enum.GetValues(typeof(CallStatus)))
                {
                    Console.WriteLine($"{i}: {Enum.GetName(typeof(CallStatus), i)}");
                }

                // Parse user input as an integer, ensure it's a valid CallStatus
                int statusChoice;
                if (!int.TryParse(Console.ReadLine(), out statusChoice) || !Enum.IsDefined(typeof(CallStatus), statusChoice))
                    throw new BlInvalidInputException("Invalid input. Please choose a valid CallStatus.");

                // Assign the valid CallStatus to fieldValue
                fieldValue = (CallStatus)statusChoice;
                break;

            // Case for DateTime fields like Opening
            case CallInListField.Opening:
                Console.WriteLine("Please enter a valid opening date (format: yyyy-MM-dd):");

                // Try parsing the input as a DateTime
                DateTime openingDate;
                if (!DateTime.TryParse(Console.ReadLine(), out openingDate))
                    throw new BlInvalidInputException("Invalid input. Opening must be a valid date.");

                // Assign the DateTime value to fieldValue
                fieldValue = openingDate;
                break;

            // Case for TimeSpan fields like TimeLeft and HandlingTime
            case CallInListField.TimeLeft:
            case CallInListField.HandlingTime:
                Console.WriteLine($"Please enter a valid {field} (format: hh:mm:ss):");

                // Try parsing the input as a TimeSpan
                TimeSpan timeValue;
                if (!TimeSpan.TryParse(Console.ReadLine(), out timeValue))
                    throw new BlInvalidInputException($"{field} must be a valid time span.");

                // Assign the TimeSpan value to fieldValue
                fieldValue = timeValue;
                break;

            // Case for string fields like LastVolunteer
            case CallInListField.LastVolunteer:
                Console.WriteLine($"Please enter a valid {field}:");

                // Check if the input is not empty or null
                string stringValue = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(stringValue))
                    throw new BlInvalidInputException($"{field} cannot be empty.");

                // Assign the string value to fieldValue
                fieldValue = stringValue;
                break;

            // If the field is not recognized
            default:
                throw new BlInvalidInputException("Unknown field.");
        }

        return fieldValue; // Return the final value (casted to object)
    }
    static void Main(string[] args)
    {
        try
        {
            MainMenu inp;
            do
            {
                Console.WriteLine("0 - EXIT \n1 - Admin Menu\n2 - Call Menu\n3 - Volunteer Menu ");
                int input;
                int.TryParse(Console.ReadLine(), out input);
                inp = (MainMenu)(input);
                switch (inp)
                {
                    case MainMenu.EXIT: break;
                    case MainMenu.VOLUNTEER:
                        VolunteerMenu();
                        break;
                    case MainMenu.ADMIN:
                        AdminMenu();
                        break;
                    case MainMenu.CALL:
                        CallMenu();
                        break;
                    default: break;
                }

            }
            while (inp != MainMenu.EXIT);

        }
        catch (Exception ex)
        {
            // Retrieve the exception message
            Console.WriteLine("An error occurred:");
            Console.WriteLine($"Message: {ex.Message}");

            // Retrieve the inner exception message if it exists
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }
    }
}