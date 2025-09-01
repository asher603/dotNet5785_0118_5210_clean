namespace DalTest;
using DalApi;
using DO;
using Secrets;
using System.ComponentModel.Design;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Security.Cryptography;

public static class Initialization
{
    private static IDal? s_dal; //stage 2

    
    private static Random s_rand;   // declare a field of type Random to be used in the Do() function

    /// <summary>
    /// Calculates the complete Israeli ID number by adding a valid check digit.
    /// </summary>
    /// <param name="idWithoutCheckDigit">The Israeli ID number without the check digit (exactly 8 digits).</param>
    /// <returns>The complete ID with a valid check digit (9 digits).</returns>
    private static int AddCheckDigitID(int idWithoutCheckDigit)
    {
        int sum = 0; // Accumulates the sum of processed digits
        int multiplier = 1; // Starts with 1 and alternates with 2

        // Convert the ID to a string to process each digit from left to right
        string idString = idWithoutCheckDigit.ToString("D8"); // Ensures the ID has 8 digits

        // Process each digit from left to right
        foreach (char digitChar in idString)
        {
            int digit = digitChar - '0'; // Convert the character to an integer
            int product = digit * multiplier; // Multiply the digit by the multiplier

            // Add the product's digits to the sum
            sum += (product % 10) + (product / 10);

            // Alternate the multiplier between 1 and 2
            multiplier = (multiplier == 1) ? 2 : 1;
        }

        // Calculate the check digit
        int checkDigit = (10 - (sum % 10)) % 10;

        // Return the complete ID with the calculated check digit
        return idWithoutCheckDigit * 10 + checkDigit;
    }
    private static void createVolunteers()
    {
        const int MIN_ID = 20000000, MAX_ID = 40000000;
        const int MIN_DIS = 2, MAX_DIS = 150;

        string[] volunteerNames =
            {"Eli Amar", "Asher Abensour", "Harel Gabay", "Dina Klein", "Shira Israelof", "David Cohen", "Rachel Goldstein", "Levi Rosenberg", "Miriam Shapiro",
            "Shalom Katz", "Sofia Lieberman", "Ezra Ben-Ami", "Esther Greenberg", "Jacob Friedman", "Talia Weiss" };

        string[] VolunteerPhone = {
            "0501234567",
            "0522345678",
            "0533456789",
            "0544567890",
            "0555678901",
            "0566789012",
            "0577890123",
            "0588901234",
            "0599012345",
            "0501122334",
            "0512233445",
            "0523344556",
            "0534455667",
            "0545566778",
            "0556677889" };

        string[] volunteerEmails =
        {
            Secrets.VOLUNTEER_MAIL_ADDRESS_1,
            Secrets.VOLUNTEER_MAIL_ADDRESS_2,
            Secrets.VOLUNTEER_MAIL_ADDRESS_3,
            Secrets.VOLUNTEER_MAIL_ADDRESS_4,
            Secrets.VOLUNTEER_MAIL_ADDRESS_5,
            Secrets.VOLUNTEER_MAIL_ADDRESS_6,
            Secrets.VOLUNTEER_MAIL_ADDRESS_7,
            Secrets.VOLUNTEER_MAIL_ADDRESS_8,
            Secrets.VOLUNTEER_MAIL_ADDRESS_9,
            Secrets.VOLUNTEER_MAIL_ADDRESS_10,
            Secrets.VOLUNTEER_MAIL_ADDRESS_11,
            Secrets.VOLUNTEER_MAIL_ADDRESS_12,
            Secrets.VOLUNTEER_MAIL_ADDRESS_13,
            Secrets.VOLUNTEER_MAIL_ADDRESS_14,
            Secrets.VOLUNTEER_MAIL_ADDRESS_15
        };


        string[] volunteerAddresses =
        {
            "Hanassi 43, Haifa, Israel",
            "HaBonim 45, Hadera, Israel",
            "Herzl 25, Ramat Gan, Israel",
            "Emek Refaim 50, Jerusalem, Israel",
            "Emek Refaim 14, Jerusalem, Israel",
            "Dizengoff 250, Tel Aviv, Israel",
            "Hanassi 70, Haifa, Israel",
            "Herzel 60, Ramat Gan, Israel",
            "Ben Gurion 40, Herzliya, Israel",
            "Tchernichovsky 60, Jerusalem, Israel",
            "Ben Gurion 16, Herzliya, Israel",
            "HaBonim 50, Hadera, Israel",
            "Herzel 17, Ramat Gan, Israel",
            "Emek Refaim 60, Jerusalem, Israel",
            "Tversky 25, Haifa, Israel"
        };

        List<double> volunteerLatitudes = new List<double>
        {
            32.4407923, // Hanassi 43, Haifa, Israel
            32.4270293, // HaBonim 45, Hadera, Israel
            32.0809711, // Herzl 25, Ramat Gan, Israel
            31.7621583, // Emek Refaim 50, Jerusalem, Israel
            31.7656821, // Emek Refaim 14, Jerusalem, Israel
            32.090514, // Dizengoff 250, Tel Aviv, Israel
            32.5207254, // Hanassi 70, Haifa, Israel
            32.085125149999996, // Herzel 60, Ramat Gan, Israel
            32.1518147, // Ben Gurion 40, Herzliya, Israel
            31.7635774, // Tchernichovsky 60, Jerusalem, Israel
            32.1590057, // Ben Gurion 16, Herzliya, Israel
            32.4270293, // HaBonim 50, Hadera, Israel
            32.0809711, // Herzel 17, Ramat Gan, Israel
            31.7613779, // Emek Refaim 60, Jerusalem, Israel
            32.7732409 // Tversky 25, Haifa, Israel
        };

        List<double> volunteerLongitudes = new List<double>
        {
            34.9157156, // Hanassi 43, Haifa, Israel
            34.9533834, // HaBonim 45, Hadera, Israel
            34.8185976, // Herzl 25, Ramat Gan, Israel
            35.217664, // Emek Refaim 50, Jerusalem, Israel
            35.2215178, // Emek Refaim 14, Jerusalem, Israel
            34.775984, // Dizengoff 250, Tel Aviv, Israel
            34.9462061, // Hanassi 70, Haifa, Israel
            34.8154885, // Herzel 60, Ramat Gan, Israel
            34.8420361, // Ben Gurion 40, Herzliya, Israel
            35.2024448, // Tchernichovsky 60, Jerusalem, Israel
            34.842534, // Ben Gurion 16, Herzliya, Israel
            34.9533834, // HaBonim 50, Hadera, Israel
            34.8185976, // Herzel 17, Ramat Gan, Israel
            35.2166562, // Emek Refaim 60, Jerusalem, Israel
            34.9908045 // Tversky 25, Haifa, Israel
        };

        List<string> passwords = new List<string>
        {
            "123",
            "qwertyuiop",
            "letmein2025",
            "admin1234",
            "secureP@ss1",
            "12345abcde",
            "MyPassword!",
            "Test1234#",
            "Sunshine88",
            "LoveYou@2025",
            "M0nkey#123",
            "P@ssword2025",
            "Happy@Home1",
            "C0dingR0cks!",
            "Winter!2025"
        };

        for (int i = 0; i < volunteerNames.Length; ++i)
        {
            int id;
            bool activeV;
            int distanceV;
            DistanceType vD;
            Role vR;
            do
                id = AddCheckDigitID(s_rand.Next(MIN_ID, MAX_ID));
            while (s_dal!.Volunteer!.Read(item => id == item.Id) != null);
            activeV = s_rand.Next(0, 2) == 1 ? true : false;
            distanceV = s_rand.Next(MIN_DIS, MAX_DIS);
            vR = i % 4 == 0 ? Role.Manager : Role.Volunteer;

            s_dal.Volunteer!.Create(new() { Id = id, FullName = volunteerNames[i], PhoneNumber = VolunteerPhone[i], Email = volunteerEmails[i], 
                Password= GetHash(passwords[i]),FullAddress = volunteerAddresses[i], Active = activeV, Distance = distanceV, VRole = vR, VDisType = DistanceType.Air, Latitude = volunteerLatitudes[i], Longitude = volunteerLongitudes[i] });
        }
    }
    static string GetHash(string input)
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
    private static void createCalls()
    {
        // TireChange descriptions
        string[] tireChangeDescriptions = new string[]
        {
            "A stranded driver needs help changing a flat tire on their vehicle quickly.",
            "A car on the side of the road requires a tire swap due to a puncture.",
            "A volunteer is needed to assist with replacing a spare tire for a motorist.",
            "Someone is stuck in a remote area with a flat tire needing urgent attention.",
            "A driver needs guidance and help in installing a spare tire safely."
        };

        // JumpStart descriptions
        string[] jumpStartDescriptions = new string[]
        {
            "A driver’s car battery has died, and they need help jumpstarting it quickly.",
            "A vehicle won’t start due to a drained battery, and assistance is required.",
            "A volunteer is requested to help jumpstart a car stuck in a parking lot.",
            "Someone is stranded on a cold morning with a dead battery needing a jumpstart.",
            "A driver needs a jumpstart to get their vehicle running after battery failure."
        };

        // FluidRefill descriptions
        string[] fluidRefillDescriptions = new string[]
        {
            "A vehicle has run out of coolant, and the driver needs help refilling it.",
            "A stranded motorist requires assistance with refilling engine oil on-site.",
            "Someone's car washer fluid is empty, and they need help refilling it quickly.",
            "A driver is out of brake fluid and needs assistance from a volunteer nearby.",
            "A car’s power steering fluid is low, and the driver is requesting help."
        };

        // LightFix descriptions
        string[] lightFixDescriptions = new string[]
        {
            "A headlight has gone out, and a driver needs help replacing it safely.",
            "A volunteer is needed to assist in fixing a broken brake light on a car.",
            "A motorist is requesting help to replace a blown-out turn signal bulb.",
            "A driver needs assistance with reattaching a loose taillight assembly.",
            "A vehicle’s dashboard lights have failed, and troubleshooting is needed."
        };

        // LostKey descriptions
        string[] lostKeyDescriptions = new string[]
        {
            "A driver has misplaced their keys and needs help locating a spare nearby.",
            "A car owner is locked out and needs assistance retrieving their spare key.",
            "A volunteer is needed to help a driver contact locksmith services quickly.",
            "A motorist has lost their key fob and requires guidance to resolve the issue.",
            "Someone is stranded without car keys and needs support in their situation."
        };

        List<double> callLatitudes = new List<double>
        {
            32.068097, // Allenby 65, Tel Aviv, Israel
            32.08137, // Dizengoff 125, Tel Aviv, Israel
            32.57382015, // Hanassi 34, Haifa, Israel
            32.0875528, // Abba Hillel 22, Ramat Gan, Israel
            32.1633245, // Ben Gurion 20, Herzliya, Israel
            31.7674084, // Tchernichovsky 25, Jerusalem, Israel
            29.5653712, // HaDekel 15, Eilat, Israel
            32.4270293, // HaBonim 15, Hadera, Israel
            32.08107, // Herzl 10, Ramat Gan, Israel
            31.7637403, // Emek Refaim 33, Jerusalem, Israel
            31.26802375, // Arava 8, Beer Sheva, Israel
            31.7596765, // Derech Hevron 30, Jerusalem, Israel
            32.0632619, // Rothschild 15, Tel Aviv, Israel
            32.7732409, // Tversky 7, Haifa, Israel
            32.0761504, // Bograshov 44, Tel Aviv, Israel
            32.0742151, // Jabotinsky 36, Ramat Gan, Israel
            32.072739, // Allenby 30, Tel Aviv, Israel
            31.7686484, // Tchernichovsky 10, Jerusalem, Israel
            31.761521, // Derech Hevron 10, Jerusalem, Israel
            29.5653712, // HaDekel 25, Eilat, Israel
            31.7596765, // Derech Hevron 5, Jerusalem, Israel
            32.079249, // Dizengoff 100, Tel Aviv, Israel
            32.4270293, // HaBonim 25, Hadera, Israel
            32.0606592, // Rothschild 55, Tel Aviv, Israel
            32.083042, // Dizengoff 150, Tel Aviv, Israel
            31.7657534, // Tchernichovsky 35, Jerusalem, Israel
            32.16506795, // Ben Gurion 12, Herzliya, Israel
            32.4270293, // HaBonim 35, Hadera, Israel
            32.0623233, // Herzel 12, Tel Aviv, Israel
            32.062971399999995, // Rothschild 30, Tel Aviv, Israel
            32.085076, // Dizengoff 175, Tel Aviv, Israel
            32.7732409, // Tversky 15, Haifa, Israel
            32.075915, // Bograshov 50, Tel Aviv, Israel
            32.57196635, // Herzel 15, Haifa, Israel
            32.0907362, // Abba Hillel 40, Ramat Gan, Israel
            32.1590057, // Ben Gurion 30, Herzliya, Israel
            31.7656726, // Tchernichovsky 40, Jerusalem, Israel
            29.5653712, // HaDekel 10, Eilat, Israel
            32.6371881, // HaBonim 40, Hadera, Israel
            32.0809711, // Herzl 20, Ramat Gan, Israel
            31.7689108, // Tchernichovsky 5, Jerusalem, Israel
            32.0622106, // Rothschild 45, Tel Aviv, Israel
            32.088271, // Dizengoff 220, Tel Aviv, Israel
            32.0656376, // Rothschild 67, Tel Aviv, Israel
            32.7732409, // Tversky 18, Haifa, Israel
            32.075551, // Bograshov 60, Tel Aviv, Israel
            32.4407923, // Hanassi 60, Haifa, Israel
            32.08910275, // Abba Hillel 50, Ramat Gan, Israel
            32.16116155, // Ben Gurion 35, Herzliya, Israel
            31.7645839 // Tchernichovsky 50, Jerusalem, Israel
        };

        List<double> callLongitudes = new List<double>
        {
            34.771172, // Allenby 65, Tel Aviv, Israel
            34.77358, // Dizengoff 125, Tel Aviv, Israel
            34.94987867254768, // Hanassi 34, Haifa, Israel
            34.8064134, // Abba Hillel 22, Ramat Gan, Israel
            34.8423088242769, // Ben Gurion 20, Herzliya, Israel
            35.2062385, // Tchernichovsky 25, Jerusalem, Israel
            34.9482766, // HaDekel 15, Eilat, Israel
            34.9533834, // HaBonim 15, Hadera, Israel
            34.8188628, // Herzl 10, Ramat Gan, Israel
            35.2199878, // Emek Refaim 33, Jerusalem, Israel
            34.85221403251482, // Arava 8, Beer Sheva, Israel
            35.2238736, // Derech Hevron 30, Jerusalem, Israel
            34.7711996, // Rothschild 15, Tel Aviv, Israel
            34.9908045, // Tversky 7, Haifa, Israel
            34.7711342, // Bograshov 44, Tel Aviv, Israel
            34.8484311, // Jabotinsky 36, Ramat Gan, Israel
            34.768023, // Allenby 30, Tel Aviv, Israel
            35.2075682, // Tchernichovsky 10, Jerusalem, Israel
            35.2249463, // Derech Hevron 10, Jerusalem, Israel
            34.9482766, // HaDekel 25, Eilat, Israel
            35.2238736, // Derech Hevron 5, Jerusalem, Israel
            34.774114, // Dizengoff 100, Tel Aviv, Israel
            34.9533834, // HaBonim 25, Hadera, Israel
            34.8576864, // Rothschild 55, Tel Aviv, Israel
            34.774177, // Dizengoff 150, Tel Aviv, Israel
            35.2062771, // Tchernichovsky 35, Jerusalem, Israel
            34.842348555844154, // Ben Gurion 12, Herzliya, Israel
            34.9533834, // HaBonim 35, Hadera, Israel
            34.7699763, // Herzel 12, Tel Aviv, Israel
            34.77262264217134, // Rothschild 30, Tel Aviv, Israel
            34.774289, // Dizengoff 175, Tel Aviv, Israel
            34.9908045, // Tversky 15, Haifa, Israel
            34.771387, // Bograshov 50, Tel Aviv, Israel
            34.95196492455229, // Herzel 15, Haifa, Israel
            34.8119048, // Abba Hillel 40, Ramat Gan, Israel
            34.842534, // Ben Gurion 30, Herzliya, Israel
            35.2059289, // Tchernichovsky 40, Jerusalem, Israel
            34.9482766, // HaDekel 10, Eilat, Israel
            34.9327996, // HaBonim 40, Hadera, Israel
            34.8185976, // Herzl 20, Ramat Gan, Israel
            35.208692, // Tchernichovsky 5, Jerusalem, Israel
            34.8058007, // Rothschild 45, Tel Aviv, Israel
            34.775488, // Dizengoff 220, Tel Aviv, Israel
            34.7766423, // Rothschild 67, Tel Aviv, Israel
            34.9908045, // Tversky 18, Haifa, Israel
            34.772081, // Bograshov 60, Tel Aviv, Israel
            34.9157156, // Hanassi 60, Haifa, Israel
            34.809028999999995, // Abba Hillel 50, Ramat Gan, Israel
            34.8429057907241, // Ben Gurion 35, Herzliya, Israel
            35.2045088 // Tchernichovsky 50, Jerusalem, Israel
        };

        string[] callAddresses =
        {
            "Allenby 65, Tel Aviv, Israel",
            "Dizengoff 125, Tel Aviv, Israel",
            "Hanassi 34, Haifa, Israel",
            "Abba Hillel 22, Ramat Gan, Israel",
            "Ben Gurion 20, Herzliya, Israel",
            "Tchernichovsky 25, Jerusalem, Israel",
            "HaDekel 15, Eilat, Israel",
            "HaBonim 15, Hadera, Israel",
            "Herzl 10, Ramat Gan, Israel",
            "Emek Refaim 33, Jerusalem, Israel",
            "Arava 8, Beer Sheva, Israel",
            "Derech Hevron 30, Jerusalem, Israel",
            "Rothschild 15, Tel Aviv, Israel",
            "Tversky 7, Haifa, Israel",
            "Bograshov 44, Tel Aviv, Israel",
            "Jabotinsky 36, Ramat Gan, Israel",
            "Allenby 30, Tel Aviv, Israel",
            "Tchernichovsky 10, Jerusalem, Israel",
            "Derech Hevron 10, Jerusalem, Israel",
            "HaDekel 25, Eilat, Israel",
            "Derech Hevron 5, Jerusalem, Israel",
            "Dizengoff 100, Tel Aviv, Israel",
            "HaBonim 25, Hadera, Israel",
            "Rothschild 55, Tel Aviv, Israel",
            "Dizengoff 150, Tel Aviv, Israel",
            "Tchernichovsky 35, Jerusalem, Israel",
            "Ben Gurion 12, Herzliya, Israel",
            "HaBonim 35, Hadera, Israel",
            "Herzel 12, Tel Aviv, Israel",
            "Rothschild 30, Tel Aviv, Israel",
            "Dizengoff 175, Tel Aviv, Israel",
            "Tversky 15, Haifa, Israel",
            "Bograshov 50, Tel Aviv, Israel",
            "Herzel 15, Haifa, Israel",
            "Abba Hillel 40, Ramat Gan, Israel",
            "Ben Gurion 30, Herzliya, Israel",
            "Tchernichovsky 40, Jerusalem, Israel",
            "HaDekel 10, Eilat, Israel",
            "HaBonim 40, Hadera, Israel",
            "Herzl 20, Ramat Gan, Israel",
            "Tchernichovsky 5, Jerusalem, Israel",
            "Rothschild 45, Tel Aviv, Israel",
            "Dizengoff 220, Tel Aviv, Israel",
            "Rothschild 67, Tel Aviv, Israel",
            "Tversky 18, Haifa, Israel",
            "Bograshov 60, Tel Aviv, Israel",
            "Hanassi 60, Haifa, Israel",
            "Abba Hillel 50, Ramat Gan, Israel",
            "Ben Gurion 35, Herzliya, Israel",
            "Tchernichovsky 50, Jerusalem, Israel"
        };

        CallType ct;
        DateTime maxTime;
        int randNum;
        string description;

        for (int i = 0; i < 50; ++i)
        {
            DateTime opening = s_dal!.Config.Clock.AddSeconds(-s_rand.Next(1000000, 100000000));

            if (i < 5)
                maxTime = opening.AddSeconds(s_rand.Next(1, (int)(s_dal.Config.Clock - opening).TotalSeconds));
            else
                maxTime = s_dal.Config.Clock.AddSeconds(s_rand.Next(1, 1000000));

            // generate a "random" number between 0 and 4
            randNum = s_rand.Next(0, 5);

            ct = randNum switch
            {
                0 => CallType.TireChange,
                1 => CallType.JumpStart,
                2 => CallType.FluidRefill,
                3 => CallType.LightFix,
                4 => CallType.LostKey,
                _ => throw new ArgumentOutOfRangeException("Invalid randNum value")    // just to make sure, should never be reached
            };

            description = ct switch
            {
                CallType.TireChange => tireChangeDescriptions[s_rand.Next(0, 5)],
                CallType.JumpStart => jumpStartDescriptions[s_rand.Next(0, 5)],
                CallType.FluidRefill => fluidRefillDescriptions[s_rand.Next(0, 5)],
                CallType.LightFix => lightFixDescriptions[s_rand.Next(0, 5)],
                CallType.LostKey => lostKeyDescriptions[s_rand.Next(0, 5)],
                _ => throw new ArgumentOutOfRangeException("Invalid CallType value")    // just to make sure, should never be reached
            };

            s_dal.Call!.Create(new() { Description = description, FullAddress = callAddresses[i], Opening = opening, CType = ct, MaxTime = maxTime, Latitude = callLatitudes[i], Longitude = callLongitudes[i] });
        }

    }
    private static void createAssignments()
    {
        DateTime enterTime;
        EndingType? eType;
        DateTime? endTime;
        int randNum;
        DateTime timeAfterAllCallsOpend = s_dal!.Config.Clock.AddSeconds(-1000000); // a time after all calls opened
        enterTime = timeAfterAllCallsOpend;
        int totalNumOfAssignmentsToInitialize = 50;


        int VolunteersCount = s_dal.Volunteer.ReadAll().Count();
        int callsCount = s_dal.Call.ReadAll().Count();

        // the division by 2 here is to make sure we leave a deacent number of volunteers and calls available for the rest of the non active assignments after we reach the limit of active assignments, the exact number 2 doesn't have any special meaning it can be 3 or any other whole number greater than 1
        int activeAssignmentsLimit = Math.Min(VolunteersCount, callsCount) / 2;

        int numOfActiveAssignments = 0; // to track the number of active assignments

        for (int i = 0; i < totalNumOfAssignmentsToInitialize; ++i)
        {

            // Find a volunteer who is active and also not currently assigned to an active assignment
            IEnumerable<DO.Volunteer?> availableVolunteers = s_dal.Volunteer.ReadAll(v => s_dal.Assignment.Read(asm => asm.VolunteerId == v.Id && asm.EndTime == null) == null);

            // Find a call that is not currently assigned to an active assignment and was not assigned to an assignment that ended with EndingType.Solved or EndingType.Expired, meaning the call is still open
            IEnumerable<Call?> availableCalls = s_dal.Call.ReadAll(c =>
                s_dal.Assignment.Read(a => a.CallId == c.Id && (a.EndTime == null || a.EType == EndingType.Solved || a.EType == EndingType.Expired)) == null);

            DO.Volunteer? volunteer = availableVolunteers.ElementAt(s_rand.Next(0, availableVolunteers.Count()));

            DO.Call? call = availableCalls.ElementAt(s_rand.Next(0, availableCalls.Count()));

            if (volunteer == null || call == null)
            {
                // just to make sure, should never be reached because we limited the number of active assignments to 10 so there should always be an available volunteer and call
                throw new DataException("No available volunteer or call for assignment");
            }

            // here to make sure each assignment is later than the previous one 
            // we add at most 18000 seconds each time so that we wont pass the current time
            // (enterTime start 1000000 seconds before the current time so after adding at most 18000 seconds for each of the 50 assignments enterTime will advance at most 900000 seconds
            // meaning it will be at most 100000 seconds before the current time, that ensure we wont create an assimnet startTime which is later than the current time)
            enterTime = enterTime.AddSeconds(s_rand.Next(100, 18000));

            // logic for setting the ending type of the assignment while ensuring there are no more than the limit of active assignments
            if (call.MaxTime > s_dal.Config.Clock)
            {
                if (numOfActiveAssignments < activeAssignmentsLimit && volunteer.Active == true)
                {
                    randNum = s_rand.Next(0, 4);
                    eType = randNum switch
                    {
                        0 => EndingType.Solved,
                        1 => EndingType.SelfCanceld,
                        2 => EndingType.CanceledByManager,
                        3 => null,
                        _ => throw new ArgumentOutOfRangeException("Invalid randNum value")    // just to make sure, should never be reached
                    };
                }
                else
                {
                    randNum = s_rand.Next(0, 3);
                    eType = randNum switch
                    {
                        0 => EndingType.Solved,
                        1 => EndingType.SelfCanceld,
                        2 => EndingType.CanceledByManager,
                        _ => throw new ArgumentOutOfRangeException("Invalid randNum value")    // just to make sure, should never be reached
                    };
                }
            }
            else
            {
                randNum = s_rand.Next(0, 4);
                eType = randNum switch
                {
                    0 => EndingType.Solved,
                    1 => EndingType.SelfCanceld,
                    2 => EndingType.CanceledByManager,
                    3 => EndingType.Expired,
                    _ => throw new ArgumentOutOfRangeException("Invalid randNum value")    // just to make sure, should never be reached
                };               
            }

            if (eType == null)
            {
                endTime = null;
                numOfActiveAssignments++;   // to support the logic of limiting the number of active assignments
            }

            else if (eType == EndingType.Expired)
                endTime = call.MaxTime;

            else
            {
                // reach here if eType is Solved, SelfCanceld or CanceledByManager

                // the next assiment in the initialization start 100000 seconds after the previous one
                // we add a random number of seconds between 1 and 10000 so that this assignment will end before the next one starts
                // this helps to ensure that we wont run into a situation where a call in an assignment that was canceled will appear in a new assignment that start before the ending time of the canceled assignment
                endTime = enterTime.AddSeconds(s_rand.Next(1, 10000));  
            }

            s_dal.Assignment!.Create(new DO.Assignment()
            {
                VolunteerId = volunteer.Id,
                CallId = call.Id,
                EnterTime = enterTime,
                EndTime = endTime,
                EType = eType
            });
        }
    }

    //public static void Do(IDal dal) //stage 2
    public static void Do() //stage 4
    {
        // for now, the seed is reset to a fixed value so that all data, except for the data which depends on the current time, will remain consistent after every initialization.
        // this will make the program easier to test and debug.
        s_rand = new(27);

        s_dal = DalApi.Factory.Get; //stage 4

        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB();

        Console.WriteLine("Initializing Volunteers list ...");
        createVolunteers();
        Console.WriteLine("Initializing Calls list ...");
        createCalls();
        Console.WriteLine("Initializing Assignments list ...");
        createAssignments();
    }
}
