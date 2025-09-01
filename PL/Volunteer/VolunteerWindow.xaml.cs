using BO;
using PL.Call;
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


namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerWindow.xaml
/// </summary>
public partial class VolunteerWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    #region Variables Of Window
    public int ManagerId { get; set; }
    public BO.Volunteer? CurrentVolunteer
    {
        get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }
    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    public string password
    {
        get { return (string)GetValue(PasswordProperty); }
        set { SetValue(PasswordProperty, value); }
    }
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register("password", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(string.Empty));

    public string? ButtonAddUpdText
    {
        get { return GetValue(ButtonAddUpdTextProperty).ToString(); }
        set { SetValue(ButtonAddUpdTextProperty, value); }
    }
    public static readonly DependencyProperty ButtonAddUpdTextProperty =
        DependencyProperty.Register("ButtonAddUpdText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(null));

    public Visibility GridOfVolunteerButtonsVisibility
    {
        get { return (Visibility)GetValue(GridOfVolunteerButtonsVisibilityProperty); }
        set { SetValue(GridOfVolunteerButtonsVisibilityProperty, value); }
    }
    public static readonly DependencyProperty GridOfVolunteerButtonsVisibilityProperty =
        DependencyProperty.Register("GridOfVolunteerButtonsVisibility", typeof(Visibility), typeof(VolunteerWindow), new PropertyMetadata(Visibility.Collapsed));

    public bool IsEnabeldChooseCallClick
    {
        get { return (bool)GetValue(IsEnabeldChooseCallClickProperty); }
        set { SetValue(IsEnabeldChooseCallClickProperty, value); }
    }
    public static readonly DependencyProperty IsEnabeldChooseCallClickProperty =
        DependencyProperty.Register("IsEnabeldChooseCallClick", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(false));

    public bool IsEnabeldFinishCallClick
    {
        get { return (bool)GetValue(IsEnabeldFinishCallClickProperty); }
        set { SetValue(IsEnabeldFinishCallClickProperty, value); }
    }
    public static readonly DependencyProperty IsEnabeldFinishCallClickProperty =
        DependencyProperty.Register("IsEnabeldFinishCallClick", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(false));

    public bool IsEnabeldCancelCallClick
    {
        get { return (bool)GetValue(IsEnabeldCancelCallClickProperty); }
        set { SetValue(IsEnabeldCancelCallClickProperty, value); }
    }
    public static readonly DependencyProperty IsEnabeldCancelCallClickProperty =
        DependencyProperty.Register("IsEnabeldCancelCallClick", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(false));

    public bool IsEnabeldRole
    {
        get { return (bool)GetValue(IsEnabeldRoleProperty); }
        set { SetValue(IsEnabeldRoleProperty, value); }
    }
    public static readonly DependencyProperty IsEnabeldRoleProperty =
        DependencyProperty.Register("IsEnabeldRole", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(false));

    public bool EnterAsManager
    {
        get { return (bool)GetValue(EnterAsManagerProperty); }
        set { SetValue(EnterAsManagerProperty, value); }
    }
    public static readonly DependencyProperty EnterAsManagerProperty =
        DependencyProperty.Register("EnterAsManager", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(false));

    private volatile DispatcherOperation? _observerOperation = null;
    #endregion Variables Of Window

    #region Buttons Click function
    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ButtonAddUpdText == "Add")
            {
                CurrentVolunteer.Password = password;
                s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
                MessageBox.Show($"successfully added the volunteer");
            }
            else if (ButtonAddUpdText == "Update")
            {
                MessageBoxResult result = MessageBoxResult.Yes; // default value is yes for the case where the following if statement is not true, so the update request will be sent without any confirmation
                if (CurrentVolunteer.Active == false && CurrentVolunteer.CallInProgress != null)
                {
                    if (EnterAsManager)
                        result = MessageBox.Show("Updating volunteer state to inactive will cause cancellation of his in progress call. \nAre you sure you want to perform this update?", "Confirm update to inactive state", MessageBoxButton.YesNo);
                    else
                        result = MessageBox.Show("Updating your state to inactive will cause cancellation of your in progress call. \nAre you sure you want to perform this update?", "Confirm update to inactive state", MessageBoxButton.YesNo);
                }
                if (result == MessageBoxResult.Yes)
                {
                    if (password != "") CurrentVolunteer.Password = password;
                    s_bl.Volunteer.UpdateVolunteerDetails(ManagerId, CurrentVolunteer);
                    MessageBox.Show($"successfully updated the volunteer details");
                }
            }

            // if the window was opened by a manager it should be automatically closed after update,
            // if it was opened by a volunteer it should stay open because the volunteer has many more things to so in there other than updating his details
            if (EnterAsManager)
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

    private void btnFinishCall_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Call.EndCall(CurrentVolunteer.Id, CurrentVolunteer.CallInProgress.AsmId);
            //CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id);
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

    private void btnCancelledCall_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Call.CancelAssignment(CurrentVolunteer.Id, CurrentVolunteer.CallInProgress.AsmId);
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

    private void btnChooseCall_Click(object sender, RoutedEventArgs e)
    {
        new ChoosingCallWindow(CurrentVolunteer).Show();
    }

    private void btnCallsHistory_Click(object sender, RoutedEventArgs e)
    {
        new CallsHistoryWindow(CurrentVolunteer).Show();
    }

    private void btnGeneratePassword_Click(object sender, RoutedEventArgs e)
    {
        CurrentVolunteer.Password = s_bl.Volunteer.GeneratePassword();
        MessageBox.Show("Your new password is " + CurrentVolunteer.Password + " - remember it");
    }
    #endregion Buttons Click function

    #region Obsevers Functions
    private void volunteerDetailsObserver()
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() => CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id));
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
       // Registering the observer method to monitor changes in the BL
       => s_bl?.Volunteer.AddObserver(CurrentVolunteer!.Id, volunteerDetailsObserver); 

    private void Window_Closed(object sender, EventArgs e)
        // Removing the observer when the window is closed
        => s_bl?.Volunteer.RemoveObserver(CurrentVolunteer.Id, volunteerDetailsObserver);

    #endregion Obsevers Functions
    public VolunteerWindow(int id, int managerId, bool enterAsManager)
    {
        try
        {
            ButtonAddUpdText = id == 0 ? "Add" : "Update";
            ManagerId = managerId;
            EnterAsManager = enterAsManager;
            CurrentVolunteer = id != 0 ? s_bl.Volunteer.GetVolunteerDetails(id) : new BO.Volunteer();
            password = CurrentVolunteer.Password;
            if (CurrentVolunteer == null)
            {
                MessageBox.Show($"Volunteer with id: {id} not found", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }
            InitializeComponent();
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
    
}
