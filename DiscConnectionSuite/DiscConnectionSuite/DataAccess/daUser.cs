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
    public class daUser
    {
        private static string connString = ConfigurationManager.ConnectionStrings["DisConnConnectionString"].ToString();
     
        public static int ValidateUser(string username, string password)
        {
            SqlConnection conn = new SqlConnection(connString);
           
            try
            {
                //Create command
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.CommandText = "dcValidateUser";

                //Add parameters
                cmd.Parameters.AddWithValue("@User", username);
                cmd.Parameters.AddWithValue("@Password", password);

                //Get data
                conn.Open();

                int roleID = Convert.ToInt32(cmd.ExecuteScalar());

                return roleID;
            } catch (Exception) {
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
