using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscConnectionSuite.BusLayer
{
    class blPurchase
    {
        public static DataTable GetCustomers()
        {
            return DataAccess.daPurchase.GetCustomers();
        }

        public static DataTable GetSingleCustomer(string fName, string lName)
        {
            return DataAccess.daPurchase.GetSingleCustomer(fName, lName);
        }

        public static DataTable CustomerSearch(string identifier)
        {
            return DataAccess.daPurchase.CustomerSearch(identifier);
        }

        public static int NewCustomer(string fName, string lName, string address, string city, string state, string zipCode, string phone, string identifier)
        {
            return DataAccess.daPurchase.NewCustomer(fName, lName, address, city, state, zipCode, phone, identifier);
        }

        public static string CreateBPD(string userId, string customerId)
        {
            return DataAccess.daPurchase.CreateBPD(userId, customerId);
        }

        public static bool SaveCustomer(string customerId, string fName, string lName, string address, string city, string state, string zipCode, string phone, string identifier)
        {
            return DataAccess.daPurchase.SaveCustomer(customerId, fName, lName, address, city, state, zipCode, phone, identifier);
        } 

        public static bool AddItems(string bc, string title, string serial, string dcPrice, string bpd) {
            return DataAccess.daPurchase.AddItems(bc, title, serial, dcPrice, bpd);
        }

        public static DataTable GetReport(string bpd)
        {
            return DataAccess.daPurchase.GetReport(bpd);
        }

        public static DataTable CheckItemBlacklist(string asin)
        {
            return DataAccess.daPurchase.CheckItemBlacklist(asin);
        }
    }
}