using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
  public  class PTI:BaseNotify
    {
        private string _code;
        private int _shiperid;
        private int _reciever;

        public int Id { get; set; }
        public int PTINumber { get; set; }
        public virtual string Code {
            get { return CodeGenerate.PTI(PTINumber); }
            set {
                _code = value;
            }
        }

        public int ShiperID
        {
            get { return _shiperid; }
            set
            {

                SetProperty(ref _shiperid, value);
            }
        }


        public int RecieverId
        {
            get { return _reciever; }
            set
            {

                SetProperty(ref _reciever, value);
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
        public string User { get; set; }
        public bool OnSMU { get; set; }


    }
}
