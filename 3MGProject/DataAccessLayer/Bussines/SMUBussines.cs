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
    public class SMUBussines:Authorization
    {
        public SMUBussines():base(typeof(SMUBussines))
        {
        }

       

        public Task<SMU> CreateNewSMU(PTI pTISelected, ObservableCollection<collies> source)
        {
            using (var db = new OcphDbContext())
            {
                var date = DateTime.Now;
                var trans = db.BeginTransaction();
                try
                {
                    
                    var smudata = new smu {  PTIId=pTISelected.Id, CreatedDate = DateTime.Now};
                    smudata.Id = db.SMU.InsertAndGetLastID(smudata);
                    if (smudata.Id <= 0)
                        throw new SystemException("Data Tidak Tersimpan");
                    
                    foreach (var item in source.Where(O=>O.IsSended))
                    {
                        var data = new smudetails { colliesId = item.Id, SMUId = smudata.Id };
                        if (!db.SMUDetails.Insert(data))
                            throw new SystemException("Data Tidak Tersimpan");

                    }


                    if(source.Where(O=>!O.IsSended).Count()<=0)
                    {
                        if(!db.PTI.Update(O => new { O.OnSMU }, new pti { Id = pTISelected.Id, OnSMU = true }, O => O.Id == pTISelected.Id))
                            throw new SystemException("Data Tidak Tersimpan");
                        else
                            pTISelected.OnSMU = true;

                        var h = User.GenerateHistory(pTISelected.Id, BussinesType.PTI, ChangeType.Update, string.Format("Dibuatkan SMU dengan Nomor T{0:D9}",smudata.Id));
                        if (!db.Histories.Insert(h))
                            throw new SystemException("Gagal Simpan Data");
                    }


                    

                    var history = User.GenerateHistory(smudata.Id, BussinesType.SMU, ChangeType.Create, "");
                    if (!db.Histories.Insert(history))
                        throw new SystemException("Gagal Simpan Data");

                    SMU sm = new SMU
                    {
                        CreatedDate = date, 
                        Id = smudata.Id,
                        IsSended = false,
                        Pcs = source.Sum(O => O.Pcs),
                        RecieverId = pTISelected.RecieverId,
                        RecieverName = pTISelected.RecieverName,
                        ShiperId = pTISelected.ShiperID,
                        ShiperName = pTISelected.ShiperName,
                        Weight = source.Sum(O => O.Weight),
                        Biaya = source.Sum(O=>O.Biaya)
                    };
                    
                    trans.Commit();
                    return Task.FromResult(sm);
                  
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new SystemException(ex.Message);
                }
              
            }
        }

        public Task InsertPTIItemToSMU(SMU smuSelected,ObservableCollection<collies> source)
        {

            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {
                    foreach (var item in source.Where(O => O.IsSended ==true).ToList())
                    {
                        if (!db.SMUDetails.Insert(new smudetails { colliesId = item.Id, SMUId = smuSelected.Id }))
                            throw new SystemException("Data Tidak Tersimpan");

                        var note = string.Format("Menambah Item Ke SMU No T{0:D9}-{1}-{2}-{3}-{4}-{5}",smuSelected.Id, item.Content, item.Kemasan, item.Pcs, item.Weight, item.TotalWeight);
                        var histDetail = User.GenerateHistory(smuSelected.Id, BussinesType.SMU, ChangeType.Update, note);
                        if (!db.Histories.Insert(histDetail))
                            throw new SystemException("Data Tidak Tersimpan");
                    }



                    if(source.Where(O=>!O.IsSended).Count()<=0)
                    {
                        if (!db.PTI.Update(O => new { O.OnSMU }, new pti { OnSMU = true, Id = smuSelected.PTIId }, O => O.Id == smuSelected.PTIId))
                            throw new SystemException("Data Tidak Tersimpan");
                    }
                    trans.Commit();
                    return Task.FromResult(0);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new SystemException(ex.Message);
                }
            }
        }

        public Task<List<SuratMuatanUdara>> GetSMUHeader(int id)
        {
            try
            {
                using (var db = new OcphDbContext())
                {

                    var cmd = db.CreateCommand();
                    cmd.CommandText = "SMUHEADER";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("id", id));
                    var reader = cmd.ExecuteReader();
                    var result = MappingProperties<SuratMuatanUdara>.MappingTable(reader);
                    if (result.Count < 0)
                        throw new SystemException("Detail SMU Tidak Ditemukan");

                    return Task.FromResult(result);

                }
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
        }

        public void SetCancelSMU(SMU selectedItem, string alasan)
        {
            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {
                    if(selectedItem.ActiveStatus!= ActivedStatus.Cancel)
                    {

                        var result = (from a in db.ManifestDetail.Where(O => O.SMUId == selectedItem.Id)
                                      join b in db.Manifest.Select() on a.manifestoutgoingId equals b.Id
                                      select b).FirstOrDefault();


                        if (result != null && result.IsTakeOff)
                            throw new SystemException(string.Format("T{0:D9} Tidak Dapat Dibatalkan \r\n Telah Berangkat Dengan Manifest Nomor MT{1:D9}", selectedItem.Id, result.Id));

                        if (result != null && !result.IsTakeOff)
                            throw new SystemException(string.Format("Batalkan T{0:D9} Dari Manifest Nomor MT{1:D8}", selectedItem.Id, result.Id));


                        ActivedStatus active = ActivedStatus.Cancel;
                        var updated = db.SMU.Update(O => new { O.ActiveStatus }, new smu { Id = selectedItem.Id, ActiveStatus = active }, O => O.Id == selectedItem.Id);
                        if (updated)
                        {
                            selectedItem.ActiveStatus = ActivedStatus.Cancel;
                            var his = User.GenerateHistory(selectedItem.Id, BussinesType.SMU, ChangeType.Cancel, alasan);
                            if (!db.Histories.Insert(his))
                                throw new SystemException("Gagal Diubah");

                            trans.Commit();
                        }
                        else
                        {
                            throw new SystemException("Gagal Diubah");
                        }
                    }else

                        throw new SystemException("SMU Telah Batal");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new SystemException(ex.Message);
                }

            }
        }

        public bool ManifestIsTakeOf(SMU selectedItem)
        {
            using (var db = new OcphDbContext())
            {
                var result = db.ManifestDetail.Where(O => O.SMUId == selectedItem.Id).FirstOrDefault();
                if(result!=null)
                {
                    var manifest = db.Manifest.Where(O => O.Id == result.manifestoutgoingId).FirstOrDefault();
                    if (manifest!=null && manifest.IsTakeOff)
                        return true;
                }

                return false;

            }
        }

        public Task<List<collies>> GetSMUOutOfManifest(int id)
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "PTIColliesOutSMU";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("ptiId", id));
                    var reader = cmd.ExecuteReader();
                    var result = MappingProperties<collies>.MappingTable(reader);
                    return Task.FromResult(result);
                }
            }
            catch (Exception)
            {
                return Task.FromResult(new List<collies>());
            }
        }

        public Task<Tuple<SMU,SMU>> SplitSMU(SMU smuSelected, ObservableCollection<SMUDetail> originSource, ObservableCollection<SMUDetail> destinationSource)
        {
            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {
                    //remove selected Move
                    foreach(var item in destinationSource)
                    {
                        if (!db.SMUDetails.Delete(O => O.SMUId == item.Id && O.colliesId == item.ColliesId))
                            throw new SystemException("Smu Gagal di Split");
                    }
                    var firstColly = originSource.FirstOrDefault();
                    var ptiModel = new PTI { Id = firstColly.PTIId, ShiperID = smuSelected.ShiperId, RecieverId = smuSelected.RecieverId, PayType = firstColly.PayType };

                    var source = new ObservableCollection<collies>();
                    foreach(var item in destinationSource)
                    {
                        source.Add(new collies { Id = item.ColliesId, Content = item.Content, IsSended = true, Pcs = item.Pcs, Price = item.Price, PtiId = item.PTIId, Weight = item.Weight });
                    }

                    smuSelected.Pcs = originSource.Sum(O => O.Pcs);
                    smuSelected.Weight = originSource.Sum(O => O.Weight);
                    smuSelected.Biaya = originSource.Sum(O => O.Biaya);

                    //SUMU
                    var smudata = new smu { PTIId=smuSelected.PTIId, CreatedDate = DateTime.Now};
                    smudata.Id = db.SMU.InsertAndGetLastID(smudata);
                    if (smudata.Id <= 0)
                        throw new SystemException("Data Tidak Tersimpan");

                    foreach (var item in source.Where(O => O.IsSended))
                    {
                        var data = new smudetails { colliesId = item.Id, SMUId = smudata.Id };
                        if (!db.SMUDetails.Insert(data))
                            throw new SystemException("Data Tidak Tersimpan");

                    }

                   
                    var history = User.GenerateHistory(smudata.Id, BussinesType.SMU, ChangeType.Create, "");
                    if (!db.Histories.Insert(history))
                        throw new SystemException("Gagal Simpan Data");

                    SMU sm = new SMU
                    {
                        CreatedDate = smuSelected.CreatedDate,
                        Id = smudata.Id,
                        IsSended = false,
                        Pcs = source.Sum(O => O.Pcs),
                        RecieverId = smuSelected.RecieverId,
                        RecieverName = ptiModel.RecieverName,
                        ShiperId = ptiModel.ShiperID,
                        ShiperName = ptiModel.ShiperName,
                        Weight = source.Sum(O => O.Weight),
                        Biaya = source.Sum(O => O.Biaya)
                    };

                    //
                    trans.Commit();
                    return Task.FromResult( new Tuple<SMU, SMU>(smuSelected, sm));


                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new SystemException(ex.Message);
                }
            }
        }

        public bool RemoveSMUItem(SMUDetail sMUDetail)
        {

            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {
                    if(db.SMUDetails.Delete(O=>O.colliesId==sMUDetail.ColliesId))
                    {
                        var item = sMUDetail;
                        var note = string.Format(@"Menghapus Item : \n\r {0}-{1}-{2}-{3}",
                                   item.Content, item.Pcs, item.Weight, item.Price);
                        var his = User.GenerateHistory(item.Id, BussinesType.SMU, ChangeType.Delete, note);
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
                    throw new SystemException(ex.Message);
                }
            }   
        }

        public Task<List<SMUDetail>> GetSMUDetail(int id)
        {
            try 
            {
                using (var db = new OcphDbContext())
                {

                    var cmd = db.CreateCommand();
                    cmd.CommandText = "SMUDetail";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("id", id));
                    var reader = cmd.ExecuteReader();
                    var result = MappingProperties<SMUDetail>.MappingTable(reader);
                    if (result.Count < 0)
                        throw new SystemException("Detail SMU Tidak Ditemukan");

                    return Task.FromResult(result);

                }
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
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