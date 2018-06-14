using BussinessLayer.Interfaces;
using ModelShared.Interfaces;
using ModelShared.Models;
using System;
using System.Collections.Generic;

namespace BussinessLayer.Domains
{
    public class CustomerDomain : ICustomerDomain
    {

        public CustomerDomain() { }

        public CustomerDomain(ICustomer customer)
        {
            Customer = customer;
        }


        public List<ICustomer> GetAllCustomer()
        {
            try
            {
                var list = new List<ICustomer>();
               var result= DataAccessLayer.Proxy.Customer.GetAllCustomer();
                foreach(var item in result)
                {
                    list.Add(new Customer
                    {
                        Address = item.Address,
                        ContactName = item.ContactName,
                        CreatedDate = item.CreatedDate,
                        Email = item.Email,
                        Handphone = item.Handphone,
                        Id = item.Id,
                        Name = item.Name,
                        Phone1 = item.Phone1,
                        Phone2 = item.Phone2
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
          
        }

        public ICustomer CreateCustomer()
        {
            throw new NotImplementedException();
        }

        public ICustomer Customer { get; }
    }
}
