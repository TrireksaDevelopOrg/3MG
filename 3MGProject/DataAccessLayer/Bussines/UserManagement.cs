using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Bussines.Authorization;

namespace DataAccessLayer.Bussines
{
    public class UserManagement:Authorization
    {
        public UserManagement():base(typeof(UserManagement))
        {

        }
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
                        result = true;

                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public List<user> GetUsers()
        {
            using (var db = new OcphDbContext())
            {
                return db.Users.Select().ToList();
            }   
        }

        public List<role> GetRoles()
        {
            using (var db = new OcphDbContext())
            {
                return db.Roles.Select().ToList();
            }
        }

        public Task<bool> IsRoleExist(string v)
        {
            using (var db = new OcphDbContext())
            {
                var exist = false;
                var result = db.Roles.Where(O => O.Name == v).FirstOrDefault();
                if (result != null)
                    exist = true;

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

        public Task<user> Login(string userName, string password)
        {
            try
            {
              //  userName = "Ocph23";
                //password = "Sony@77";
                using (var db = new OcphDbContext())
                {
                    var result = db.Users.Where(O => O.UserName == userName && O.Password == password).FirstOrDefault();
              
                    return Task.FromResult(result);

                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public bool ChangePassword(user user, string newPassword)
        {
            using (var db = new OcphDbContext())
            {
                if (!db.Users.Update(O => new { O.Password }, new user { Password = newPassword }, O => O.Id == User.Id))
                {
                    throw new SystemException("Password Anda Tidak Berhasil Diubah");
                }

                return true;
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
                    if (result != null)
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
                        if (ur != null && db.UserRoles.Delete(O => O.rolesId == result.Id && O.UsersId == userId))
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
        public static Task<bool> InRole(this user u, string roleName)
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

        public static bool CanAccess(this user user, MethodBase method)
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

        public static bool AddHistory(this user user, int bussinesId, BussinesType bty, ChangeType cty, string note)
        {
            var history = new changehistory
            {
                UsersId = user.Id,
                BussinessId = bussinesId,
                BussinesType = bty,
                ChangeType = cty,
                CreatedDate = DateTime.Now,
                Note = note
            };
            try
            {
                using (var db = new OcphDbContext())
                {
                    return db.Histories.Insert(history);
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static changehistory GenerateHistory(this user user, int bussinesId, BussinesType bty, ChangeType cty, string note)
        {
            var history = new changehistory
            {
                UsersId = user.Id,
                BussinessId = bussinesId,
                BussinesType = bty,
                ChangeType = cty,
                CreatedDate = DateTime.Now,
                Note = note
            };

            return history;
        }

    }
}