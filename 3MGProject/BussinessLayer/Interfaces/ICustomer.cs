using BussinessLayer.Models;
using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BussinessLayer.Interfaces
{
    public interface ICustomer:Icustomer
    {
        void SaveChanged();
        void Delete();
        List<Deposit> GetDeposites();
    }
}
