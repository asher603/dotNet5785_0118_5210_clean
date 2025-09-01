namespace BO;

/// <param name="MainMenu">The main menu</param>
public enum MainMenu { EXIT, ADMIN, CALL, VOLUNTEER }

/// <param name="VolunteerMenu">The volunteer menu</param>
public enum VolunteerMenuEn { EXIT, VolunteerRegistration, GetVolunteersList, GetVolunteerDetails, UpdateVolunteerDetails, DeleteVolunteer, AddVolunteer };

/// <param name="CallMenu">The call menu</param>
public enum CallMenuEn { EXIT, GetCallQuantitiesByStatus, GetFilteredAndSortedCalls, GetCallDetails, UpdateCall, DeleteCall, AddCall, GetClosedCallsHandledByVolunteer, GetOpenCallsOfVolunteer, EndCall, CancelAssignment, ChooseCall }

/// <param name="AdminMenu">The admin menu</param>
public enum AdminMenuEn { EXIT, GetClock, AdvanceClock, GetRiskRange, SetRiskRange, ResetDB, InitializeDB }

/// <param name="Role">The role of the volunteer</param>
public enum Role { Manager, Volunteer };

/// <param name="DistanceType">How you calculate the distance</param>
public enum DistanceType { Air, Walking, Driving };

/// <param name="CallType">The type of call</param>
public enum CallType { TireChange, JumpStart, FluidRefill, LightFix, LostKey, None }

/// <param name="EndingType"> enum containing all types of ending for the call handling </param>
public enum EndingType { Solved, SelfCanceld, CanceledByManager, Expired };

/// <param name="InProgressCallStatus"> enum containing all types of status for in prograss call handling </param>
public enum InProgressStatus { InProgress, InProgressInRisk };

/// <param name="CallStatus"> enum containing all types of status for call handling </param>
public enum CallStatus { Open, InProgress, Closed, Expired, OpenInRisk, InProgressInRisk };

/// <param name="TimeUnit"> enum containing all types of time units that we need </param>
public enum TimeUnit { Minute, Hour, Day, Month, Year };

//public enum volunteerFilterOptions { Active, Inactive, All };

/// <param name="VolunteerInListFields"> enum containing all the fields of VolunteerInList </param>
public enum VolunteerInListFields
{
    Id,
    FullName,
    Active,
    TotalCallsHandled,
    TotalCallsCancelled,
    TotalCallsExpired,
    CallInProgressId,
    CType
}

/// <param name="CallInListFields"> enum containing all the fields of CallInList </param>
public enum CallInListField
{
    Id,
    CallId,
    CType,
    Opening,
    TimeLeft,
    LastVolunteer,
    HandlingTime,
    Status,
    TotalAssignments
}

/// <param name="ClosedCallInListField"> enum containing all the fields of ClosedCallInList </param>

public enum ClosedCallInListField
{
    Id,
    CType,
    FullAddress,
    Opening,
    EnterTime,
    EndTime,
    EType
}

/// <param name="OpenCallInListField"> enum containing all the fields of OpenCallInList </param>
public enum OpenCallInListField
{
    Id,
    CType,
    Description,
    FullAddress,
    Opening,
    MaxTime,
    Distance
}

/// <param name="VolunteerFilterOptions"> enum containing all the options the for the filtering of the volunteers list </param>
public enum VolunteerFilterOptions { Active, CType }

/// <param name="OnScreenVolunteerFilterOptions"> enum containing all the options for the value of the field to filter by, which will appear in the combo box on the screen </param>
public enum OnScreenVolunteerFilterOptions { All, Active, Inactive, TireChange, JumpStart, FluidRefill, LightFix, LostKey, NoCallType }

public enum ActivationStatusFilterOptions { Active, Inactive, NoFilter }

public enum CallTypeFilterOptions { NoFilter, TireChange, JumpStart, FluidRefill, LightFix, LostKey, NoCallType}

public enum CallTypeFilterOptionsWithoutNoCallTypeOption { TireChange, JumpStart, FluidRefill, LightFix, LostKey, NoFilter }



public enum OnScreenCallFilterStatusOptions
{
    NoFilter,
    Open, InProgress, Closed, Expired, OpenInRisk, InProgressInRisk // = Call Status
}
