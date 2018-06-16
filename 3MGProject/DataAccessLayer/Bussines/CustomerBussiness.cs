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
                        O.Phone2
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
    }
}
