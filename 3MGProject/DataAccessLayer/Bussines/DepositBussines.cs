using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DataModels;

namespace DataAccessLayer.Bussines
{
    public class DepositBussines
    {
        public void AddNewDeposit(deposit dep)
        {
            using (var db = new OcphDbContext())
            {
                if(dep.Id<=0)
                {
                    dep.Id = db.Deposit.InsertAndGetLastID(dep);
                    if (dep.Id <= 0)
                        throw new SystemException("Data Tidak Tersimpan");
                }else
                {
                    if(!db.Deposit.Update(O => new { O.Jumlah,O.PaymentType,O.TanggalBayar },dep,O=>O.Id==dep.Id))
                        throw new SystemException("Data Tidak Tersimpan");
                }
            }
        }
    }
}
