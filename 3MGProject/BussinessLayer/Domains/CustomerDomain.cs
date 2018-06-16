using BussinessLayer.Interfaces;
using BussinessLayer.Models;
using System;
using System.Collections.Generic;

namespace BussinessLayer.Domains
{
    public class CustomerDomain 
    {

        public CustomerDomain() { }

        public List<ICustomer> GetAll()
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

        public List<Deposit> GetDeposites(int customerId)
        {
            var customer = GetCustomerById(customerId);
            if (customer != null)
                return customer.GetDeposites();
            else
                throw new SystemException("Customer Tidak Ditemukan");
        }

        public ICustomer GetCustomerById(int customerId)
        {
            var item = DataAccessLayer.Proxy.Customer.GetCustomerById(customerId);
            return new Customer
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
            };
        }

        public ICustomer SaveChange(Customer cust)
        {
            cust.SaveChanged();
            return cust;
        }

        public ICustomer CreateEmptyCustomer()
        {
            return new Customer();
        }

        public void Delete(int id)
        {
            DataAccessLayer.Proxy.Customer.Delete(id);
        }
    }
}
