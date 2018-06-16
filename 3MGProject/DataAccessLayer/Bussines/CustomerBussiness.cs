using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DataAccessLayer.Bussines
{
    public class CustomerBussiness : Autherization
    {
        public CustomerBussiness() : base(typeof(CustomerBussiness)) { }


        [Authorize("Operational")]
        public List<customer> GetCustomersDeposites()
        {
            if (User.UserCanAccess(MethodBase.GetCurrentMethod()))
            {
                using (var db = new OcphDbContext())
                {
                    CustomerType ct = CustomerType.Deposit;
                    return db.Customers.Where(O => O.CustomerType == ct).ToList();
                }
            }
            else
                throw new SystemException(NotHaveAccess);

        }
        
        public void AddNewCustomerDeposit(customer cust)
        {
            using (var db = new OcphDbContext())
            {
               if(cust.Id<=0)
                {
                    cust.Id = db.Customers.InsertAndGetLastID(cust);
                    if (cust.Id <= 0)
                        throw new SystemException("Data Tidak Tersimpan");
                }else
                {
                    if(!db.Customers.Update(O=> new
                    {
                        O.Address,
                        O.ContactName,
                        O.Email,
                        O.Handphone,
                        O.Name,
                        O.Phone1,
                        O.Phone2
                    }, cust, O => O.Id == cust.Id))
                    {
                        throw new SystemException("Data Tidak Tersimpan");
                    }
                        
                }
            }
        }

        public List<deposit> GetDepositsOfCustomer(customer value)
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    var datas = db.Deposit.Where(O => O.CustomerId == value.Id).ToList();
                    return datas;
                }
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
        }
    }
}
