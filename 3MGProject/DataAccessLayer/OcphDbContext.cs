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
   public class OcphDbContext:MySqlDbConnection
    {
        private string stringConnection = "Server=localhost;database=trimgpenjualan;UID=root;password=;CharSet=utf8;Persist Security Info=True";
        public OcphDbContext()
        {
            this.ConnectionString = stringConnection;
        }

        internal IRepository<customer> Customers => new Repository<customer>(this);

    }
}
