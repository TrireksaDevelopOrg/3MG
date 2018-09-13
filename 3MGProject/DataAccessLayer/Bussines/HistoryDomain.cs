using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using MySql.Data.MySqlClient;
using Ocph.DAL.Mapping;

namespace DataAccessLayer.Bussines
{
    public class HistoryDomain
    {
        public Task<List<History>> GetPTIHistory(PTI selectedPTI)
        {
            List<History> list = null;
            try
            {
                using (var db = new OcphDbContext())
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "PTIHistories";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("id", selectedPTI.Id));
                    var reader = cmd.ExecuteReader();
                    list = MappingProperties<History>.MappingTable(reader);
                    return Task.FromResult(list);
                }
            }
            catch (Exception)
            {
                return Task.FromResult(list);
            }
        }
    }
}
