using BO;
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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for ChoosingCallWindow.xaml
    /// </summary>
    public partial class ChoosingCallWindow : Window
    {
        // Static reference to the BL layer
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        #region Variables Of Window
        private volatile DispatcherOperation? _observerCallOperation = null;
        private volatile DispatcherOperation? _observerVolunteerOperation = null;
        private BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }
        // Definition of the dependency property
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(ChoosingCallWindow), new PropertyMetadata(null));

        // Dependency property for the list of open calls
        public IEnumerable<BO.OpenCallInList> OpenCallsList
        {
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallsListProperty); }
            set { SetValue(OpenCallsListProperty, value); }
        }
        // Definition of the dependency property
        public static readonly DependencyProperty OpenCallsListProperty =
            DependencyProperty.Register("OpenCallsList", typeof(IEnumerable<BO.OpenCallInList>), typeof(ChoosingCallWindow), new PropertyMetadata(null));

        // Property to hold the currently selected open call
        public BO.OpenCallInList? SelectedOpenCall
        {
            get { return (BO.OpenCallInList)GetValue(SelectedOpenCallProperty); }
            set { SetValue(SelectedOpenCallProperty, value); }
        }
        // Definition of the dependency property
        public static readonly DependencyProperty SelectedOpenCallProperty =
            DependencyProperty.Register("SelectedOpenCall", typeof(BO.OpenCallInList), typeof(ChoosingCallWindow), new PropertyMetadata(null));
        
        public BO.CallType? SelectedFilterOption { get; set; } = null;

        public BO.OpenCallInListField SelectedSortOption { get; set; } = BO.OpenCallInListField.Id;

        #endregion Variables Of Window
        private void OnOpenCallsListFilterOrSortOptionChanged(object sender, SelectionChangedEventArgs e)
        {
            // calling openCallsListObserver to update the list based on the current filter and sort options
            openCallsListObserver();
        }

        // Method to query the filtered list and update the OpenCallsList property
        private void queryOpenCallsList()
        {
            try
            {
                OpenCallsList = s_bl?.Call.GetOpenCallsOfVolunteer(CurrentVolunteer.Id, SelectedFilterOption, SelectedSortOption)!;
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
        private void openCallsListObserver()
        {
            if (_observerCallOperation is null || _observerCallOperation.Status == DispatcherOperationStatus.Completed)
                _observerCallOperation = Dispatcher.BeginInvoke(() => queryOpenCallsList());
        }

        private void currentVolunteerDetailsObserver()
        {
            try
            {
                if (_observerVolunteerOperation is null || _observerVolunteerOperation.Status == DispatcherOperationStatus.Completed)
                    _observerVolunteerOperation = Dispatcher.BeginInvoke(() => CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id));
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Registering the observer method to monitor changes in the BL
            s_bl?.Call.AddObserver(openCallsListObserver);
            s_bl?.Volunteer.AddObserver(CurrentVolunteer.Id, currentVolunteerDetailsObserver);
        }

        // Event triggered when the window is closed
        private void Window_Closed(object sender, EventArgs e)
        {
            // Removing the observer when the window is closed
            s_bl?.Volunteer.RemoveObserver(CurrentVolunteer.Id, currentVolunteerDetailsObserver);
            s_bl?.Call.RemoveObserver(openCallsListObserver);
        }
        #endregion Obsevers Functions

        #region Buttons Click function
        private void btnChooseCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the chosenCall from the Button's DataContext we maybe need to fix this
                BO.OpenCallInList chosenCall = (BO.OpenCallInList)((Button)sender).DataContext;
                s_bl.Call.ChooseCall(CurrentVolunteer.Id, chosenCall.Id);    // calling the choose call function from the BL layer
                MessageBox.Show($"you have successfully chosen to handle the call with ID: {chosenCall.Id}");
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

        private void btnUpdateDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // this window can be opend only of the volunteer doesnt have a call in progress,
                // but its still possible that while this window is open the volunteer got assigned to a call in some other window.
                // therefore we need to check if the volunteer has a call in progress and we can not assume that he doesn't have one

                MessageBoxResult result = MessageBoxResult.Yes; // default value is yes for the case where the following if statement is not true, so the update request will be sent without any confirmation
                if (CurrentVolunteer.Active == false && CurrentVolunteer.CallInProgress != null)
                {
                    result = MessageBox.Show("Updating your state to inactive will cause cancellation of your in progress call. \nAre you sure you want to perform this update?", "Confirm update to inactive state", MessageBoxButton.YesNo);
                    this.Close();   // closing the window because the volunteer is no longer active and therefore can not choose a call
                }
                if (result == MessageBoxResult.Yes)
                {
                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id, CurrentVolunteer);  // calling the update function from the BL layer
                    MessageBox.Show($"successfully updated your details");
                    if (CurrentVolunteer.Active == false)   // checking if the volunteer state was updated to inactive
                    {
                        this.Close();   // closing the window because the volunteer is no longer active and therefore can not choose a call
                    }
                }                
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

        #endregion Buttons Click function

        // Constructor initializes the component and queries the list of open calls
        public ChoosingCallWindow(BO.Volunteer _currentVolunteer)
        {
            CurrentVolunteer = _currentVolunteer;
            InitializeComponent();
        }
    }
}