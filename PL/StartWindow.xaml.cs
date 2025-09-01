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

namespace PL;

/// <summary>
/// Interaction logic for StartWindow.xaml
/// </summary>
public partial class StartWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public int logInId
    {
        get { return (int)GetValue(logInIdProperty); }
        set { SetValue(logInIdProperty, value); }
    }
    public static readonly DependencyProperty logInIdProperty =
        DependencyProperty.Register("logInId", typeof(int), typeof(StartWindow));
    public string password
    {
        get { return (string)GetValue(PasswordProperty); }
        set { SetValue(PasswordProperty, value); }
    }
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register("password", typeof(string), typeof(StartWindow), new PropertyMetadata(string.Empty));

    bool mangerIsConn = false;
    public StartWindow()
    {
        InitializeComponent();
    }

    private void btnLogIn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BO.Role volunteerRole = s_bl.Volunteer.GetVolunteerRoleAndValidatePasswordForRegistration(logInId, password);
            {
                if (!mangerIsConn && volunteerRole == BO.Role.Manager)
                {
                    mangerIsConn = true;
                    new chooseMangerOrVolunteer(logInId).Show();
                }
                else
                {
                    new VolunteerWindow(logInId, logInId, false).Show();    // its a self update so the updator id and the volunteer to update id are the same
                }
            }
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                MessageBox.Show($"{ex.Message} \n{ex.InnerException.Message}");
            }
            else
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

    }
}
