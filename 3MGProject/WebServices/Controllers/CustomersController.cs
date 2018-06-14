using BussinessLayer.Domains;
using ModelShared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebServices.Controllers
{
    public class CustomersController : ApiController
    {
        // GET: api/Customers
        public IEnumerable<ICustomer> Get()
        {
            var context = new CustomerDomain();

            return context.GetAllCustomer();


        }

        // GET: api/Customers/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Customers
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Customers/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Customers/5
        public void Delete(int id)
        {
        }
    }
}
