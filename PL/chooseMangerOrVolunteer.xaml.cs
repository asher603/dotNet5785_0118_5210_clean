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

namespace PL
{
    /// <summary>
    /// Interaction logic for chooseMangerOrVolunteer.xaml
    /// </summary>
    public partial class chooseMangerOrVolunteer : Window
    {
        public int idManger { get; set; }
        public chooseMangerOrVolunteer(int id)
        {
            idManger = id;
            InitializeComponent();
        }

        private void BtnManger_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(idManger).Show();
            this.Close();
        }
        private void BtnVolunteer_Click(object sender, RoutedEventArgs e)
        {
            new Volunteer.VolunteerWindow(idManger, idManger, false).Show();
        }
    }
}
