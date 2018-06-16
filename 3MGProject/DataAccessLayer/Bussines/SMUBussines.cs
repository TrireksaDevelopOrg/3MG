using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DataModels;
using DataAccessLayer.Models;
using MySql.Data.MySqlClient;
using Ocph.DAL.Mapping;

namespace DataAccessLayer.Bussines
{
    public class SMUBussines
    {
        public SMUBussines()
        {
        }

        public void CreateNewSMU(PTI pTISelected)
        {
            throw new NotImplementedException();
        }

        public void CreateNewSMU(PTI pTISelected, ObservableCollection<collies> source)
        {
            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {
                    if(pTISelected.PayType!= PayType.Deposit)
                    {

                    }
                    var smudata = new smu { CreatedDate = DateTime.Now, Kode = "001" };
                    smudata.Id = db.SMU.InsertAndGetLastID(smudata);
                    if (smudata.Id <= 0)
                        throw new SystemException("Data Tidak Tersimpan");
                    
                    foreach (var item in source.Where(O=>O.IsSended))
                    {
                        var data = new smudetails { colliesId = item.Id, SMUId = smudata.Id };
                        if (!db.SMUDetails.Insert(data))
                            throw new SystemException("Data Tidak Tersimpan");

                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new SystemException(ex.Message);
                }
              
            }
        }

        public Task<List<SMU>> GetSMU(DateTime startDate, DateTime endDate)
        {

            using (var db = new OcphDbContext())
            {
                try
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "SMU";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("startDate", startDate));
                    cmd.Parameters.Add(new MySqlParameter("endDate", endDate));
                    var reader =cmd.ExecuteReader();
                    var result = MappingProperties<SMU>.MappingTable(reader);
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