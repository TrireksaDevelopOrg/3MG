using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class DebetDeposit:BaseNotify
    {
        private double _ppn;
        private double total;

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int SMUNumber { get; set; }
        public int SMUId { get; set; }
        public int colliesId { get; set; }
        public int Pcs { get; set; }
        public double Weight { get; set; }
        public double Biaya { get; set; }
        public PayType PayType { get; set; }
        public int PTIId { get; set; }
        public int PTINumber { get; set; }
        public string ShiperName { get; set; }
        public string ReciverName { get; set; }
        public int ShiperID { get; set; }
        public int ManifestCode { get; set; }
        public string User { get; set; }
        public double PPN
        {
            get
            {
                if (_ppn <= 0)
                    _ppn = Biaya + (Biaya * 10 / 100);
                return _ppn;
            }
            set { SetProperty(ref _ppn, value); }
        }

        public virtual double Total { get { return Biaya + PPN; } set { total = value; } }
    }
}
