using DataAccessLayer.DataModels;

namespace DataAccessLayer.Models
{
    public class History:changehistory
    {
        private string _number;

        public string Name { get; set; }
        public virtual string Number
        {
            get
            {
              string  result = string.Empty;
                switch (BussinesType)
                {
                    case BussinesType.Deposit:
                        result = string.Format("{0:D6}", BussinessId);
                        break;
                    case BussinesType.DebetDeposit:
                        result = string.Format("{0:D6}", BussinessId);
                        break;
                    case BussinesType.PTI:
                        result= string.Format("{0:D6}", BussinessId);
                        break;
                    case BussinesType.SMU:
                        result = string.Format("T{0:D9}", BussinessId);
                        break;
                    case BussinesType.Manifest:
                        result = string.Format("MT{0:D8}", BussinessId);
                        break;
                    case BussinesType.Schedule:
                        result = string.Format("{0:D6}", BussinessId);
                        break;
                    case BussinesType.Invoice:
                        result = string.Format("{0:D6}", BussinessId);
                        break;
                    case BussinesType.Customer:
                        result = string.Format("{0:D6}", BussinessId);
                        break;
                    default:
                        break;
                }

                return result;
            }
            set
            {
                _number = value;
            }
        }
    }
}
