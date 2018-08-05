using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MySql.Data.MySqlClient;
using Ocph.DAL.Mapping;
using DataAccessLayer.Models;

namespace DataAccessLayer.Bussines
{
    public class CustomerBussiness : Authorization
    {
        public CustomerBussiness() : base(typeof(CustomerBussiness)) { }


        public List<customer> GetCustomersDeposites()
        {
            using (var db = new OcphDbContext())
            {
                CustomerType ct = CustomerType.Deposit;
                return db.Customers.Where(O => O.CustomerType == ct).ToList();
            }
        }
        
        public void AddNewCustomerDeposit(customer cust)
        {
            using (var db = new OcphDbContext())
            {
               if(cust.Id<=0)
                {
                    cust.Id = db.Customers.InsertAndGetLastID(cust);
                    if (cust.Id <= 0)
                        throw new SystemException("Data Tidak Tersimpan");
                }else
                {
                    if(!db.Customers.Update(O=> new
                    {
                        O.Address,
                        O.ContactName,
                        O.Email,
                        O.Handphone,
                        O.Name,
                        O.Phone1,
                        O.NoIdentitas
                    }, cust, O => O.Id == cust.Id))
                    {
                        throw new SystemException("Data Tidak Tersimpan");
                    }
                        
                }
            }
        }

        public Task<List<Deposit>> GetDepositsOfCustomer(customer value)
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "DepositOfCustomer";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("id", value.Id));
                    var reader = cmd.ExecuteReader();
                    var list = MappingProperties<Deposit>.MappingTable(reader);
                    return Task.FromResult(list);
                }
            }
            catch (Exception)
            {
                return Task.FromResult(new List<Deposit>());
            }
        }


        public Task<double> GetSisaSaldo(int shiperID)
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
                    return Task.FromResult(saldo);
                }
            }
            catch (Exception)
            {
                double res = 0;
                return Task.FromResult(res);
            }
        }

        public void EditCustomerDeposit(customer cust)
        {

            using (var db = new OcphDbContext())
            {
                try
                {
                    if(!db.Customers.Update(O=>new {O.Address,O.ContactName,O.CustomerType,O.Email,O.Handphone,O.Name,O.NoIdentitas,O.Phone1 },cust,O=>O.Id==cust.Id))
                    {
                        throw new SystemException("Data Tidak Tersimpan");
                    }
                }
                catch (Exception ex)
                {
                    throw new SystemException(ex.Message);
                }
            }
        }

        public Task<List<DebetDeposit>> GetDebetDeposit(int id)
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "CustomerDebetDeposit";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("id", id));
                    var reader = cmd.ExecuteReader();
                    var list = MappingProperties<DebetDeposit>.MappingTable(reader);
                    return Task.FromResult(list);
                }
            }
            catch (Exception)
            {
                return Task.FromResult(new List<DebetDeposit>());
            }
        }

        public Task<List<Saldo>> GetRekeningKorang(DateTime tanggal,int ShiperId)
        {
            DateTime now = new DateTime(tanggal.Year, tanggal.Month, tanggal.Day);
            try
            {
                using (var db = new OcphDbContext())
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "SaldoAkhirSebelumTanggal";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("tanggal", now));
                    cmd.Parameters.Add(new MySqlParameter("custId", ShiperId));
                    var saldoAkhir =(double) cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "DebetFromDate";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("tanggal", tanggal));
                    cmd.Parameters.Add(new MySqlParameter("custId", ShiperId));
                    var reader =cmd.ExecuteReader();
                    var DataDebet = MappingProperties<SMU>.MappingTable(reader);
                    reader.Close();
                    var deposit = db.Deposit.Where(O => O.CustomerId == ShiperId && O.CreatedDate >= now).ToList();

                    List<Saldo> list = new List<Saldo>();
                    list.Add(new Models.Saldo { SaldoAkhir = saldoAkhir, Tanggal = now.AddDays(-1), Description="Saldo Awal" });
                    foreach(var item in DataDebet )
                    {
                        list.Add(new Saldo
                        {
                            Description = string.Format("SMU T{0:D9}",item.Id),
                            Debet = item.Biaya, SMUId=item.Id,
                            Tanggal = item.CreatedDate,  Biaya=item.Biaya-(item.Biaya*0.1), CreatedDate=item.CreatedDate, ManifestId=item.ManifestId,
                              Pcs=item.Pcs, PPN=item.PPN,  ReciverName=item.RecieverName, Total=item.Biaya, Weight=item.Weight
                        });
                    }


                    foreach(var item in deposit)
                    {
                        list.Add(new Saldo { Description = "Deposit", Kredit = item.Jumlah , Tanggal=item.CreatedDate});
                    }

                    return Task.FromResult(list.OrderBy(O=>O.Tanggal).ToList());
                }
            }
            catch (Exception)
            {
                return Task.FromResult(new List<Saldo>());
            }
        }
    }
}
