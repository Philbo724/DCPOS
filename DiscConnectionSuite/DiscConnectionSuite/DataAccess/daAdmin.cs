using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscConnectionSuite.DataAccess
{
    class daAdmin
    {

        private static string connString = ConfigurationManager.ConnectionStrings["DisConnConnectionString"].ToString();

        public static DataTable GetCustomers()
        {
            SqlConnection conn = new SqlConnection(connString);

            try
            {

                //Create command
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.CommandText = "dcGetCustomers";

                //Get data
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                //Return data
                return dt;

            }
            catch (Exception)
            {
                return new DataTable();
            }
            finally
            {
                conn.Close();
            }
        }

        public static DataTable GetBlacklistedCustomers()
        {
            SqlConnection conn = new SqlConnection(connString);

            try
            {

                //Create command
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.CommandText = "dcGetBlacklistedCustomers";

                //Get data
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                //Return data
                return dt;

            }
            catch (Exception)
            {
                return new DataTable();
            }
            finally
            {
                conn.Close();
            }
        }

        public static DataTable GetAllCustomers()
        {
            SqlConnection conn = new SqlConnection(connString);

            try
            {

                //Create command
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.CommandText = "dcGetAllCustomers";

                //Get data
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                //Return data
                return dt;

            }
            catch (Exception)
            {
                return new DataTable();
            }
            finally
            {
                conn.Close();
            }
        }

        public static bool SaveCustomer(string customerId, string fName, string lName, string address, string city, string state, string zipCode, string phone, string identifier, bool blacklist)
        {

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                //Create command
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.CommandText = "dcAdminUpdateCustomer";

                //add parameters
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
                cmd.Parameters.AddWithValue("@FName", fName);
                cmd.Parameters.AddWithValue("@LName", lName);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@City", city);
                cmd.Parameters.AddWithValue("@State", state);
                cmd.Parameters.AddWithValue("@ZipCode", zipCode);
                if (blacklist)
                {
                    cmd.Parameters.AddWithValue("@Blacklist", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Blacklist", 0);
                }
                

                if (phone.Equals(string.Empty))
                {
                    cmd.Parameters.AddWithValue("@Phone", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Phone", phone);
                }
                cmd.Parameters.AddWithValue("@Identifier", identifier);

                //get data
                conn.Open();
                cmd.ExecuteNonQuery();

                return true;

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public static DataTable GetSingleCustomer(string fName, string lName)
        {
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                //Create command
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.CommandText = "dcGetSingleCustomer";

                //Add parameters
                cmd.Parameters.AddWithValue("@FName", fName);
                cmd.Parameters.AddWithValue("@LName", lName);

                //Get data
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                //Return data
                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
            finally
            {
                conn.Close();
            }

        }
    }
}
