using DataAccessLayer.Models;
using MySql.Data.MySqlClient;
using Ocph.DAL.Mapping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Bussines
{
    public class LaporanBussines:Authorization
    {
        public LaporanBussines():base(typeof(LaporanBussines))
        {

        }

        public Task<List<Penjualan>> GetDataLaporan(DateTime start, DateTime end)
        {

            using (var db = new OcphDbContext())
            {
                try
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "Penjualan";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("Dari", start));
                    cmd.Parameters.Add(new MySqlParameter("Sampai", end));
                    var reader = cmd.ExecuteReader();
                    var result = MappingProperties<Penjualan>.MappingTable(reader);
                    return Task.FromResult(result);
                }
                catch (Exception ex )
                {

                    throw new SystemException(ex.Message);
                }
            }
        }
    }
}