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
    public class daPurchase
    {
        private static string connString = ConfigurationManager.ConnectionStrings["DisConnConnectionString"].ToString();

        public static DataTable GetCustomers()
        {
            SqlConnection conn = new SqlConnection(connString);

            try {
                
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

            } catch(Exception) {
                return new DataTable();
            } finally {
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

        public static DataTable CustomerSearch(string identifier)
        {
            SqlConnection conn = new SqlConnection(connString);

            try {
                //Create command
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.CommandText = "dcCustomerSearch";

                //add parameters
                cmd.Parameters.AddWithValue("@Identifier", identifier);

                //Get data
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                //Return data
                return dt;
            } catch(Exception) {
                return new DataTable();
            } finally {
                conn.Close();
            }
        }

        public static int NewCustomer(string fName, string lName, string address, string city, string state, string zipCode, string phone, string identifier)
        {
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                //Create command
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.CommandText = "dcCreateCustomer";

                //add parameters
                cmd.Parameters.AddWithValue("@FName", fName);
                cmd.Parameters.AddWithValue("@LName", lName);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@City", city);
                cmd.Parameters.AddWithValue("@State", state);
                cmd.Parameters.AddWithValue("@ZipCode", zipCode);
                if(phone.Equals(string.Empty)) {
                    cmd.Parameters.AddWithValue("@Phone", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Phone", phone);
                }
                cmd.Parameters.AddWithValue("@Identifier", identifier);

                //Get data
                conn.Open();

                int returnValue = Convert.ToInt32(cmd.ExecuteScalar());

                return returnValue;
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }

        public static string CreateBPD(string userId, string customerId)
        {
            SqlConnection conn = new SqlConnection(connString);
            try {
                //Create command
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.CommandText = "dcCreateBPD";

                //add parameters
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
                cmd.Parameters.AddWithValue("@UserName", userId);

                //Get data
                conn.Open();

                string returnValue = cmd.ExecuteScalar().ToString();

                return returnValue;
            }
            catch (Exception)
            {
                return string.Empty;
            }
            finally
            {
                conn.Close();
            }
        }

        public static bool SaveCustomer(string customerId, string fName, string lName, string address, string city, string state, string zipCode, string phone, string identifier)
        {

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                //Create command
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.CommandText = "dcUpdateCustomer";

                //add parameters
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
                cmd.Parameters.AddWithValue("@FName", fName);
                cmd.Parameters.AddWithValue("@LName", lName);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@City", city);
                cmd.Parameters.AddWithValue("@State", state);
                cmd.Parameters.AddWithValue("@ZipCode", zipCode);
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

        public static bool AddItems(string bc, string title, string serial, string dcPrice, string bpd) {
            try {
                SqlConnection conn = new SqlConnection(connString);

                try {
                    //Create command
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    cmd.CommandText = "dcAddItems";

                    //add parameters
                    cmd.Parameters.AddWithValue("@BC", bc);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Serial", serial);
                    cmd.Parameters.AddWithValue("@DCPrice", dcPrice);
                    cmd.Parameters.AddWithValue("@BPDID", bpd);

                    //get data
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    return true;

                } catch(Exception) {
                    return false;
                } finally {
                    conn.Close();
                }
            } catch(Exception) {
                return false;
            }
        }

        public static DataTable GetReport(string bpd)
        {
            SqlConnection conn = new SqlConnection(connString);

            try {

                //Create command
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.CommandText = "dcReportData";

                //add parameters
                cmd.Parameters.AddWithValue("@BPDID", bpd);

                //Get data
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                //Return data
                return dt;

            } catch(Exception) {
                return new DataTable();
            } finally {
                conn.Close();
            }
        }

        public static DataTable CheckItemBlacklist(string asin)
        {
            SqlConnection conn = new SqlConnection(connString);

            try
            {

                //Create command
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.CommandText = "dcCheckItemBlacklist";

                //add parameters
                cmd.Parameters.AddWithValue("@ASIN", asin);

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