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
                        if (item.Shiper.Id <= 0)
                        {
                            item.Shiper.CreatedDate = da;
                            item.ShiperID = db.Customers.InsertAndGetLastID(item.Shiper);
                            item.Shiper.Id = item.ShiperID;
                            if (item.ShiperID <= 0)
                                throw new SystemException("Data Tidak Tersimpan");
                            var his = User.GenerateHistory(item.ShiperID, BussinesType.Customer, ChangeType.Create, "Menambah Shiper");
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



                        if (!db.PTI.Insert(item))
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
                    throw new SystemException(NotHaveAccess);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new SystemException(ex.Message);
                }
            }
        }

        public Task<List<collies>> GetDetailOfPTI(PTI selected)
        {
            using (var db = new OcphDbContext())
            {
                var result = db.Collies.Where(O => O.PtiId == selected.Id).ToList();
                return Task.FromResult(result);
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

        public Task<List<PTI>> GetPTIForPrint(int id)
        {
            var list = new List<PTI>();
            try
            {
                using (var db = new OcphDbContext())
                {

                    var sp = string.Format("PTIFORPRINT");
                    var cmd = db.CreateCommand();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("id", id));
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

        public Task<bool> SetCancelPTI(PTI selectedPTI,string alasan)
        {
            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {

                    if (selectedPTI.OnSMU)
                    {
                        ActivedStatus ac = ActivedStatus.Cancel;
                        var result = db.SMU.Where(O => O.PTIId == selectedPTI.Id && O.ActiveStatus!=ac ).FirstOrDefault();
                        if(result!=null)
                            throw new SystemException("PTI Telah Dibuatkan SMU, Batalkan SMU Lebih Dulu");
                    }
                      
                    ActivedStatus active = ActivedStatus.Cancel;
                    var updated = db.PTI.Update(O => new { O.ActiveStatus }, new pti { Id = selectedPTI.Id, ActiveStatus = active }, O => O.Id == selectedPTI.Id);
                    if (updated)
                    {
                        selectedPTI.ActiveStatus = ActivedStatus.Cancel;
                        var his = User.GenerateHistory(selectedPTI.Id, BussinesType.PTI, ChangeType.Cancel, alasan);
                        if(!db.Histories.Insert(his))
                            throw new SystemException("Gagal Diubah");

                        trans.Commit();

                        return Task.FromResult(true);
                    }
                    else
                    {
                        throw new SystemException("Gagal Diubah");
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new SystemException(ex.Message);
                }

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

        [Authorize("Manager")]
        public collies UpdateCollies(collies colly)
        {
            collies item=null;
            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {
                    item = db.Collies.Where(O => O.Id == colly.Id).FirstOrDefault();
                    if (item != null)
                    {
        
                        if (UpdateCollieValidate(item,colly) && db.Collies.Update(O => new { O.Content, O.Pcs, O.Kemasan, O.Price, O.Weight }, colly, O => O.Id == colly.Id))
                        {
                            var note = string.Format(@"Mengubah Item :  \n\r {0}-{1}-{2}-{3} \r\n Ke {4}-{5}-{6}-{7}", 
                                item.Content,item.Pcs,item.Weight,item.Price,
                                colly.Content,colly.Pcs,colly.Weight,colly.Price);
                            var his = User.GenerateHistory(colly.PtiId, BussinesType.PTI, ChangeType.Update, note);
                            if (db.Histories.Insert(his))
                            {
                                trans.Commit();
                                return null;
                            }
                        }
                    }
                        throw new SystemException();

                }
                catch (Exception)
                {
                    trans.Rollback();
                    return item;
                }
            }

        }

        private bool UpdateCollieValidate(collies item, collies colly)
        {
            if (item.Content!=colly.Content || item.Pcs!=colly.Pcs || item.Weight!=colly.Weight || item.Price!=colly.Price)
                return true;
            return false;
                
        }

        public void AddNewCollyItem(collies selectedRow)
        {
            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {
                    selectedRow.Id = db.Collies.InsertAndGetLastID(selectedRow);
                    if (selectedRow.Id > 0)
                    {
                        var item = selectedRow;
                        var note = string.Format(@"Menambah Item Baru: \n\r {0}-{1}-{2}-{3}",
                                   item.Content, item.Pcs, item.Weight, item.Price);
                        var his = User.GenerateHistory(selectedRow.PtiId, BussinesType.PTI, ChangeType.Update, note);
                        if (db.Histories.Insert(his))
                            trans.Commit();
                        else
                            throw new SystemException("Data Tidak Tersimpan");
                    }
                    else
                        throw new SystemException("Data Tidak Tersimpan");

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new SystemException(ex.Message);
                }
            }
        }

        public bool RemoveItemCollies(collies collies)
        {

            using (var db = new OcphDbContext())
            {

                var trans = db.BeginTransaction();
                try
                {
                    if(db.Collies.Delete(O=>O.Id==collies.Id))
                    {
                        var item = collies;
                        var note = string.Format(@"Menghapus Item : \n\r {0}-{1}-{2}-{3}",
                                   item.Content, item.Pcs, item.Weight, item.Price);
                        var his = User.GenerateHistory(collies.PtiId, BussinesType.PTI, ChangeType.Delete, note);
                        if (db.Histories.Insert(his))
                        {
                            trans.Commit();
                            return true;
                        }
                    }
                    throw new SystemException("Data Tidak Terhapus");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    if (ex.Message == "Ada Relasi Dengan Table Lain")
                        throw new SystemException("Item Telah Terdaftar Di SMU, Hapus Terlebih Dahulu Di SMU");
                    throw new SystemException(ex.Message);
                }
            }
        }
    }
}
