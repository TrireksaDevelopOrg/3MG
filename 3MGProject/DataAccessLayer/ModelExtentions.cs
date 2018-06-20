using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
  internal static  class ModelExtentions
    {
        internal static customer ToModel(this customer cust)
        {
            return new customer
            {
                Address = cust.Address,
                ContactName = cust.ContactName,
                CreatedDate = cust.CreatedDate,
                Email = cust.Email,
                Handphone = cust.Handphone,
                Id = cust.Id,
                Name = cust.Name,
                Phone1 = cust.Phone1,
                NoIdentitas = cust.NoIdentitas
            };
        }
    }
}
