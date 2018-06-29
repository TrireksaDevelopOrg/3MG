using DataAccessLayer.DataModels;
using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class SMU:BaseNotify
    {
        private bool _isSended;
        private double _ppn;
        private double total;

        public int Id { get; set; }
        public string ShiperName { get; set; }
        public string   RecieverName{ get; set; }
        public int ShiperId { get; set; }
        public int RecieverId{ get; set; }
        public int Pcs { get; set; }
        public double Weight { get; set; }
        public double Biaya { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ManifestId { get; set; }
        public int PTIId { get; set; }
        public string Content { get; set; } 
        public PayType PayType { get; set; }
      
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
                    _ppn=Biaya * (0.1);
                return _ppn;
            }
            set { SetProperty(ref _ppn, value); }
        }

        public virtual double Total { get { return Biaya + PPN; } set { total = value; } }



        public ObservableCollection<SMUDetail> Details { get; set; } = new ObservableCollection<SMUDetail>();

        public ActivedStatus ActiveStatus { get;  set; }

        public void SetSilentIsSended()
        {
            throw new NotImplementedException();
        }

        public void SetSilentIsSended(bool v)
        {
            _isSended = v;
        }
    }
}
