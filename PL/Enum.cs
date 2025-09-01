using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;

public class CallStatusCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallStatus> s_enums =
(Enum.GetValues(typeof(BO.CallStatus)) as IEnumerable<BO.CallStatus>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}



public class VolunteerCollection : IEnumerable
{
    static readonly IEnumerable<BO.OnScreenVolunteerFilterOptions> s_enums =
(Enum.GetValues(typeof(BO.OnScreenVolunteerFilterOptions)) as IEnumerable<BO.OnScreenVolunteerFilterOptions>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class ActivationStatusFilterOptionsCollection : IEnumerable
{
    static readonly IEnumerable<BO.ActivationStatusFilterOptions> s_enums =
(Enum.GetValues(typeof(BO.ActivationStatusFilterOptions)) as IEnumerable<BO.ActivationStatusFilterOptions>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class CallTypeFilterOptionsCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallTypeFilterOptions> s_enums =
(Enum.GetValues(typeof(BO.CallTypeFilterOptions)) as IEnumerable<BO.CallTypeFilterOptions>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class CallTypeFilterOptionsWithoutNoCallTypeOptionCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallTypeFilterOptionsWithoutNoCallTypeOption> s_enums =
(Enum.GetValues(typeof(BO.CallTypeFilterOptionsWithoutNoCallTypeOption)) as IEnumerable<BO.CallTypeFilterOptionsWithoutNoCallTypeOption>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}


public class VolunteerInListFieldsCollection : IEnumerable
{
    static readonly IEnumerable<BO.VolunteerInListFields> s_enums =
(Enum.GetValues(typeof(BO.VolunteerInListFields)) as IEnumerable<BO.VolunteerInListFields>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class VolunteerRole : IEnumerable
{
    static readonly IEnumerable<BO.Role> s_enums =
(Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class VolunteerTypeDis: IEnumerable
{
    static readonly IEnumerable<BO.DistanceType> s_enums =
(Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class CallFilterByStatus : IEnumerable
{
    static readonly IEnumerable<BO.OnScreenCallFilterStatusOptions> s_enums =
(Enum.GetValues(typeof(BO.OnScreenCallFilterStatusOptions)) as IEnumerable<BO.OnScreenCallFilterStatusOptions>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}



public class CallInListCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallInListField> s_enums =
(Enum.GetValues(typeof(BO.CallInListField)) as IEnumerable<BO.CallInListField>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}


public class OpenCallInListSortOptionsKey : IEnumerable
{
    static readonly IEnumerable<BO.OpenCallInListField> s_enums =
(Enum.GetValues(typeof(BO.OpenCallInListField)) as IEnumerable<BO.OpenCallInListField>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class ClosedCallInListSortOptionsKey : IEnumerable
{
    static readonly IEnumerable<BO.ClosedCallInListField> s_enums =
(Enum.GetValues(typeof(BO.ClosedCallInListField)) as IEnumerable<BO.ClosedCallInListField>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}


public class CallTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallType> s_enums =
(Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}