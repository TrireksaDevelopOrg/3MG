using BussinessLayer.Domains;
using BussinessLayer.Interfaces;
using BussinessLayer.Models;
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
        CustomerDomain context = new CustomerDomain();
        // GET: api/Customers
        public IEnumerable<ICustomer> Get()
        {
            return context.GetAll();
        }

        [Route("api/Customer/GetDeposites/{CustomerId}")]
        public IHttpActionResult GetDeposites(int CustomerId)
        {
            try
            {
               var result= context.GetDeposites(CustomerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Customers
        public IHttpActionResult Post([FromBody]Customer cust)
        {
            try
            {
               ICustomer customer= context.SaveChange(cust);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Customers/5
        public IHttpActionResult Put(int id, [FromBody]Customer cust)
        {
            try
            {
                ICustomer customer = context.SaveChange(cust);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // DELETE: api/Customers/5
        public IHttpActionResult Delete(int id)
        {
            try
            {
                context.Delete(id);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
