using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelShared.Interfaces
{
    public interface ICustomer : Icustomer
    {
        bool SaveChanged();


    }
}
