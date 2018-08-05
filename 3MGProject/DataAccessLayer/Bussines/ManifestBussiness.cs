using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DataModels;
using DataAccessLayer.Models;
using MySql.Data.MySqlClient;
using Ocph.DAL;
using Ocph.DAL.Mapping;

namespace DataAccessLayer.Bussines
{
    public class ManifestBussiness:Authorization
    {
        public ManifestBussiness():base(typeof(ManifestBussiness))
        {
           
        }

        public Task<List<SMU>> GetSMUForCreateManifest()
        {
            using (var db = new OcphDbContext())
            {
                var cmd= db.CreateCommand();
                cmd.CommandText = "SMUForManifest";
                cmd.CommandType = CommandType.StoredProcedure;
                var reader = cmd.ExecuteReader();
                var result = MappingProperties<SMU>.MappingTable(reader);
                if (result.Count <= 0)
                    throw new SystemException("Data TIdak Ada");

                return Task.FromResult(result);
            }
        }

        public Task<manifestoutgoing> CreateNewManifest(Schedule scheduleSelected, manifestoutgoing manifest, ObservableCollection<SMU> source)
        {
          
            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {
                    manifest.Id = db.Manifest.InsertAndGetLastID(manifest);
                    if (manifest.Id <= 0)
                        throw new SystemException("Manifest Tidak Tersimpan");

                    foreach(var item in source.Where(O=>O.IsSended).ToList())
                    {
                        var detail = new manifestdetails { Id=0, manifestoutgoingId = manifest.Id, SMUId = item.Id };
                        detail.Id = db.ManifestDetail.InsertAndGetLastID(detail);
                        if(detail.Id<=0)
                            throw new SystemException("Manifest Tidak Tersimpan");
                        //Debet Deposit
                        if(item.PayType== PayType.Deposit)
                        {
                            var date = DateTime.Now;
                            Tuple<bool, double> cukup = CustomerDepositCukup(item.ShiperId, item.Total);
                            var debet = new debetdeposit { CreatedDate =scheduleSelected.Tanggal , SMUId = item.Id };
                            if (cukup.Item1)
                            {
                                var depId = db.DebetDeposit.InsertAndGetLastID(debet);
                                if (depId <= 0)
                                    throw new SystemException("Gagal Debet Deposit");

                                var hist = User.GenerateHistory(depId, BussinesType.DebetDeposit, ChangeType.Create, "");
                                if (!db.Histories.Insert(hist))
                                    throw new SystemException("Gagal Debet Deposit");
                            }
                            else
                            {
                                throw new SystemException(string.Format(" Saldo Tidak Cukup !\r Sisa Saldo Rp {0:N2}", cukup.Item2));
                            }

                        }



                        //update SMU

                        if (!db.SMU.Update(O => new { O.IsSended }, new smu { Id = item.Id, IsSended = true }, O => O.Id == item.Id))
                            throw new SystemException("Manifest Tidak Tersimpan");

                        var his = User.GenerateHistory(item.Id, BussinesType.SMU, ChangeType.Update, string.Format("Ditambahkan Ke Manifest MT{0:D8}",manifest.Id));
                        if(!db.Histories.Insert(his))
                            throw new SystemException("Manifest Tidak Tersimpan");

                    }

                    var history = User.GenerateHistory(manifest.Id, BussinesType.Manifest, ChangeType.Create, "");
                    if (!db.Histories.Insert(history))
                        throw new SystemException("Manifest Tidak Tersimpan");

                    manifest.User = User.Name;
                    
                    trans.Commit();
                    return Task.FromResult(manifest);

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new SyntaxErrorException(ex.Message);
                }
            }
        }

        public void InsertNewSMU(Manifest manifest ,IEnumerable<SMU> enumerable)
        {
            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {

                    foreach(var item in enumerable)
                    {
                        if (db.ManifestDetail.Insert(new manifestdetails { manifestoutgoingId = manifest.Id, SMUId = item.Id }))
                        {
                            if (!db.SMU.Update(O => new { O.IsSended }, new smu { IsSended = false }, O => O.Id == item.Id))
                                throw new SystemException("SMU Tidak Berhasil di tambah");
                            var his = User.GenerateHistory(manifest.Id, BussinesType.Manifest, ChangeType.Update,
                                string.Format("Tambah SMU T{0:D9}", item.Id));
                            if (!db.Histories.Insert(his))
                                throw new SystemException("SMU Tidak Berhasil di tambah");
                        }
                        else
                            throw new SystemException("SMU Tidak Berhasil di tambah");
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

        public Task<List<SMU>> ManifestDetails(Manifest selected)
        {
            using (var db = new OcphDbContext())
            {
                var cmd = db.CreateCommand();
                cmd.CommandText = "ManifestDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("id", selected.Id));
                var reader = cmd.ExecuteReader();
                var result = MappingProperties<SMU>.MappingTable(reader);
                if (result.Count <= 0)
                    throw new SystemException("Manifest Ini Telah Kosong, Sebaiknya Anda Batalkan");

                return Task.FromResult(result);
            }
        }


        public Task<List<PreFligtManifest>> ManifestPreFlightDetails(Manifest selected)
        {
            using (var db = new OcphDbContext())
            {
                var cmd = db.CreateCommand();
                cmd.CommandText = "PreFlight";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("id", selected.Id));
                var reader = cmd.ExecuteReader();
                var result = MappingProperties<PreFligtManifest>.MappingTable(reader);
                if (result.Count <= 0)
                    throw new SystemException("Data TIdak Ada");

                return Task.FromResult(result);
            }
        }

        public void SetCancelManifest(Manifest manifestSelected, string alasan)
        {
            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {
                    if (manifestSelected.IsTakeOff)
                        throw new SystemException(string.Format("Manifest Nomor  MT{0:D8}  Tidak Dapat Dibatalkan.\r\n Pesawat Telah Berangkat Atau Tiba di tujuan", manifestSelected.Id));

                    ActivedStatus actived = ActivedStatus.Cancel;
                    var updated = db.Manifest.Update(O => new { O.ActiveStatus }, new manifestoutgoing { Id = manifestSelected.Id, ActiveStatus = actived },
                        O => O.Id == manifestSelected.Id);
                    if(updated)
                    {
                        var history = User.GenerateHistory(manifestSelected.Id, BussinesType.Manifest, ChangeType.Cancel, alasan);
                        if (!db.Histories.Insert(history))
                            throw new SyntaxErrorException("Manifest tidak dapat dibatalkan");

                        manifestSelected.ActiveStatus = ActivedStatus.Cancel;
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {

                    throw new SystemException(ex.Message);
                }
            }
        }

        public void SetTakeOff(Manifest manifestSelected)
        {

            using (var db = new OcphDbContext())
            {
                try
                {
                    if(!db.Manifest.Update(O=> new {O.IsTakeOff  },new manifestoutgoing {IsTakeOff=true },O=>O.Id==manifestSelected.Id))
                    {
                        throw new SystemException("Data Tidak Tersimpan");
                    }

                    manifestSelected.IsTakeOff = true;

                }
                catch (Exception ex)
                {

                    throw new SystemException(ex.Message);
                }
            }
        }

        public bool RemoveSMU(Manifest manif, SMU item)
        {

            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {
                    if (db.ManifestDetail.Delete(O => O.SMUId == item.Id))
                    {
                        var note = string.Format(@"Menghapus SMU : T{0:D9} Dari Manifest MT{1:D8}",item.Id,manif.Id);
                        var his = User.GenerateHistory(manif.Id, BussinesType.Manifest, ChangeType.Update, note);
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

        public Task<List<Manifest>> GetManifest(DateTime start, DateTime end)
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "ManifestFromTo";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("startDate", start));
                    cmd.Parameters.Add(new MySqlParameter("endDate", end));
                    var reader = cmd.ExecuteReader();
                    var result = MappingProperties<Manifest>.MappingTable(reader);
                    if (result.Count <= 0)
                        throw new SystemException("Data TIdak Ada");

                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
        }


        public Tuple<bool, double> CustomerDepositCukup(int shiperID, double biaya)
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
                        return new Tuple<bool, double>(true, saldo);
                    else
                        return new Tuple<bool, double>(false, saldo);
                }
            }
            catch (Exception)
            {

                return new Tuple<bool, double>(false, 0);
            }
        }
    }
}
