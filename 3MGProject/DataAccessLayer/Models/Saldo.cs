using Ocph.DAL;
using System;

namespace DataAccessLayer.Models
{
    public class Saldo:BaseNotify
    {
        public DateTime Tanggal { get; set; }
        public double Kredit { get; set; }
        public double Debet { get; set; }
        public double SaldoAkhir { get; set; }
        public string Description { get; set; }

        private double _ppn;
        private double total;

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int SMUId { get; set; }
        public int ManifestId { get; set; }

        public int colliesId { get; set; }
        public int Pcs { get; set; }
        public double Weight { get; set; }
        public double Biaya { get; set; }
        public PayType PayType { get; set; }
        public int PTIId { get; set; }
        public string ShiperName { get; set; }
        public string ReciverName { get; set; }
        public int ShiperID { get; set; }
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
