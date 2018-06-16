using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataModels
{

    [TableName("userinrole")]
    public class userinrole : BaseNotify
    {
        [DbColumn("UsersId")]
        public int UsersId
        {
            get { return _usersid; }
            set
            {

                SetProperty(ref _usersid, value);
            }
        }

        [DbColumn("rolesId")]
        public int rolesId
        {
            get { return _rolesid; }
            set
            {

                SetProperty(ref _rolesid, value);
            }
        }

        private int _usersid;
        private int _rolesid;
    }
}
