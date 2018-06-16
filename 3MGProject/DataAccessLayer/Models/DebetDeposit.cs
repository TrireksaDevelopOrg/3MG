using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class DebetDeposit
    {
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
        public string User { get; set; }
    }
}
