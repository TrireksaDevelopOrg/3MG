using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Manifest : manifestoutgoing
    {
        public bool IsTakeOff { get; set; }
        public DateTime Tanggal { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public int PlaneId { get; set; }
        public bool Complete { get; set; }
        public int PortFrom { get; set; }
        public int PortTo { get; set; }
        public string PlaneName { get; set; }
        public string PlaneCode { get; set; }
        public string OriginPortName { get; set; }
        public string OriginPortCode { get; set; }
        public string OriginCityName { get; set; }
        public string OriginCityCode { get; set; }
        public string DestinationPortName { get; set; }
        public string DestinationPortCode { get; set; }
        public string DestinationCityName { get; set; }
        public string DestinationCityCode { get; set; }
        public int OriginPortCityId { get; set; }
        public int DestinationPortCityId { get; set; }
    }
 
}
