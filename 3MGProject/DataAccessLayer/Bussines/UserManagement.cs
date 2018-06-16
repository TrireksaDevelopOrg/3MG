using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Bussines.Autherization;

namespace DataAccessLayer.Bussines
{
     public  class UserManagement
    {
        public bool IsFirstUser()
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    var result = false;
                    var res = db.Users.Select().Count();
                    if (res <= 0)
                        result = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> CreateNewUser(user r)
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    r.Id = db.Users.InsertAndGetLastID(r);
                    var result = false;
                    if (r.Id > 0)
                        result =true;

                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> IsRoleExist(string v)
        {
            using (var db = new OcphDbContext())
            {
                var exist = false;
                var result = db.Roles.Where(O => O.Name == v).FirstOrDefault();
                if (result != null)
                    exist=true;

               return Task.FromResult(exist);
            }
        }

        public Task<bool> AddNewRole(string role)
        {
            using (var db = new OcphDbContext())
            {
                var exist = db.Roles.Insert(new DataModels.role { Name = role });
                return Task.FromResult(exist);
            }
        }

        public Task<user> Login(string userName,string password)
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    var result = db.Users.Where(O => O.UserName == userName && O.Password == password).FirstOrDefault();
                    Autherization.User = result;
                    return Task.FromResult(result);

                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }


        public Task<bool> AddUserInRole(int userId, string roleName)
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    var isSaved = false;
                    var result = db.Roles.Where(O => O.Name == roleName).FirstOrDefault();
                    if(result!=null)
                    {
                        userinrole ur = new userinrole { rolesId = result.Id, UsersId = userId };
                        if (db.UserRoles.Insert(ur))
                            isSaved = true;
                    }
                    return Task.FromResult(isSaved);
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }


        public Task<bool> RemoveUserInRole(int userId, string roleName)
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    var isSaved = false;
                    var result = db.Roles.Where(O => O.Name == roleName).FirstOrDefault();
                    if (result != null)
                    {
                        var ur = db.UserRoles.Where(O => O.UsersId == userId && O.rolesId == result.Id).FirstOrDefault();
                        if (ur!=null && db.UserRoles.Delete(O=>O.rolesId==result.Id && O.UsersId==userId))
                            isSaved = true;
                    }
                    return Task.FromResult(isSaved);
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }







    }


    public static class UserManagemnetExtention
    {
        public static Task<bool> InRole(this user u,string roleName)
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    var userRoles = from a in db.UserRoles.Where(O => O.UsersId == u.Id)
                                    join r in db.Roles.Select() on a.rolesId equals r.Id
                                    select r;
                    if (userRoles.Where(O => O.Name.ToUpper().Equals(roleName.ToUpper())).Count() > 0)
                        return Task.FromResult(true);
                    else
                        return Task.FromResult(false);

                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public static Task<List<role>> Roles(this user u)
        {
            using (var db = new OcphDbContext())
            {
                var userRoles = from a in db.UserRoles.Where(O => O.UsersId == u.Id)
                                join r in db.Roles.Select() on a.rolesId equals r.Id
                                select r;
                return Task.FromResult(userRoles.ToList());
            }
        }

        public static bool UserCanAccess(this user user,MethodBase method)
        {
            AuthorizeAttribute attr = (AuthorizeAttribute)method.GetCustomAttributes(typeof(AuthorizeAttribute), true)[0];
            var values = attr.Roles;
            var roles = User.Roles();

            var haveAccess = false;
            foreach (var item in values)
            {
                var result = User.InRole(item).Result;
                if (result)
                {
                    haveAccess = true;
                    break;
                }
            }

            return haveAccess;
        }

    }

}
