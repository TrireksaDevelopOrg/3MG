using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
  public  class PTI
    {
        private string _code;

        public int Id { get; set; }
        public int PTINumber { get; set; }
        public virtual string Code {
            get { return CodeGenerate.PTI(PTINumber); }
            set {
                _code = value;
            }
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
        public DateTime CreatedDate { get; set; }
        public PayType PayType { get; set; }
        public string UserId { get; set; }


    }
}
