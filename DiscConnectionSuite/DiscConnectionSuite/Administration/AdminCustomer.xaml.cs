using System;
using System.Collections.Generic;
using System.Data;
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

namespace DiscConnectionSuite.Administration
{
    /// <summary>
    /// Interaction logic for AdminCustomer.xaml
    /// </summary>
    public partial class AdminCustomer : Window
    {
        private string userId;
        private int mode;
        private int customerId;

        public AdminCustomer(string newUserId)
        {
            InitializeComponent();
            userId = newUserId;
            mode = 0;
            GetCustomers();
        }

        private void GetCustomers()
        {
            //get list of customers for mode
            DataTable dt = new DataTable();
            switch (mode)
            {
                case 0:
                    dt = BusLayer.blAdmin.GetCustomers();
                    break;
                case 1:
                    dt = BusLayer.blAdmin.GetBlacklistedCustomers();
                    break;
                case 2:
                    dt = BusLayer.blAdmin.GetAllCustomers();
                    break;
                default:
                    ErrMsg.Content = "Unable to retrieve customers.";
                    return;
            }

            cbCustomers.Items.Clear();

            string name = string.Empty;
            int i;

            //add customers to combobox
            cbCustomers.Items.Add("Select a customer...");

            for (i = 0; i < dt.Rows.Count; i++)
            {
                name = dt.Rows[i]["LastName"].ToString();
                name = name + ", ";
                name = name + dt.Rows[i]["FirstName"].ToString();
                cbCustomers.Items.Add(name);
            }

            cbCustomers.SelectedIndex = 0;

        }

        private void btMode_Click(object sender, RoutedEventArgs e)
        {
            switch (mode)
            {
                case 0:
                    ACWindow.Title = "Blacklisted Customers";
                    btMode.Content = "View All";
                    mode = 1;
                    ClearForm();
                    break;
                case 1:
                    ACWindow.Title = "All Customers";
                    btMode.Content = "View Available";
                    mode = 2;
                    ClearForm();
                    break;
                case 2:
                    ACWindow.Title = "Available Customers";
                    btMode.Content = "View Blacklist";
                    mode = 0;
                    ClearForm();
                    break;
                default:
                    break;
            }

            GetCustomers();

        }

        private void xcbCustomers_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (cbCustomers.SelectedIndex <= 0)
            {
                return;
            }

            try
            {
                string customer = cbCustomers.SelectedItem.ToString();
                string fName = customer.Substring(customer.IndexOf(",") + 2);
                string lName = customer.Remove(customer.IndexOf(","));

                txFirstName.Text = fName;
                txLastName.Text = lName;

                DataTable dt = BusLayer.blAdmin.GetSingleCustomer(fName, lName);

                if (dt.Rows.Count != 1)
                {
                    ErrMsg.Content = "A database error has occurred. Check connectivity and/or call Jonathan.";
                    return;
                }

                txAddress.Text = dt.Rows[0]["StreetAddress"].ToString();
                txCity.Text = dt.Rows[0]["City"].ToString();
                txState.Text = dt.Rows[0]["State"].ToString();
                txZipCode.Text = dt.Rows[0]["ZipCode"].ToString();
                txIdentifier.Text = dt.Rows[0]["Identifier"].ToString();
                if ((bool)dt.Rows[0]["Blacklist"] == true)
                {
                    chBlacklist.IsChecked = true;
                }

                if (dt.Rows[0]["Phone"] != DBNull.Value)
                {
                    txPhone.Text = dt.Rows[0]["Phone"].ToString();
                }

                customerId = Convert.ToInt32(dt.Rows[0]["CustomerID"].ToString());
            }
            catch (Exception)
            {
                ErrMsg.Content = "An error occurred. Please try again.";
            }
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm() == false)
                {
                    return;
                }

                if (BusLayer.blAdmin.SaveCustomer(customerId.ToString(), txFirstName.Text.ToString().Trim(),
                    txLastName.Text.ToString().Trim(), txAddress.Text.ToString().Trim(),
                    txCity.Text.ToString().Trim(), txState.Text.ToString().Trim(), txZipCode.Text.ToString().Trim(),
                    txPhone.Text.ToString().Trim(), txIdentifier.Text.ToString().Trim(), (chBlacklist.IsChecked == true)) == true)
                {

                    ClearForm();
                    ErrMsg.Content = "Customer information saved.";
                }
                else
                {
                    ErrMsg.Content = "An error occurred while saving. Please try again.";
                }
            }
            catch (Exception)
            {

            }
        }
        
        private void ClearForm()
        {
            try
            {
                //clear ALL fields
                cbCustomers.Items.Clear();
                txFirstName.Text = string.Empty;
                txLastName.Text = string.Empty;
                txIdentifier.Text = string.Empty;
                txAddress.Text = string.Empty;
                txCity.Text = string.Empty;
                txState.Text = string.Empty;
                txZipCode.Text = string.Empty;
                txPhone.Text = string.Empty;
                ErrMsg.Content = string.Empty;
                chBlacklist.IsChecked = false;

                //reinitialize form
                GetCustomers();
                cbCustomers.Focus();
            }
            catch (Exception)
            {
                ErrMsg.Content = "An error occurred while clearing the form. Please exit and reload.";
            }

        }

        private void btExit_Click(object sender, RoutedEventArgs e)
        {
            MainMenu mainMenu = new MainMenu(userId);
            mainMenu.Show();
            this.Close();
        }

        private void cbCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private bool ValidateForm()
        {
            try
            {

                if (txFirstName.Text.ToString().Equals(string.Empty))
                {
                    ErrMsg.Content = "Please enter a value for First Name";
                    txFirstName.Focus();
                    return false;
                }
                else if (txLastName.Text.ToString().Equals(string.Empty))
                {
                    ErrMsg.Content = "Please enter a value for Last Name";
                    txLastName.Focus();
                    return false;
                }
                else if (txAddress.Text.ToString().Equals(string.Empty))
                {
                    ErrMsg.Content = "Please enter a value for Street Address";
                    txAddress.Focus();
                    return false;
                }
                else if (txCity.Text.ToString().Equals(string.Empty))
                {
                    ErrMsg.Content = "Please enter a value for City";
                    txCity.Focus();
                    return false;
                }
                else if (txState.Text.ToString().Equals(string.Empty))
                {
                    ErrMsg.Content = "Please enter a value for State.";
                    txState.Focus();
                    return false;
                }
                else if (txState.Text.Length != 2)
                {
                    ErrMsg.Content = "Please enter the 2 letter abbreviation for the state";
                    txState.Focus();
                    return false;
                }
                else if (txZipCode.Text.ToString().Equals(string.Empty))
                {
                    ErrMsg.Content = "Please enter a value for Zip Code";
                    txZipCode.Focus();
                    return false;
                }
                else if (txIdentifier.Text.ToString().Equals(string.Empty))
                {
                    ErrMsg.Content = "Please enter a value for Identifier";
                    txIdentifier.Focus();
                    return false;
                }
                else if (txPhone.Text.Length != 0 && txPhone.Text.Length != 10)
                {
                    ErrMsg.Content = "Please enter a valid phone number or leave blank";
                    txPhone.Focus();
                    return false;
                }

                return true;

            }
            catch (Exception)
            {
                ErrMsg.Content = "An error occurred while validating this page. Please try again.";
                return false;
            }
        }
    }
}
