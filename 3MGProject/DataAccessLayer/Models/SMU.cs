using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class SMU:BaseNotify
    {
        private string _code;
        private bool _isSended;
        private double _ppn;
        private double total;

        public int Id { get; set; }
        public int Kode { get; set; }
        public string ShiperName { get; set; }
        public string   RecieverName{ get; set; }
        public int ShiperId { get; set; }
        public int RecieverId{ get; set; }
        public int Pcs { get; set; }
        public double Weight { get; set; }
        public double Biaya { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ManifestCode { get; set; }
      
        public bool IsSended {
            get { return _isSended; }
            set
            {
                SetProperty(ref _isSended, value);
            }
        }

        public double PPN {
            get {
                if (_ppn <= 0)
                    _ppn=Biaya+(Biaya * 10/100);
                return _ppn;
            }
            set { SetProperty(ref _ppn, value); }
        }

        public virtual double Total { get { return Biaya + PPN; } set { total = value; } }

        public virtual string Code
        { get
            {
                if(string.IsNullOrEmpty(_code))
                {
                    _code = CodeGenerate.SMU(Kode);
                }
                return _code; }
            set
            {
                 _code = value;
            }
        }

        public ActivedStatus ActiveStatus { get;  set; }
    }
}
