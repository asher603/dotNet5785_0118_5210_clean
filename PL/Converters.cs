using BO;
using PL.Call;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PL;

class ConvertBoolToActivationStatusFilterEnum : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool? activationStatusAsBool = (bool?)value;
        return activationStatusAsBool switch
        {
            null => BO.ActivationStatusFilterOptions.NoFilter,
            true => BO.ActivationStatusFilterOptions.Active,
            false => BO.ActivationStatusFilterOptions.Inactive
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
    culture)
    {
        BO.ActivationStatusFilterOptions activationStatus = (BO.ActivationStatusFilterOptions)value;
        return activationStatus switch
        {
            BO.ActivationStatusFilterOptions.NoFilter => null!,
            BO.ActivationStatusFilterOptions.Active => true,
            BO.ActivationStatusFilterOptions.Inactive => false,
            _ => throw new BlInvalidInputException("invalid ActivationStatusFilterOptions") // just to make sure, should never be reached
        };
    }
}

class ConvertCallTypeEnumToCallTypeFilterEnum : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallType? callType = (BO.CallType?)value;
        return callType switch
        {
            null => BO.CallTypeFilterOptions.NoFilter,
            BO.CallType.TireChange => BO.CallTypeFilterOptions.TireChange,
            BO.CallType.JumpStart => BO.CallTypeFilterOptions.JumpStart,
            BO.CallType.FluidRefill => BO.CallTypeFilterOptions.FluidRefill,
            BO.CallType.LightFix => BO.CallTypeFilterOptions.LightFix,
            BO.CallType.LostKey => BO.CallTypeFilterOptions.LostKey,
            BO.CallType.None => BO.CallTypeFilterOptions.NoCallType,
            _ => throw new BlInvalidInputException("Invalid CallType")  // just to make sure, should never be reached
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
    culture)
    {
        BO.CallTypeFilterOptions callTypeFilter = (BO.CallTypeFilterOptions)value;
        return callTypeFilter switch
        {
            BO.CallTypeFilterOptions.NoFilter => null!,
            BO.CallTypeFilterOptions.TireChange => BO.CallType.TireChange,
            BO.CallTypeFilterOptions.JumpStart => BO.CallType.JumpStart,
            BO.CallTypeFilterOptions.FluidRefill => BO.CallType.FluidRefill,
            BO.CallTypeFilterOptions.LightFix => BO.CallType.LightFix,
            BO.CallTypeFilterOptions.LostKey => BO.CallType.LostKey,
            BO.CallTypeFilterOptions.NoCallType => BO.CallType.None,
            _ => throw new BlInvalidInputException("invalid CallTypeFilterOption") // just to make sure, should never be reached
        };
    }
}

class ConvertCallTypeEnumToCallTypeFilterOptionsWithoutNoCallTypeOptionEnum : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallType? callType = (BO.CallType?)value;
        return callType switch
        {
            null => BO.CallTypeFilterOptionsWithoutNoCallTypeOption.NoFilter,
            BO.CallType.TireChange => BO.CallTypeFilterOptionsWithoutNoCallTypeOption.TireChange,
            BO.CallType.JumpStart => BO.CallTypeFilterOptionsWithoutNoCallTypeOption.JumpStart,
            BO.CallType.FluidRefill => BO.CallTypeFilterOptionsWithoutNoCallTypeOption.FluidRefill,
            BO.CallType.LightFix => BO.CallTypeFilterOptionsWithoutNoCallTypeOption.LightFix,
            BO.CallType.LostKey => BO.CallTypeFilterOptionsWithoutNoCallTypeOption.LostKey,
            _ => throw new BlInvalidInputException("Invalid CallType")  // just to make sure, should never be reached
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
    culture)
    {
        BO.CallTypeFilterOptionsWithoutNoCallTypeOption callTypeFilter = (BO.CallTypeFilterOptionsWithoutNoCallTypeOption)value;
        return callTypeFilter switch
        {
            BO.CallTypeFilterOptionsWithoutNoCallTypeOption.NoFilter => null!,
            BO.CallTypeFilterOptionsWithoutNoCallTypeOption.TireChange => BO.CallType.TireChange,
            BO.CallTypeFilterOptionsWithoutNoCallTypeOption.JumpStart => BO.CallType.JumpStart,
            BO.CallTypeFilterOptionsWithoutNoCallTypeOption.FluidRefill => BO.CallType.FluidRefill,
            BO.CallTypeFilterOptionsWithoutNoCallTypeOption.LightFix => BO.CallType.LightFix,
            BO.CallTypeFilterOptionsWithoutNoCallTypeOption.LostKey => BO.CallType.LostKey,
            _ => throw new BlInvalidInputException("invalid OnScreenOpenCallFilterOption") // just to make sure, should never be reached
        };
    }
}


class ConvertCallStatusEnumToOnScreenCallFilterStatusOptionsEnum : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallStatus? callStatus = (BO.CallStatus?)value;
        return callStatus switch
        {
            null => BO.OnScreenCallFilterStatusOptions.NoFilter,
            BO.CallStatus.Open => BO.OnScreenCallFilterStatusOptions.Open,
            BO.CallStatus.InProgress => BO.OnScreenCallFilterStatusOptions.InProgress,
            BO.CallStatus.Closed => BO.OnScreenCallFilterStatusOptions.Closed,
            BO.CallStatus.Expired => BO.OnScreenCallFilterStatusOptions.Expired,
            BO.CallStatus.OpenInRisk => BO.OnScreenCallFilterStatusOptions.OpenInRisk,
            BO.CallStatus.InProgressInRisk => BO.OnScreenCallFilterStatusOptions.InProgressInRisk,
            _ => throw new BlInvalidInputException("Invalid CallStatus")  // just to make sure, should never be reached
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.OnScreenCallFilterStatusOptions callFilterStatus = (BO.OnScreenCallFilterStatusOptions)value;
        return callFilterStatus switch
        {
            BO.OnScreenCallFilterStatusOptions.NoFilter => null!,
            BO.OnScreenCallFilterStatusOptions.Open => BO.CallStatus.Open,
            BO.OnScreenCallFilterStatusOptions.InProgress => BO.CallStatus.InProgress,
            BO.OnScreenCallFilterStatusOptions.Closed => BO.CallStatus.Closed,
            BO.OnScreenCallFilterStatusOptions.Expired => BO.CallStatus.Expired,
            BO.OnScreenCallFilterStatusOptions.OpenInRisk => BO.CallStatus.OpenInRisk,
            BO.OnScreenCallFilterStatusOptions.InProgressInRisk => BO.CallStatus.InProgressInRisk,
            _ => throw new BlInvalidInputException("Invalid OnScreenCallFilterStatusOptions") // just to make sure, should never be reached
        };
    }

}


class ConvertUpdateToTrue : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string btnText = (string)value;
        return btnText switch
        {
            "Update" => true,
            _ => false
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
    culture)
    {
        throw new NotImplementedException();
    }
}


// for converting bool to Visibility
class ConvertTrueToCollapsed : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool enterAsManager = (bool)value;
        //if (parameter == )
        return enterAsManager switch
        {
            true => Visibility.Collapsed,
            _ => Visibility.Visible
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
    culture)
    {
        throw new NotImplementedException();
    }
}


// for converting CallInProgress to Visibility
class ConvertNullToCollapsed : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallInProgress callInProgress = (BO.CallInProgress)value;
        return callInProgress switch
        {
            null => Visibility.Collapsed,                
            _ => Visibility.Visible
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
    culture)
    {
        throw new NotImplementedException();
    }
}


class ConvertNullCallInProgressAndActiveToTrue : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.Volunteer currentVolunteer = (BO.Volunteer)value;
        return currentVolunteer.CallInProgress == null && currentVolunteer.Active == true;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
    culture)
    {
        throw new NotImplementedException();
    }
}


class ConvertNullToFalse : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallInProgress callInProgress = (BO.CallInProgress)value;
        return callInProgress switch
        {
            null => false,
            _ => true
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
    culture)
    {
        throw new NotImplementedException();
    }
}

//class ConvertDeleteCallList : IValueConverter
//{
    
//    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        if((int)value == 0) return Visibility.Visible;
//        return Visibility.Collapsed;
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
//    culture)
//    {
//        throw new NotImplementedException();
//    }
//}

class ConvertDeleteCallList : IValueConverter
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int callId = (int)value;
        return s_bl.Call.IsDeletableCall(callId) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
    culture)
    {
        throw new NotImplementedException();
    }
}

class ConvertCancelCallList : IValueConverter
{

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallStatus status = (BO.CallStatus)value;
        if (status == CallStatus.InProgress || status == CallStatus.InProgressInRisk) 
            return Visibility.Visible;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
    culture)
    {
        throw new NotImplementedException();
    }
}

class ConvertButtomTextSimulator : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ?  "Start Simulator" : "Stop Simulator";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
    culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// The convertor checks if the password is strong, medium, weak and change the color of the text
/// </summary>
/// <returns>weak= Red, medium= Orange, strong = Green</returns>
class ConvertColorPassword : IValueConverter
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int ratePassword = s_bl.Volunteer.RatePassword((string)value);
        if (ratePassword == 2) return Brushes.Green;
        if (ratePassword == 1) return Brushes.Orange;
        return Brushes.Red;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
    culture)
    {
        throw new NotImplementedException();
    }
}


class ConvertEmptyAddressToRedBackground : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // to use for displaying the address in red if it wasent validated yet or if it was found to be invalid
        string Address = (string)value;
        return string.IsNullOrEmpty(Address) ? Brushes.Red : Brushes.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
    culture)
    {
        throw new NotImplementedException();
    }
}

class ConvertStatusColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        CallStatus status = (CallStatus)value;
        return status switch
        {
            CallStatus.Expired => Brushes.Gray,
            CallStatus.Closed=>Brushes.Green,
            CallStatus.Open => Brushes.Blue,
            CallStatus.InProgress => Brushes.Orange,
            CallStatus.InProgressInRisk => Brushes.Red,
            CallStatus.OpenInRisk => Brushes.Purple,
            _=>Brushes.Black
        };

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo
    culture)
    {
        throw new NotImplementedException();
    }
}