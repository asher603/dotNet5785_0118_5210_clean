using BO;
using PL.Call;
using PL.Volunteer;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    #region Variables Of Window
    public int idManger { get; set; }

    public int[] CallQuantitiesArray
    {
        get { return (int[])GetValue(CallQuantitiesArrayProperty); }
        set { SetValue(CallQuantitiesArrayProperty, value); }
    }
    public static readonly DependencyProperty CallQuantitiesArrayProperty =
        DependencyProperty.Register("CallQuantitiesArray", typeof(int[]), typeof(MainWindow));

    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }
    public static readonly DependencyProperty CurrentTimeProperty = 
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

    public TimeSpan RiskRange
    {
        get { return (TimeSpan)GetValue(RiskRangeProperty); }
        set { SetValue(RiskRangeProperty, value); }
    }
    public static readonly DependencyProperty RiskRangeProperty =
        DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow));

    public int Interval
    {
        get { return (int)GetValue(IntervalProperty); }
        set { SetValue(IntervalProperty, value); }
    }
    public static readonly DependencyProperty IntervalProperty =
        DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow));

    public bool CanStartSimulator
    {
        get { return (bool)GetValue(CanStartSimulatorProperty); }
        set { SetValue(CanStartSimulatorProperty, value); }
    }
    public static readonly DependencyProperty CanStartSimulatorProperty =
        DependencyProperty.Register("CanStartSimulator", typeof(bool), typeof(MainWindow));


    private volatile DispatcherOperation? _observerClockOperation = null;
    private volatile DispatcherOperation? _observerConfigOperation = null;
    private volatile DispatcherOperation? _observerCallOperation = null;
    #endregion Variables Of Window

    #region Buttons Click function
    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Minute);
    }

    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Hour);
    }

    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Day);
    }

    private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Month);
    }

    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Year);
    }
 
    private void RiskRangeInput_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Ensure the current value of RiskRange is passed to the DAL
            s_bl.Admin.SetRiskRange(RiskRange);

            // Display a confirmation message
            MessageBox.Show($"Risk range updated to: {RiskRange}");
        }
        catch (Exception ex)
        {
            // Handle any errors that may occur
            MessageBox.Show($"Error updating risk range: {ex.Message}");
        }
    }

    private void btnHandleVol_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerListWindow(idManger).Show();
    }
    private void btnHandleCal_Click(object sender, RoutedEventArgs e)
    {
        new CallListWindow(idManger).Show();
    }
    private void btnResetDB_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show(
            "Are you sure?", "Warning",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            //close all open windows
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                {
                    window.Close();
                }
            }
            s_bl.Admin.ResetDB();
            Mouse.OverrideCursor = Cursors.Arrow;
            CallQuantitiesArrayObserver();
        }
    }
    private void btnInitDB_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show(
            "Are you sure?", "Warning",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            //close all open windows
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                {
                    window.Close();
                }
            }
            s_bl.Admin.InitializeDB();
            Mouse.OverrideCursor = Cursors.Arrow;
            CallQuantitiesArrayObserver();
        }
    }


    private void btnStartStopSimulator(object sender, RoutedEventArgs e)
    {
        if (CanStartSimulator)
        {
            s_bl.Admin.StartSimulator(Interval);
            CanStartSimulator = false;
        }
        else
        {
            s_bl.Admin.StopSimulator();
            CanStartSimulator = true;
        }
    }
    #endregion Buttons Click function

    #region Obsevers Functions
    private void queryCallQuantitiesArray()
    {
        CallQuantitiesArray = s_bl.Call.GetCallQuantitiesByStatus();
    }

    private void clockObserver()
    {
        if (_observerClockOperation is null || _observerClockOperation.Status == DispatcherOperationStatus.Completed)
            _observerClockOperation = Dispatcher.BeginInvoke(() =>CurrentTime = s_bl.Admin.GetClock());
    }

    private void configObserver()
    {
        if (_observerConfigOperation is null || _observerConfigOperation.Status == DispatcherOperationStatus.Completed)
            _observerConfigOperation = Dispatcher.BeginInvoke(() => RiskRange = s_bl.Admin.GetRiskRange());
    }

    private void CallQuantitiesArrayObserver()
    {
        if (_observerCallOperation is null || _observerCallOperation.Status == DispatcherOperationStatus.Completed)
            _observerCallOperation = Dispatcher.BeginInvoke(() => queryCallQuantitiesArray());
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Set the current system clock value using the Admin interface
        CurrentTime = s_bl.Admin.GetClock();

        // Set configuration variables using the Admin interface
        RiskRange = s_bl.Admin.GetRiskRange();

        // Add the clock observer method to observe clock changes
        s_bl.Admin.AddClockObserver(clockObserver);

        // Add the configuration observer method to observe configuration changes
        s_bl.Admin.AddConfigObserver(configObserver);

        // Add the call quantities array observer method to observe call quantity changes
        s_bl?.Call.AddObserver(CallQuantitiesArrayObserver); 
    }
    private void MainWindow_Closed(object sender, EventArgs e)
    {
        // Remove the clock observer
        s_bl.Admin.RemoveClockObserver(clockObserver);

        // Remove the configuration observer
        s_bl.Admin.RemoveConfigObserver(configObserver);

        // Remove the call quantities array observer
        s_bl?.Call.RemoveObserver(CallQuantitiesArrayObserver);
    }
    #endregion Obsevers Functions
 

    public MainWindow(int id)
    {
        idManger = id;
        CanStartSimulator = true;
        Interval = 180; // the amount of time in minutes that the clock will advance every second during the simulation
        CallQuantitiesArrayObserver();
        InitializeComponent();
    }

}