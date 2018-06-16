using DataAccessLayer.DataModels;
using DataAccessLayer.Models;
using MySql.Data.MySqlClient;
using Ocph.DAL;
using Ocph.DAL.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Bussines
{
    public class PtiBussines:Authorization
    {
        public PtiBussines() : base(typeof(PtiBussines)) { }

        [Authorize("Admin")]
        public void SaveChange(pti item)
        {
            DateTime da = DateTime.Now;
            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {
                    if (User.CanAccess(MethodBase.GetCurrentMethod()))
                    {
                        if (item.Id <= 0)
                        {
                            if (item.Shiper.Id <= 0)
                            {
                                item.Shiper.CreatedDate = da;
                                item.ShiperID = db.Customers.InsertAndGetLastID(item.Shiper);
                                item.Shiper.Id = item.ShiperID;
                                if (item.ShiperID <= 0)
                                    throw new SystemException("Data Tidak Tersimpan");
                                var his = User.GenerateHistory(item.ShiperID, BussinesType.Customer, ChangeType.Create, "");
                                db.Histories.Insert(his);
                            }
                            else
                                item.ShiperID = item.Shiper.Id;

                            if (item.Reciever.Id <= 0)
                            {
                                item.Reciever.CreatedDate = da;
                                item.RecieverId = db.Customers.InsertAndGetLastID(item.Reciever);
                                item.Reciever.Id = item.RecieverId;
                                if (item.RecieverId <= 0)
                                    throw new SystemException("Data Tidak Tersimpan");
                                var his = User.GenerateHistory(item.RecieverId, BussinesType.Customer, ChangeType.Create, "");
                                db.Histories.Insert(his);
                            }
                            else
                                item.RecieverId = item.Reciever.Id;


                            
                            item.Id = db.PTI.InsertAndGetLastID(item);
                            if (item.Id <= 0)
                                throw new SystemException("Data Tidak Tersimpan");


                            foreach (var data in item.Collies)
                            {
                                data.PtiId = item.Id;
                                data.Id = db.Collies.InsertAndGetLastID(data);
                                if (data.Id <= 0)
                                    throw new SystemException("Data Tidak Tersimpan");
                            }

                            var history = User.GenerateHistory(item.Id, BussinesType.PTI, ChangeType.Create, "");
                            db.Histories.Insert(history);
                            trans.Commit();

                        }
                        else
                        {
                            item.Id = db.PTI.InsertAndGetLastID(item);
                            if (item.Id < 0)
                                throw new SystemException("Data Tidak Tersimpan");
                        }
                    }else
                    throw new SystemException(NotHaveAccess);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new SystemException(ex.Message);
                }
            }
        }

        public Task<List<collies>> GetColliesOutOfSMU(int id)
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "PTIColliesOutSMU";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("PtiId", id));
                    var reader = cmd.ExecuteReader();
                    var list = MappingProperties<collies>.MappingTable(reader);
                    return Task.FromResult(list);
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<List<PTI>> GetPTIFromTo(DateTime startDate, DateTime endDate)
        {
            var list = new List<PTI>();
            try
            {
                using (var db = new OcphDbContext())
                {

                    var sp = string.Format("PtiFromTo");
                    var cmd = db.CreateCommand();
                  
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("startDate",startDate));
                    cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("endDate", endDate));
                    var dr = cmd.ExecuteReader();
                    list = MappingProperties<PTI>.MappingTable(dr);
                    dr.Close();

                    return Task.FromResult(list);
                }
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
        }
    }
}
