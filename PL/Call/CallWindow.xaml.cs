using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PL.Call;

/// <summary>
/// Interaction logic for CallWindow.xaml
/// </summary>
public partial class CallWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    
    #region Variables Of Window
    public BO.Call? CurrentCall
    {
        get { return (BO.Call?)GetValue(CurrentCallProperty); }
        set { SetValue(CurrentCallProperty, value); }
    }
    public static readonly DependencyProperty CurrentCallProperty =
        DependencyProperty.Register("CurrentCallProperty", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

    public bool canBeChanged
    {
        get { return (bool)GetValue(CanBeChangedProperty); }
        set { SetValue(CanBeChangedProperty, value); }
    }
    public static readonly DependencyProperty CanBeChangedProperty =
        DependencyProperty.Register("canBeChangedProperty", typeof(bool), typeof(CallWindow), new PropertyMetadata(null));

    public bool maxTimeChange
    {
        get { return (bool)GetValue(MaxTimeChangeProperty); }
        set { SetValue(MaxTimeChangeProperty, value); }
    }
    public static readonly DependencyProperty MaxTimeChangeProperty =
        DependencyProperty.Register("MaxTimeChangeProperty", typeof(bool), typeof(CallWindow), new PropertyMetadata(null));
    #endregion Variables Of Window

    private void btnUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Call.UpdateCall(CurrentCall);
            MessageBox.Show($"successfully updated the call");
            this.Close();
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                MessageBox.Show($"{ex.Message} \n{ex.InnerException.Message}", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show($"{ex.Message}", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public CallWindow(int callId)
    {
       
        CurrentCall = s_bl.Call.GetCallDetails(callId);
       
        canBeChanged = (CurrentCall.Status == BO.CallStatus.Open || CurrentCall.Status == BO.CallStatus.OpenInRisk);
        maxTimeChange = (CurrentCall.Status != BO.CallStatus.Expired && CurrentCall.Status != BO.CallStatus.Closed);
        InitializeComponent();
    }
}
