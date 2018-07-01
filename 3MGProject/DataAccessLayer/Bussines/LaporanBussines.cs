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

        public Task<List<BorderelCargoModel>> GetDataBorderelCargo(DateTime from)
        {
            using (var db = new OcphDbContext())
            {
                try
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "BorderelCargo";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("indate", from));
                    var reader = cmd.ExecuteReader();
                    var result = MappingProperties<BorderelCargoModel>.MappingTable(reader);
                    return Task.FromResult(result);
                }
                catch (Exception ex)
                {

                    throw new SystemException(ex.Message);
                }
            }
        }

        public Task<List<PreFligtManifest>> GetDataBufferStock()
        {
            using (var db = new OcphDbContext())
            {
                try
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "bufferstock";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    var reader = cmd.ExecuteReader();
                    var result = MappingProperties<PreFligtManifest>.MappingTable(reader);
                    return Task.FromResult(result);
                }
                catch (Exception ex)
                {

                    throw new SystemException(ex.Message);
                }
            }
        }

        public Task<List<PreFligtManifest>> GetDataCargoterangkut(int siperID, DateTime from, DateTime to)
        {
            using (var db = new OcphDbContext())
            {
                try
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "terangkut";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("custId", siperID));
                    cmd.Parameters.Add(new MySqlParameter("startDate", from));
                    cmd.Parameters.Add(new MySqlParameter("endDate", to));
                    var reader = cmd.ExecuteReader();
                    var result = MappingProperties<PreFligtManifest>.MappingTable(reader);
                    return Task.FromResult(result);
                }
                catch (Exception ex)
                {

                    throw new SystemException(ex.Message);
                }
            }
        }
    }
}