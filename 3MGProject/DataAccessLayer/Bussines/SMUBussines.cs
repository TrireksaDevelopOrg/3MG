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
                    
                    var smudata = new smu {  PTIId=pTISelected.Id, CreatedDate = DateTime.Now, Kode= CodeGenerate.GetNewSMUNumber().Result };
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

                        var h = User.GenerateHistory(pTISelected.Id, BussinesType.PTI, ChangeType.Update, string.Format("Dibuatkan SMU dengan Nomor T{0:D9}",smudata.Kode));
                        if (!db.Histories.Insert(h))
                            throw new SystemException("Gagal Simpan Data");
                    }


                    if (pTISelected.PayType == PayType.Deposit)
                    {
                        Tuple<bool,double> cukup = CustomerDepositCukup(pTISelected.ShiperID, source.Where(O => O.IsSended).Sum(O=>O.Biaya));
                        var debet = new debetdeposit { CreatedDate = date, SMUId = smudata.Id };
                        if (cukup.Item1)
                        {
                            var depId = db.DebetDeposit.InsertAndGetLastID(debet);
                            if (depId<=0)
                                throw new SystemException("Gagal Debet Deposit");

                            var his = User.GenerateHistory(depId, BussinesType.DebetDeposit, ChangeType.Create, "");
                            if (!db.Histories.Insert(his))
                                throw new SystemException("Gagal Simpan Data");
                        }
                        else
                        {
                            throw new SystemException(string.Format(" Saldo Tidak Cukup !\r Sisa Saldo Rp {0:N2}",cukup.Item2));
                        }
                
                    }

                    var history = User.GenerateHistory(smudata.Id, BussinesType.SMU, ChangeType.Create, "");
                    if (!db.Histories.Insert(history))
                        throw new SystemException("Gagal Simpan Data");

                    SMU sm = new SMU
                    {
                        CreatedDate = date, 
                        Id = smudata.Id,
                        IsSended = false,
                        Kode = smudata.Kode,
                        Pcs = source.Sum(O => O.Pcs),
                        RecieverId = pTISelected.RecieverId,
                        RecieverName = pTISelected.RecieverName,
                        ShiperId = pTISelected.ShiperID,
                        ShiperName = pTISelected.ShiperName,
                        Weight = source.Sum(O => O.Weight),
                        Biaya = source.Sum(O=>O.Biaya)
                    };
                    pTISelected.OnSMU = true;
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
                            throw new SystemException(string.Format("{0} Tidak Dapat Dibatalkan \r\n Telah Berangkat Dengan Manifest Nomor {1}", selectedItem.Code, result.Code));

                        if (result != null && !result.IsTakeOff)
                            throw new SystemException(string.Format("Batalkan {0} Dari Manifest Nomor {1}", selectedItem.Code, result.Code));


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
                        if (!db.SMUDetails.Delete(O => O.SMUId == item.Id && O.colliesId == item.CollyId))
                            throw new SystemException("Smu Gagal di Split");
                    }
                    var firstColly = originSource.FirstOrDefault();
                    var ptiModel = new PTI { Id = firstColly.PTIId, ShiperID = smuSelected.ShiperId, RecieverId = smuSelected.RecieverId, PayType = firstColly.PayType };

                    var source = new ObservableCollection<collies>();
                    foreach(var item in destinationSource)
                    {
                        source.Add(new collies { Id = item.CollyId, Content = item.Content, IsSended = true, Pcs = item.Pcs, Price = item.Price, PtiId = item.PTIId, Weight = item.Weight });
                    }

                    smuSelected.Pcs = originSource.Sum(O => O.Pcs);
                    smuSelected.Weight = originSource.Sum(O => O.Weight);
                    smuSelected.Biaya = originSource.Sum(O => O.Biaya);

                    //SUMU
                    var smudata = new smu { PTIId=smuSelected.PTIId, CreatedDate = DateTime.Now, Kode = CodeGenerate.GetNewSMUNumber().Result };
                    smudata.Id = db.SMU.InsertAndGetLastID(smudata);
                    if (smudata.Id <= 0)
                        throw new SystemException("Data Tidak Tersimpan");

                    foreach (var item in source.Where(O => O.IsSended))
                    {
                        var data = new smudetails { colliesId = item.Id, SMUId = smudata.Id };
                        if (!db.SMUDetails.Insert(data))
                            throw new SystemException("Data Tidak Tersimpan");

                    }

                    if (ptiModel.PayType == PayType.Deposit)
                    {
                        Tuple<bool, double> cukup = CustomerDepositCukup(ptiModel.ShiperID, source.Where(O => O.IsSended).Sum(O => O.Biaya));
                        var debet = new debetdeposit { CreatedDate = smuSelected.CreatedDate, SMUId = smudata.Id };
                        var depId = db.DebetDeposit.InsertAndGetLastID(debet);
                        if (depId <= 0)
                            throw new SystemException("Gagal Debet Deposit");

                        var his = User.GenerateHistory(depId, BussinesType.DebetDeposit, ChangeType.Create, "");
                        if (!db.Histories.Insert(his))
                            throw new SystemException("Gagal Simpan Data");
                        
                    }

                    var history = User.GenerateHistory(smudata.Id, BussinesType.SMU, ChangeType.Create, "");
                    if (!db.Histories.Insert(history))
                        throw new SystemException("Gagal Simpan Data");

                    SMU sm = new SMU
                    {
                        CreatedDate = smuSelected.CreatedDate,
                        Id = smudata.Id,
                        IsSended = false,
                        Kode = smudata.Kode,
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

      

        private Tuple<bool,double> CustomerDepositCukup(int shiperID, double biaya)
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "CustomerSaldo";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("cusId", shiperID));
                    var data = cmd.ExecuteScalar();
                    double saldo = double.Parse(data.ToString());
                    if (saldo >= biaya)
                        return new Tuple<bool, double>(true,saldo);
                    else
                        return new Tuple<bool, double>(false, saldo);
                }
            }
            catch (Exception)
            {

                 return new Tuple<bool, double>(false, 0);
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