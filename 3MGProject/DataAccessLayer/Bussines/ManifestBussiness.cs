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

                        //update SMU

                        if (!db.SMU.Update(O => new { O.IsSended }, new smu { Id = item.Id, IsSended = true }, O => O.Id == item.Id))
                            throw new SystemException("Manifest Tidak Tersimpan");

                        var his = User.GenerateHistory(item.Id, BussinesType.SMU, ChangeType.Update, "Set SMU Terkirim");
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
                        throw new SystemException(string.Format("Manifest Nomor  {0}  Tidak Dapat Dibatalkan.\r\n Pesawat Telah Berangkat Atau Tiba di tujuan", manifestSelected.Code));

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
    }
}
