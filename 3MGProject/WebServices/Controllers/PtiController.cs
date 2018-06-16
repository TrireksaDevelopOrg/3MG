using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebServices.Controllers
{
    public class PtiController : ApiController
    {
        // GET: api/Pti
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Pti/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Pti
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Pti/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Pti/5
        public void Delete(int id)
        {
        }
    }
}
