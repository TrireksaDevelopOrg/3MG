using Ocph.DAL;
using System;
using System.Collections.ObjectModel;
using DataAccessLayer.DataModels;

namespace DataAccessLayer.Models
{
    public  class PTI:pti
    {
        public PTI()
        {
            Details = new ObservableCollection<collies>();
        }

        public string ShiperName{ get; set; }
        public string ShiperHandphone { get; set; }
        public string ShiperAddress { get; set; }
        public string RecieverName{ get; set; }
        public string RecieverHandphone { get; set; }
        public string RecieverAddress { get; set; }

        public int Pcs{ get; set; }
        public double Weight  { get; set; }
        public double Biaya{ get; set; }
        public string UserId { get; set; }
        public string User { get; set; }
        public string FromCityName { get; set; }
        public string FromCityCode { get; set; }
        public string  ToCityName { get; set; }
        public string ToCityCode { get; set; }

        public virtual ObservableCollection<collies> Details { get; set; }


    }
}
