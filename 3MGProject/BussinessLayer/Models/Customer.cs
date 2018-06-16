using BussinessLayer.Interfaces;
using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BussinessLayer.Models
{
    public class Customer : ICustomer
    {
        public int Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ContactName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Phone1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Phone2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Handphone { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Address { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Email { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime CreatedDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void SaveChanged()
        {
            try
            {
                DataAccessLayer.Proxy.Customer.SaveChange(this);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public void Delete()
        {
            try
            {
                DataAccessLayer.Proxy.Customer.Delete(this.Id);
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
        }

        public List<Deposit> GetDeposites()
        {
            throw new NotImplementedException();
        }

        
    }
}