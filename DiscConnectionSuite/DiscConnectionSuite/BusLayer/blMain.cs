using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;



namespace DiscConnectionSuite.BusLayer
{
    class blMain
    {
        public static int CallLogin(string username, string password)
        {
            return DataAccess.daUser.ValidateUser(username, password);
        }
    }
}
