namespace DalTest;
using Dal;
using DalApi;
using DO;
using System;
using System.Data;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

internal class Program
{
    //static readonly IDal s_dal = new DalList(); //stage 2
    //static readonly IDal s_dal = new DalXml(); //stage 3
    static readonly IDal s_dal = Factory.Get; //stage 4
    enum mainMenu { EXIT, VolunteerMenu, CallMenu, AssignmentMenu, Reset, ShowAll, ConfigMenu, resetDataConfig };
    enum entityMenu { EXIT, Create, Read, ReadAll, Update, Delete, DeleteAll};
    enum configMenu { EXIT, Add1Min, Add1Hour, Add1Day, Add1Month, Add1Year, ShowNow, SetNew, printConfig, Reset};
    enum whichToPrint { CallId, AssignmentId, AssignmentCallId };
    /// <summary>
    /// Showes the main menu
    /// </summary>
    public static void mainMenuDis()
    {
        mainMenu inp;
        do
        {
            Console.WriteLine("0 - EXIT \n1 - VolunteerMenu\n2 - CallMenu\n3 - AssignmentMenu\n4 - Reset\n5 - ShowAll\n6 - ConfigMenu\n7 - resetDataConfig");
            int input;
            int.TryParse(Console.ReadLine(), out input);
            inp = (mainMenu)(input);
            switch (inp) { 
                case mainMenu.EXIT: break;
                case mainMenu.VolunteerMenu:
                    VolunteerMenuDis();
                    break;
                case mainMenu.CallMenu:
                    CallMenuDis();
                    break;
                case mainMenu.AssignmentMenu:
                    AssignmentMenuDis();
                    break;
                case mainMenu.Reset:
                    //Initialization.Do(s_dal); //stage 2
                    Initialization.Do(); //stage 4
                    break;
                case mainMenu.ShowAll:
                    Console.WriteLine("All the volunteers: ");
                    foreach(Volunteer vol in s_dal.Volunteer!.ReadAll())
                    {
                        Console.WriteLine(vol);
                    }
                    Console.WriteLine("All the calls: ");
                    foreach (Call cl in s_dal.Call!.ReadAll())
                    {
                        Console.WriteLine(cl);
                    }
                    Console.WriteLine("All the assignment: ");
                    foreach (Assignment asm in s_dal.Assignment!.ReadAll())
                    {
                        Console.WriteLine(asm);
                    }
                    break;
                case mainMenu.ConfigMenu:
                    ConfigMenuDis();
                    break;
                case mainMenu.resetDataConfig:
                    s_dal.Volunteer.DeleteAll();
                    s_dal.Call.DeleteAll();
                    s_dal.Assignment.DeleteAll();
                    s_dal.Config.Reset(); 
                    break;
                default: break;
            }

        }
        while (inp != mainMenu.EXIT);
    }

    /// <summary>
    /// Manges all the params for Volunteer
    /// </summary>
    /// <returns>
    /// the new Volunteer with the user commands
    /// </returns>
    public static Volunteer inputVolunteer()
    {
        int num, id, distance;
        string str, fullName, phoneNumber, email, FullAddress, distance_str;
        double lot, lon;
        bool active = false;
        Role vRole;

        Console.WriteLine("enter id:");
        str = Console.ReadLine();
        //check
        if (!int.TryParse(str, out id))
            throw new DalInvalidInputException("Invalid input. Please enter a valid integer.");

        Console.WriteLine("enter full name:");
        fullName = Console.ReadLine();

        Console.WriteLine("enter phone number:");
        phoneNumber = Console.ReadLine();
        

        Console.WriteLine("enter email:");
        email = Console.ReadLine();

        Console.WriteLine("enter full adress:");
        FullAddress = Console.ReadLine();

        Console.WriteLine("enter the latitude:");
        str = Console.ReadLine();
        double.TryParse(str, out lot);

        Console.WriteLine("enter the Longitude:");
        str = Console.ReadLine();
        double.TryParse(str, out lon);

        Console.WriteLine("if active enter 1, if not enter 0:");
        str = Console.ReadLine();
        
        if (!int.TryParse(str, out num))
            throw new DalInvalidInputException("Invalid input. Please enter a valid integer.");
        if (num == 0)
            active = false;
        else if (num == 1)
            active = true;

        Console.WriteLine("Enter the distance: ");
        distance_str = Console.ReadLine();
        //check
        if (!int.TryParse(distance_str, out distance))
            throw new DalInvalidInputException("Invalid input. Please enter a valid integer.");

        Console.WriteLine("if manager enter 1, if volunteer enter 0:");
        str = Console.ReadLine();
        vRole = Role.Volunteer;
        if (!int.TryParse(str, out num))
            throw new DalInvalidInputException("Invalid input. Please enter a valid integer.");
        if (num == 0)
            vRole = Role.Manager;
        else if (num == 1)
            vRole = Role.Volunteer;

        Console.WriteLine("if distance is by air enter 0, if by walking enter 1, if by driving enter 2:");
        str = Console.ReadLine();
        DistanceType vDisType = DistanceType.Air;
        if (!int.TryParse(str, out num))
            throw new DalInvalidInputException("Invalid input. Please enter a valid integer.");
        if (num == 0)
            vDisType = DistanceType.Air;
        else if (num == 1)
            vDisType = DistanceType.Walking;
        else if (num == 2)
            vDisType = DistanceType.Driving;

        return new ()
        {
            Id = id,
            FullName = fullName,
            PhoneNumber = phoneNumber,
            Email = email,
            FullAddress = FullAddress,
            Latitude = lot,
            Longitude = lon,
            Active = active,
            Distance = distance,
            VRole = vRole,
            VDisType = vDisType
        };


    }
    
    /// <summary>
    /// Showes the Volunteer menu
    /// </summary>
    public static void VolunteerMenuDis()
    {
        entityMenu inp;
        do
        {
            Console.WriteLine("0 - EXIT\n1 - Create\n2 - Read\n3 - ReadAll\n4 - Update\n5 - Delete\n6 - DeleteAll");
            int input;
            int.TryParse(Console.ReadLine(), out input);
            inp = (entityMenu)(input);
            
            switch (inp)
            {
                case entityMenu.EXIT: break;
                case entityMenu.Create:


                    Volunteer vol = inputVolunteer();
                    s_dal.Volunteer!.Create(vol);
                    break;
                case entityMenu.Read:
                    Console.WriteLine("Enter Id to check: ");
                    int id = Console.Read();
                    vol = s_dal.Volunteer!.Read(item=>item.Id == id);
                    Console.WriteLine(vol);
                    break;
                case entityMenu.ReadAll:
                    foreach(Volunteer volu in s_dal.Volunteer!.ReadAll())
                    {
                        Console.WriteLine(volu);
                    }
                    break;
                case entityMenu.Update:
                    vol = inputVolunteer();
                    s_dal.Volunteer!.Update(vol);
                    break;
                case entityMenu.Delete:
                    Console.WriteLine("Enter Id to delete: ");
                    id = Console.Read();
                    s_dal.Volunteer!.Delete(id);
                    break;
                case entityMenu.DeleteAll:
                    s_dal.Volunteer!.DeleteAll();
                    break;
                default: break;
            }

        }
        while (inp != entityMenu.EXIT);
    }


    /// <summary>
    /// Manges all the params for Call
    /// </summary>
    /// <returns>
    /// the new Call with the user commands
    /// </returns>
    public static Call inputCall()
    {
        string str;
        int num;
        double lot, lon;
        string format = "MM-dd-yyyy HH:mm:ss";


        Console.WriteLine("enter description of the call:");
        string Description = Console.ReadLine();

        Console.WriteLine("enter adress of the call:");
        string adress = Console.ReadLine();

        Console.WriteLine("enter the latitude:");
        str = Console.ReadLine();
        double.TryParse(str, out lot);

        Console.WriteLine("enter the Longitude:");
        str = Console.ReadLine();
        double.TryParse(str, out lon);

        Console.WriteLine("for TireChange enter 0, for JumpStart enter 1, for FluidRefill enter 2, for LightFix enter 3, for LostKey enter 4:");
        str = Console.ReadLine();
        CallType cType = CallType.TireChange;
        if (!int.TryParse(str, out num))
            throw new DalInvalidInputException("Invalid input. Please enter a valid integer.");
        if (num == 0)
            cType = CallType.TireChange;
        else if (num == 1)
            cType = CallType.JumpStart;
        else if (num == 2)
            cType = CallType.FluidRefill;
        else if (num == 3)
            cType = CallType.LightFix;
        else if (num == 4)
            cType = CallType.LostKey;


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

        return new Call()
        {
            Description = Description,
            FullAddress = adress,
            Latitude = lot,
            Longitude = lon,
            Opening = opening,
            CType = cType,
            MaxTime = maxTime
        };
    }
    /// <summary>
    /// Showes the Call menu
    /// </summary>
    public static void CallMenuDis()
    {
        entityMenu inp;
        do
        {
            Console.WriteLine("0 - EXIT\n1 - Create\n2 - Read\n3 - ReadAll\n4 - Update\n5 - Delete\n6 - DeleteAll");
            int input;
            int.TryParse(Console.ReadLine(), out input);
            inp = (entityMenu)(input);
            switch (inp)
            {
                case entityMenu.EXIT: break;
                case entityMenu.Create:


                    Call cl = inputCall();
                    s_dal.Call!.Create(cl);
                    break;
                case entityMenu.Read:
                    Console.WriteLine("Enter Id to check: ");
                    int id = Console.Read();
                    cl = s_dal.Call!.Read(item => item.Id == id);
                    Console.WriteLine(cl);
                    break;
                case entityMenu.ReadAll:
                    foreach (Call cll in s_dal.Call!.ReadAll())
                    {
                        Console.WriteLine(cll);
                    }
                    break;
                case entityMenu.Update:
                    cl = inputCall();
                    s_dal.Call!.Update(cl);
                    break;
                case entityMenu.Delete:
                    Console.WriteLine("Enter Id to delete: ");
                    id = Console.Read();
                    s_dal.Call!.Delete(id);
                    break;
                case entityMenu.DeleteAll:
                    s_dal.Call!.DeleteAll();
                    break;
                default: break;
            }

        }
        while (inp != entityMenu.EXIT);
    }

    /// <summary>
    /// Manges all the params for Assignment
    /// </summary>
    /// <returns>
    /// the new Assignment with the user commands
    /// </returns>
    public static Assignment inputAssignment()
    {
        string str;
        int num;
        string format = "MM-dd-yyyy HH:mm:ss";

        Console.WriteLine("enter id of volunteer:");
        str = Console.ReadLine();
        int volunteerId;
        if (!int.TryParse(str, out volunteerId))
            throw new DalInvalidInputException("Invalid input. Please enter a valid integer.");


        Console.WriteLine($"Please enter the date and time when the assignment open in the format {format}:");

        // Read the input from the user
        str = Console.ReadLine();

        DateTime enterTime;
        DateTime.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out enterTime);


        Console.WriteLine($"Please enter the date and time when the assignment end in the format {format}:");

        // Read the input from the user
        str = Console.ReadLine();

        DateTime endTime;
        DateTime.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out endTime);


        Console.WriteLine("if assignment Solved enter 0, if canceled by caller enter 1, if Canceled by manager enter 2, if Expired enter 3: ");
        str = Console.ReadLine();
        EndingType? eType = EndingType.Expired;
        if (!int.TryParse(str, out num))
            throw new DalInvalidInputException("Invalid input. Please enter a valid integer.");
        if (num == 0)
            eType = EndingType.Solved;
        else if (num == 1)
            eType = EndingType.SelfCanceld;
        else if (num == 2)
            eType = EndingType.CanceledByManager;
        else if (num == 3)
            eType = EndingType.Expired;


        return new()
        {
            VolunteerId = volunteerId,
            EnterTime = enterTime,
            EType = eType,
            EndTime = endTime
        };

    }
    /// <summary>
    /// Showes the Assignment menu
    /// </summary>
    public static void AssignmentMenuDis()
    {
        entityMenu inp;
        do
        {
            Console.WriteLine("0 - EXIT\n1 - Create\n2 - Read\n3 - ReadAll\n4 - Update\n5 - Delete\n6 - DeleteAll");
            int input;
            int.TryParse(Console.ReadLine(), out input);
            inp = (entityMenu)(input);
            switch (inp)
            {
                case entityMenu.EXIT: break;
                case entityMenu.Create:


                    Assignment asm = inputAssignment();
                    s_dal.Assignment!.Create(asm);
                    break;
                case entityMenu.Read:
                    Console.WriteLine("Enter Id to check: ");
                    int id = Console.Read();
                    asm = s_dal.Assignment!.Read(item => item.Id == id);
                    Console.WriteLine(asm);
                    break;
                case entityMenu.ReadAll:
                    foreach (Assignment assm in s_dal.Assignment!.ReadAll())
                    {
                        Console.WriteLine(assm);
                    }
                    break;
                case entityMenu.Update:
                    asm = inputAssignment();
                    s_dal.Assignment!.Update(asm);
                    break;
                case entityMenu.Delete:
                    Console.WriteLine("Enter Id to delete: ");
                    id = Console.Read();
                    s_dal.Assignment!.Delete(id);
                    break;
                case entityMenu.DeleteAll:
                    s_dal.Assignment!.DeleteAll();
                    break;
                default: break;
            }

        }
        while (inp != entityMenu.EXIT);
    }

    public static void printConfigHelp()
    {
        whichToPrint inp;
        int input;
        Console.WriteLine("0 - Call Id\n1 - Assignment Id\n2 - Assignment Call Id");
        int.TryParse(Console.ReadLine(), out input);
        inp = (whichToPrint)input;
        switch (inp)
        {
            case whichToPrint.CallId:
                //Console.WriteLine();
                break;
            case whichToPrint.AssignmentId:
                break;
            case whichToPrint.AssignmentCallId:
                break;
        }
    }
    /// <summary>
    /// Showes the Config menu
    /// </summary>
    public static void ConfigMenuDis()
    {
        configMenu inp;
        do
        {
            Console.WriteLine("0 - EXIT\n1 - Add 1 Min\n2 -  Add 1 Hour\n3 - Add 1 Day\n4 - Add 1 Month\n5 - Add 1 Year\n6 - Show Now\n7 - Set New\n8 - print Config\n9 - Reset");
            
            int input;
            int.TryParse(Console.ReadLine(), out input);
            inp = (configMenu)input;
            switch (inp)
            {
                case configMenu.EXIT: break;
                case configMenu.Add1Min:
                    s_dal.Config.Clock = s_dal.Config.Clock.AddMinutes(1);
                    break;
                case configMenu.Add1Hour:
                    s_dal.Config.Clock = s_dal.Config.Clock.AddHours(1);
                    break;
                case configMenu.Add1Day:
                    s_dal.Config.Clock = s_dal.Config.Clock.AddDays(1);
                    break;
                case configMenu.Add1Month:
                    s_dal.Config.Clock = s_dal.Config.Clock.AddMonths(1);
                    break;
                case configMenu.Add1Year:
                    s_dal.Config.Clock = s_dal.Config.Clock.AddYears(1);
                    break;
                case configMenu.ShowNow:
                    Console.WriteLine(s_dal.Config.Clock);
                    break;
                case configMenu.SetNew:
                    
                    break;
                case configMenu.printConfig:
                    printConfigHelp();
                    break;
                case configMenu.Reset:
                    s_dal.Volunteer.DeleteAll();
                    s_dal.Call.DeleteAll();
                    s_dal.Assignment.DeleteAll();
                    s_dal.Config.Reset();
                    break;
                default: break;
            }

        }
        while (inp != configMenu.EXIT);

    }
    
    public static void Main()
    {
        try
        {
            mainMenuDis();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

