using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
  public  class Proxy<T>
    {
        public Proxy()
        {
            this.Type = typeof(T);
        }
        public Type Type { get; }



    }
}
