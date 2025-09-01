using BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PL.Volunteer;


/// <summary>
/// Interaction logic for VolunteerListWindow.xaml
/// </summary>
public partial class VolunteerListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    #region Variables Of Window
    public int ManagerId { get; set; }
    public IEnumerable<BO.VolunteerInList> VolunteerList
    {
        get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
        set { SetValue(VolunteerListProperty, value); }
    }

    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

    public BO.VolunteerInList? SelectedVolunteer { get; set; }
    public bool? SelectedActivationStatusFilter { get; set; } = null;
    public BO.CallType? SelectedCallTypeFilter { get; set; } = null;
    public BO.VolunteerInListFields? SelectedSortOption { get; set; } = BO.VolunteerInListFields.Id;
    private volatile DispatcherOperation? _observerOperation = null;

    #endregion Variables Of Window

    private void OnVolunteerSorterOrFilterChanged(object sender, SelectionChangedEventArgs e)
    {
        // calling volunteerListObserver to update the list based on the current filter option
        volunteerListObserver();    
    }   

    // Method to query the filtered list and update the VolunteerList property
    private void queryVolunteerList()
    {
        try
        {
            VolunteerList = s_bl?.Volunteer.GetVolunteersListDoubleFiltering(SelectedActivationStatusFilter, SelectedCallTypeFilter, SelectedSortOption)!;
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
    // Method that acts as an observer and calls queryVolunteerList to update the list
    private void volunteerListObserver()
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() => queryVolunteerList()); // calling queryVolunteerList to update the list based on the current filter option
    }

    // Event triggered when the window is loaded
    private void Window_Loaded(object sender, RoutedEventArgs e)
        // Registering the observer method to monitor changes in the BL
        => s_bl?.Volunteer.AddObserver(volunteerListObserver); // Registers the observer to the BL layer

    // Event triggered when the window is closed
    private void Window_Closed(object sender, EventArgs e)
        // Removing the observer when the window is closed
        => s_bl?.Volunteer.RemoveObserver(volunteerListObserver); // Unregisters the observer from the BL layer
    #endregion Obsevers Functions

    #region Buttons Click function
    private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedVolunteer != null)
            new VolunteerWindow(SelectedVolunteer.Id, ManagerId, true).Show();
    }
   
    private void btnAddNew_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerWindow(0, ManagerId, true).Show(); //id =0 adding
    }

    private void DeleteVolunteerButton_Click(object sender, RoutedEventArgs e)
    {
        // Get the volunteer from the Button's DataContext we maybe need to fix this
        BO.VolunteerInList volunteerToDelete = (BO.VolunteerInList)((Button)sender).DataContext;

        // Show a confirmation message box before deleting
        MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this volunteer?", "Confirm Deletion", MessageBoxButton.YesNo);

        // If the user clicks Yes, proceed with deletion
        if (result == MessageBoxResult.Yes)
        {
            try
            {
                // Attempt to delete the volunteer
                s_bl.Volunteer.DeleteVolunteer(volunteerToDelete.Id);

                // No further action needed; the volunteer will be removed automatically due to observer pattern
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
        // If the user clicks No, the MessageBox closes and nothing happens.
    }

    #endregion Buttons Click function
    public VolunteerListWindow(int managerId)
    {
        ManagerId = managerId;
        InitializeComponent();
    }
}
