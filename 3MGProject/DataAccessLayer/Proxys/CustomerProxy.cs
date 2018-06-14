using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DataModels;

namespace DataAccessLayer.Proxys
{
    public class CustomerProxy
    {
        public List<Icustomer> GetAllCustomer()
        {

            using (var db = new OcphDbContext())
            {
                try
                {
                    var result = db.Customers.Select();
                    return result.ToList<Icustomer>();
                }
                catch (Exception ex)
                {
                    throw new SystemException(ex.Message);
                }
            }
        }
    }
}
