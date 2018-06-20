using DataAccessLayer.DataModels;
using DataAccessLayer.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Bussines
{
   public class ScheduleBussines:Authorization
    {
        public ScheduleBussines() : base(typeof(ScheduleBussines)) { }

        public Task<List<planes>> GetPlanes()
        {

            using (var db = new OcphDbContext())
            {
                var result = db.Planes.Select().ToList();
                return Task.FromResult( result);
            }
        }

        public Task<List<city>> GetCities()
        {
            using (var db = new OcphDbContext())
            {
                var result = db.Cities.Select().ToList();
                return Task.FromResult(result);
            }
        }

        public Task<List<ports>> GetPorts()
        {
            using (var db = new OcphDbContext())
            {
                var result = from a in db.Ports.Select()
                             join b in db.Cities.Select() on a.CityId equals b.Id
                             select new ports { City = b.CityName, CityId = b.Id, Code = a.Code, Name = a.Name, Id = a.Id };
                return Task.FromResult(result.ToList());
            }
        }

        public Task<city> CreateNewCity(city item)
        {
            using (var db = new OcphDbContext())
            {
                item.Id = db.Cities.InsertAndGetLastID(item);
                if (item.Id <= 0)
                    throw new SystemException("Data Tidak Tersimpan");
                else
                    return Task.FromResult(item);
            }
        }

        public Task<planes> CreateNewPlane(planes item)
        {

            using (var db = new OcphDbContext())
            {
                item.Id = db.Planes.InsertAndGetLastID(item);
                if (item.Id <= 0)
                    throw new SystemException("Data Tidak Tersimpan");
                else
                    return Task.FromResult(item);
            }
        }

        public Task<List<Schedule>> GetSchedules(DateTime date)
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "GetSchedulueByDate";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("dateSelected", date));
                    var reader = cmd.ExecuteReader();
                    var result = Ocph.DAL.Mapping.MappingProperties<Schedule>.MappingTable(reader);
                    return Task.FromResult( result);
                }
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
        }

        public Task<ports> CreateNewPort(ports item)
        {
            using (var db = new OcphDbContext())
            {
                item.Id = db.Ports.InsertAndGetLastID(item);
                if (item.Id <= 0)
                    throw new SystemException("Data Tidak Tersimpan");

                return Task.FromResult(item);
            }
        }

        [Authorize("Administrator, Manager")]
        public Task<schedules> AddNewSchedule(schedules model)
        {
            var date = DateTime.Now;
            model.CreatedDate = date;
            model.Tanggal = date;
            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {
                    if (User.CanAccess(MethodBase.GetCurrentMethod()))
                    {
                        model.Id = db.Schedules.InsertAndGetLastID(model);
                        if (model.Id > 0)
                        {

                            var history = User.GenerateHistory(model.Id, BussinesType.Schedule, ChangeType.Create, "");
                            if (db.Histories.Insert(history))
                            {
                                trans.Commit();
                                model.User = User.Name;
                                return Task.FromResult(model);
                            }
                               
                            else
                                throw new SystemException("Data Tidak Tersimpan");
                        }
                        else
                            throw new SystemException("Data Tidak Tersimpan");
                      } else
                        throw new SystemException(NotHaveAccess);
;                   
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new SystemException(ex.Message);
                }
            
            }
        }
    }
}
