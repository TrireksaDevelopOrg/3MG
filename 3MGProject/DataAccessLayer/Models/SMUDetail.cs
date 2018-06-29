using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class SMUDetail :BaseNotify, ICloneable
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
        public int ColliesId { get; set; }
        public string Kemasan { get; set; }


        public PayType PayType { get; set; }

        public double Biaya
        {
            get {
                if (_biaya <= 0)
                    _biaya = (Pcs * Weight) * Price;
                return _biaya; }
            set { SetProperty(ref _biaya, value); }
        }

        public double TotalWeight
        {
            get
            {
                if (_totalWeight <= 0)
                    _totalWeight = Pcs * Weight;
                return _totalWeight;
            }
            set { SetProperty(ref _totalWeight, value); }
        }


        private bool selected;
        private double _biaya;
        private double _totalWeight;

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
