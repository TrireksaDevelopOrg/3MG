﻿using DataAccessLayer.DataModels;
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

        public collies SplitItemPTI(PreFligtManifest selectedItemPTI, int jumlah)
        {
            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {
                    var sisa = selectedItemPTI.Pcs - jumlah;
                    if (!db.Collies.Update(O => new { O.Pcs }, new collies { Pcs = jumlah }, O => O.Id == selectedItemPTI.ColliesId))
                        throw new SystemException("Item PTI Tidak Berhasil Di Split");

                    var newItem = new collies { Content = selectedItemPTI.Content, IsSended = false, Kemasan = selectedItemPTI.Kemasan, Pcs = sisa,
                        Price = selectedItemPTI.Price, PtiId = selectedItemPTI.PTIId, Weight = selectedItemPTI.Weight };
                    newItem.Id = db.Collies.InsertAndGetLastID(newItem);
                    if(newItem.Id<=0)
                        throw new SystemException("Item PTI Tidak Berhasil Di Split");

                    var note = string.Format("Split PTI No {0:D6} item {1:D7} ({2}Colly) Menjadi {3} Colly dan {4}Colly", selectedItemPTI.PTIId,
                         selectedItemPTI.ColliesId, selectedItemPTI.Pcs, sisa, jumlah);
                    var hist = User.GenerateHistory(selectedItemPTI.PTIId, BussinesType.PTI, ChangeType.Update, note);

                    if(!db.Histories.Insert(hist))
                        throw new SystemException("Item PTI Tidak Berhasil Di Split");

                    trans.Commit();
                    return newItem;
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
                    var sp = string.Format("Ptistatus");
                    var cmd = db.CreateCommand();

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("Id", selectedPTI.Id));
                    var dr = cmd.ExecuteReader();
                    var statuses = MappingProperties<PreFligtManifest>.MappingTable(dr);

                    dr.Close();
                    if(statuses!=null && statuses.Count>0)
                    {
                        if(statuses.Count>1)
                        {
                            foreach(var item in statuses)
                            {
                                var man = db.Manifest.Where(O => O.Id == item.Id).FirstOrDefault();
                                if(man!=null && !man.IsTakeOff)
                                {
                                    if (!db.ManifestDetail.Delete(O => O.SMUId == item.SMUId))
                                    {
                                        throw new SystemException(string.Format("PTI Tidak Dapat Dibatalkan, Keluarkan SMU T{0} dari Manifest MT{1}",item.SMUId,item.Id));
                                    }
                                   
                                }
                                else if(man!=null && man.IsTakeOff)
                                {
                                    throw new SystemException(string.Format("PTI Tidak Dapat Dibatalkan, \r\n PTI telah diberangkatkan dengan Manifest MT{0:D8} \r\n dan SMU T{1:D9}",item.Id, item.SMUId));
                                }


                                if(item.SMUId>0)
                                {
                                    ActivedStatus ac = ActivedStatus.Cancel;
                                    if (!db.SMU.Update(O => new { O.ActiveStatus }, new smu { ActiveStatus = ac }, O => O.Id == item.SMUId))
                                        throw new SystemException("Tidak Dapat Membatalkan SMU");
                                    var his = User.GenerateHistory(item.SMUId, BussinesType.SMU, ChangeType.Cancel, 
                                        "System Membatalkan SMU NO "+item.SMUId+" Karena PTI  DI Batalkan : "+alasan);
                                    if (!db.Histories.Insert(his))
                                        throw new SystemException("Gagal Diubah");
                                }

                            }
                          
                        }else
                        {
                            var e = statuses.FirstOrDefault();
                            if (e!=null && e.SMUId>0 && e.Id>0)
                            {
                                var man = db.Manifest.Where(O => O.Id == e.Id).FirstOrDefault();
                                if(man!=null && man.IsTakeOff)
                                {
                                    throw new SystemException(string.Format("PTI Tidak Dapat Dibatalkan, \r\n PTI telah diberangkatkan dengan Manifest MT{0:D8} \r\n dan SMU T{1:D9}", e.Id, e.SMUId));
                                }else if(man != null && !man.IsTakeOff)
                                {
                                    if(db.ManifestDetail.Delete(O=>O.SMUId==e.SMUId))
                                    {
                                        var his = User.GenerateHistory(man.Id, BussinesType.Manifest, ChangeType.Update,
                                                "System Membatalkan SMU NO " + e.SMUId + " Dari Manifest No :"+man.Id+" Karena PTI  DI Batalkan : " + alasan);

                                        if (!db.Histories.Insert(his))
                                            throw new SystemException("Gagal Diubah");



                                        ActivedStatus ac = ActivedStatus.Cancel;
                                        if (!db.SMU.Update(O => new { O.ActiveStatus }, new smu { ActiveStatus = ac }, O => O.Id == e.SMUId))
                                            throw new SystemException("Tidak Dapat Membatalkan SMU");
                                        his = User.GenerateHistory(e.SMUId, BussinesType.SMU, ChangeType.Cancel,
                                                  "System Membatalkan SMU NO " + e.SMUId + " Karena PTI  DI Batalkan : " + alasan);
                                        if (!db.Histories.Insert(his))
                                            throw new SystemException("Gagal Diubah");

                                    }
                                }
                            }
                        }
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

        public void AddNewCollyItem(PTI selectedPTI, collies selectedRow)
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
                        if (!db.PTI.Update(O => new { O.OnSMU }, new pti { OnSMU = false }, O => O.Id == selectedPTI.Id))
                            throw new SystemException("Data Tidak Tersimpan");

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

        public bool ItemColliIsSended(collies colly)
        {
            using (var db = new OcphDbContext())
            {
                var smuDetail = db.SMUDetails.Where(O => O.colliesId == colly.Id).FirstOrDefault();
                if(smuDetail!=null)
                {
                    var manifestDetail = db.ManifestDetail.Where(O => O.SMUId == smuDetail.SMUId).FirstOrDefault();
                    if (manifestDetail != null)
                    {
                        var manifest = db.Manifest.Where(O => O.Id == manifestDetail.manifestoutgoingId).FirstOrDefault();
                        if (manifest != null && manifest.IsTakeOff)
                            return true;
                    }
                }
                return false;
            }
        }
    }
}
