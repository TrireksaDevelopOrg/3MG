using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public enum ActivedStatus
    {
        OK,Pending,Cancel
    }
    
    public enum PaymentType
    {
        Transfer, Cash, Cek
    }

    public enum PayType
    {
        Chash,Credit,Deposit
    }

    public enum WeightType
    {
        Weight, WeightVolume
    }
                    
    public enum CustomerType
    {
        Common,Deposit
    }

    public enum Gender
    {
        Pria,Wanita
    }

    public enum ChangeType
    {
        Create,Update,Delete,
        Cancel
    }

    public enum BussinesType
    {
        Deposit, DebetDeposit, PTI, SMU, Manifest, Schedule, Invoice, Customer
    }

}
