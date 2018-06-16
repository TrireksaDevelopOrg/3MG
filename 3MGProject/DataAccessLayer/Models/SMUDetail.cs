using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class SMUDetail : BaseNotify, ICloneable
    {

        public int Id { get; set; }
        public int Kode { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsSended { get; set; }
        public string Content { get; set; }
        public int Pcs { get; set; }
        public double Weight { get; set; }
        public double Price { get; set; }
        public bool Sended { get; set; }
        public int PTIId { get; set; }
        public int CollyId { get; set; }

        public PayType PayType { get; set; }

        public double Biaya
        {
            get {
                if (_biaya <= 0)
                    _biaya = (Pcs * Weight) * Price;
                return _biaya; }
            set { SetProperty(ref _biaya, value); }
        }


        private bool selected;
        private double _biaya;

        public virtual bool Selected
        {
            get { return selected; }
            set { SetProperty(ref selected, value); }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public SMUDetail GetClone()
        {
            return (SMUDetail)Clone();
        }
    }
}
