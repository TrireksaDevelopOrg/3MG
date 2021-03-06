﻿using System;
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
        public static user User { get; set; }

        public string GetUser()
        {
            return User.Name;
        }

        public Task<bool> IsInRole(string role)
        {
            return User.InRole(role);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute
    {
        public List<string> Roles = new List<string>();
        public AuthorizeAttribute(string v)
        {
            if (v.Contains(','))
            {
                var result = v.Split(',');
                foreach (var item in result)
                {
                    Roles.Add(item);
                }
            }else
            {
                Roles.Add(v);
            }
        }

        public AuthorizeAttribute(string[] roles)
        {
            this.Roles = roles.ToList();
        }
    }

}
