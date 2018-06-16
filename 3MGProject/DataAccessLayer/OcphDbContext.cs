using Ocph.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL.Provider.MySql;
using DataAccessLayer.DataModels;

namespace DataAccessLayer
{
   internal class OcphDbContext:MySqlDbConnection
    {
        private string stringConnection = "Server=localhost;database=trimgpenjualan;UID=root;password=;CharSet=utf8;Persist Security Info=True";
        public OcphDbContext()
        {
            this.ConnectionString = stringConnection;
        }

        internal IRepository<customer> Customers => new Repository<customer>(this);
        internal IRepository<pti> PTI=> new Repository<pti>(this);
        internal IRepository<collies> Collies=> new Repository<collies>(this);
        internal IRepository<deposit> Deposit => new Repository<deposit>(this);
        internal IRepository<debetdeposit> DebetDeposit=> new Repository<debetdeposit>(this);
        internal IRepository<smu> SMU=> new Repository<smu>(this);
        internal IRepository<smudetails> SMUDetails => new Repository<smudetails>(this);
        internal IRepository<user> Users=> new Repository<user>(this);
        internal IRepository<role> Roles=> new Repository<role>(this);
        internal IRepository<userinrole> UserRoles=> new Repository<userinrole>(this);

    }
}
