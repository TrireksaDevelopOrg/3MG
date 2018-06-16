using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DataModels;
using Ocph.DAL;

namespace DataAccessLayer.Bussines
{
    public class Authorization:BaseNotify
    {

        public Authorization(Type member)
        {
            var me = member.GetMethods();

        }
        public string NotHaveAccess { get { return "Anda Tidak Memiliki Akses"; } }
        public static user User { get; internal set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute
    {
        public List<string> Roles = new List<string>();
        public AuthorizeAttribute(string v)
        {
            var result = v.Split(',');
            foreach (var item in result)
            {
                Roles.Add(item);
            }
        }
    }

}
