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
    /// Interaction logic for CallsHistoryWindow.xaml
    /// </summary>
    public partial class CallsHistoryWindow : Window
    {
        // Static reference to the BL layer
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        #region Variables Of Window
        private BO.Volunteer CurrentVolunteer { get; set; }


        // Dependency property for the list of closed calls
        public IEnumerable<BO.ClosedCallInList> ClosedCallsList
        {
            get { return (IEnumerable<BO.ClosedCallInList>)GetValue(ClosedCallsListProperty); }
            set { SetValue(ClosedCallsListProperty, value); }
        }

        // Definition of the dependency property
        public static readonly DependencyProperty ClosedCallsListProperty =
            DependencyProperty.Register("ClosedCallsList", typeof(IEnumerable<BO.ClosedCallInList>), typeof(CallsHistoryWindow), new PropertyMetadata(null));

        public BO.CallType? SelectedFilterOption { get; set; } = null;

        public BO.ClosedCallInListField? SelectedSortOption { get; set; } = BO.ClosedCallInListField.Id;

        private volatile DispatcherOperation? _observerOperation = null;

        #endregion Variables Of Window
        
        private void OnClosedCallsListFilterOrSortOptionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // calling queryClosedCallsList to update the list based on the current filter and sort options
                closedCallsListObserver();
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

        // Method to query the filtered list and update the ClosedCallsList property
        private void queryClosedCallsList()
        {
            try
            {
                ClosedCallsList = s_bl?.Call.GetClosedCallsHandledByVolunteer(CurrentVolunteer.Id, SelectedFilterOption, SelectedSortOption)!;
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
        private void closedCallsListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() => queryClosedCallsList());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Registering the observer method to monitor changes in the BL
            s_bl?.Volunteer.AddObserver(CurrentVolunteer.Id, closedCallsListObserver);    // not sure if this is needed
            s_bl?.Call.AddObserver(closedCallsListObserver);
        }

        // Event triggered when the window is closed
        private void Window_Closed(object sender, EventArgs e)
        {
            // Removing the observer when the window is closed
            s_bl?.Volunteer.RemoveObserver(CurrentVolunteer.Id, closedCallsListObserver); // not sure if this is needed
            s_bl?.Call.RemoveObserver(closedCallsListObserver);
        }
        #endregion Obsevers Functions

        // Constructor initializes the component and queries the list of closed calls
        public CallsHistoryWindow(BO.Volunteer _currentVolunteer)
        {
            CurrentVolunteer = _currentVolunteer;
            InitializeComponent();
        }
    }
}