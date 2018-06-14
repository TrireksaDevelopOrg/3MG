using DataAccessLayer.Proxys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class Proxy
    {
        private static CustomerProxy _customer;

        public static CustomerProxy Customer
        {
            get
            {
                if (_customer == null)
                    _customer = new CustomerProxy();
                return _customer;
            }
        }




    }
}
