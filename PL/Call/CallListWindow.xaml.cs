using BO;
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

public partial class CallListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    #region Variables Of Window
    public int idManger { get; set; }
    public IEnumerable<BO.CallInList> CallList
    {
        get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));


    public BO.CallInList? SelectedCall { get; set; }

    public BO.CallStatus? currentStatusFilterOption { get; set; } = null;
    public BO.CallType? currentCTypeFilterOption { get; set; } = null;
    public CallInListField? currentSortOption { get; set; } = BO.CallInListField.Id;

    private volatile DispatcherOperation? _observerOperation = null;
    #endregion Variables Of Window

    #region Buttons Click function
    private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedCall != null)
            new CallWindow(SelectedCall.CallId).Show();
    }

    private void btnAddNew_Click(object sender, RoutedEventArgs e)
    {
        new AddCallWindow().Show();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        // Get the call from the Button's DataContext
        BO.CallInList callToDelete = (BO.CallInList)((Button)sender).DataContext; // Access the DataContext directly

        // Show a confirmation message box before canceling
        MessageBoxResult result = MessageBox.Show("Are you sure you want to cancel this assignment?", "Confirm Deletion", MessageBoxButton.YesNo);

        // If the user clicks Yes, proceed with deletion
        if (result == MessageBoxResult.Yes)
        {
            try
            {
                // Attempt to cancel the call
                s_bl.Call.CancelAssignment(idManger, callToDelete.Id ?? -1);

                // No further action needed; the volunteer will be removed automatically due to observer pattern
            }
            catch (Exception ex)
            {
                // If an error occurs, show an error message
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // If the user clicks No, the MessageBox closes and nothing happens.

    }
    private void DeleteCallButton_Click(object sender, RoutedEventArgs e)
    {
        // Get the call from the Button's DataContext
        BO.CallInList callToDelete = (BO.CallInList)((Button)sender).DataContext; // Access the DataContext directly

        // Show a confirmation message box before deleting
        MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this Call?", "Confirm Deletion", MessageBoxButton.YesNo);

        // If the user clicks Yes, proceed with deletion
        if (result == MessageBoxResult.Yes)
        {
            try
            {
                // Attempt to delete the call
                s_bl.Call.DeleteCall(callToDelete.CallId);

                // No further action needed; the volunteer will be removed automatically due to observer pattern
            }
            catch (Exception ex)
            {
                // If an error occurs, show an error message
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    #endregion Buttons Click function
    
    private void OnCallFilterOrStatusChanged(object sender, SelectionChangedEventArgs e)
    {
        // calling queryVolunteerList to update the list based on the current filter and sort options
        CallListObserver();
    }

    // Method to query the filtered list and update the CallList property
    private void queryCallList()
    {
        //if (currentStatusFilterOption != BO.OnScreenCallFilterStatusOptions.NoFilter)
        //    CallList =  s_bl.Call.GetFilteredAndSortedCalls(CallInListField.Status, (BO.CallStatus)((int)(currentStatusFilterOption) - 1), currentSortOption);
        //if (currentCTypeFilterOption != null)
        //    CallList = s_bl.Call.GetFilteredAndSortedCalls(CallInListField.CType,currentCTypeFilterOption, currentSortOption);
        //else CallList = s_bl.Call.GetFilteredAndSortedCalls(null, null, currentSortOption);
        try
        {
            CallList = s_bl.Call.GetFilteredAndSortedCallsDoubleFiltering(currentStatusFilterOption, currentCTypeFilterOption, currentSortOption);
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

    #region Obsevers Functions
    // Method that acts as an observer and calls queryCallList to update the list
    private void CallListObserver()
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() => queryCallList()); // calling queryCallList to update the list based on the current filter option
    }

    // Event triggered when the window is loaded
    private void Window_Loaded(object sender, RoutedEventArgs e)
        // Registering the observer method to monitor changes in the BL
        => s_bl?.Call.AddObserver(CallListObserver); // Registers the observer to the BL layer

    // Event triggered when the window is closed
    private void Window_Closed(object sender, EventArgs e)
        // Removing the observer when the window is closed
        => s_bl?.Call.RemoveObserver(CallListObserver); // Unregisters the observer from the BL layer

    #endregion Obsevers Functions

    public CallListWindow(int id)
    {
        idManger = id;
        InitializeComponent();
    }

}
