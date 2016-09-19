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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiscConnectionSuite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtUsername.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            //authenticate credentials with DB
            if(LoginAuthenticated()) {
                MainMenu menu = new MainMenu(txtUsername.Text.ToString().Trim());
                menu.Show();
                this.Close();
            }
        }

        private bool LoginAuthenticated()
        {
            string username = txtUsername.Text.ToString();
            string password = pwdPassword.Password.ToString();
            if (BusLayer.blMain.CallLogin(username, password) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
