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

namespace PL.Call;

/// <summary>
/// Interaction logic for AddCallWindow.xaml
/// </summary>
public partial class AddCallWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    
    public BO.Call? CurrentCall
    {
        get { return (BO.Call?)GetValue(AddCurrentCallProperty); }
        set { SetValue(AddCurrentCallProperty, value); }
    }
    public static readonly DependencyProperty AddCurrentCallProperty =
        DependencyProperty.Register("AddCurrentCallProperty", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));
    
    
    public AddCallWindow()
    {
        CurrentCall = new BO.Call();
        InitializeComponent();
    }

    private void btnAddNew_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            MessageBox.Show(CurrentCall.ToString());
            s_bl.Call.AddCall(CurrentCall);
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
        this.Close();
    }
}
