using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DataModels;
using DataAccessLayer.Models;

namespace DataAccessLayer.Bussines
{
    public class DepositBussines:Authorization
    {
        public DepositBussines() : base(typeof(DepositBussines)) { }

        [Authorize("Accounting")]
        public void AddNewDeposit(Deposit dep)
        {
            using (var db = new OcphDbContext())
            {
                var trans = db.BeginTransaction();
                try
                {
                    if (dep.Id <= 0)
                    {
                        dep.Id = db.Deposit.InsertAndGetLastID(dep);
                        if (dep.Id <= 0)
                            throw new SystemException("Deposit Tidak Tersimpan");

                        var history = User.GenerateHistory(dep.Id, BussinesType.Deposit, ChangeType.Create, "");
                        if(!db.Histories.Insert(history))
                            throw new SystemException("Deposti Tidak Tersimpan");
                        trans.Commit();
                    }
                    else
                    {
                        if (!db.Deposit.Update(O => new { O.Jumlah, O.PaymentType, O.TanggalBayar }, dep, O => O.Id == dep.Id))
                            throw new SystemException("Deposit Tidak Tersimpan");

                        var history = User.GenerateHistory(dep.Id, BussinesType.Deposit, ChangeType.Update, "");
                        if (!db.Histories.Insert(history))
                            throw new SystemException("Deposti Tidak Tersimpan");
                        dep.User = User.Name;
                        trans.Commit();
                    }
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
