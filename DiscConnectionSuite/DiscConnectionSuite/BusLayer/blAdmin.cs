using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscConnectionSuite.BusLayer
{
    class blAdmin
    {
        public static DataTable GetCustomers()
        {
            return DataAccess.daAdmin.GetCustomers();
        }

        public static DataTable GetBlacklistedCustomers()
        {
            return DataAccess.daAdmin.GetBlacklistedCustomers();
        }

        public static DataTable GetAllCustomers()
        {
            return DataAccess.daAdmin.GetAllCustomers();
        }

        public static bool SaveCustomer(string customerId, string fName, string lName, string address, string city, string state, string zipCode, string phone, string identifier, bool blacklist)
        {
            return DataAccess.daAdmin.SaveCustomer(customerId, fName, lName, address, city, state, zipCode, phone, identifier, blacklist);
        }

        public static DataTable GetSingleCustomer(string fName, string lName)
        {
            return DataAccess.daAdmin.GetSingleCustomer(fName, lName);
        }
    }
}
