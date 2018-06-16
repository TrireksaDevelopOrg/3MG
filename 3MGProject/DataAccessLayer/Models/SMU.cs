using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class SMU
    {
        private string _code;

        public int Id { get; set; }
        public int Kode { get; set; }
        public string ShiperName { get; set; }
        public string   RecieverName{ get; set; }
        public int Pcs { get; set; }
        public double Weight { get; set; }
        public double Biaya { get; set; }
        public DateTime CreatedDate { get; set; }
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

    }
}
