using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class BorderelCargoModel : PreFligtManifest
    {
        public string PlaneName { get; set; }
        public string PlaneReg { get; set; }
        public string PortFromCode { get; set; }
        public string PortToCode { get; set; }
    }
}
