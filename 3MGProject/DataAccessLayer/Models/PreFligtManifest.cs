﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class PreFligtManifest
    {
        private double _totalWeight;

        public int Id { get; set; }
        public int SMUId { get; set; }
        public int PTIId { get; set; }
        public int ColliesId { get; set; }
        public string Content { get; set; }
        public int Pcs { get; set; }
        public double Weight { get; set; }
        public double Price { get; set; }
        public string Kemasan { get; set; }
        public PayType PayType { get; set; }
        public bool IsSended { get; set; }
        public string ShiperName { get; set; }
        public int ShiperId { get; set; }
        public string RecieverName { get; set; }
        public string FlightNumber { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime Tanggal { get; set; }
        public double TotalWeight
        {
            get
            {
                if (_totalWeight <= 0)
                    _totalWeight = Pcs * Weight;
                return _totalWeight;
            }
            set { _totalWeight=value; }
        }



    }

}