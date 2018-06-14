using ModelShared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelShared.Models
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

        public bool SaveChanged()
        {
            throw new NotImplementedException();
        }
    }
}
