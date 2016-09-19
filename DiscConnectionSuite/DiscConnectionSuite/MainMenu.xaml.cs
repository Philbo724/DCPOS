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

namespace DiscConnectionSuite
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        private string userId;

        public MainMenu(string newUserId)
        {
            InitializeComponent();
            userId = newUserId;
        }

        private void btnPurchase_Click(object sender, RoutedEventArgs e)
        {
            Purchase.Customer customer = new Purchase.Customer(userId);
            customer.Show();
            this.Close();
        }

        private void btnCustomers_Click(object sender, RoutedEventArgs e)
        {
            Administration.AdminCustomer customer = new Administration.AdminCustomer(userId);
            customer.Show();
            this.Close();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
