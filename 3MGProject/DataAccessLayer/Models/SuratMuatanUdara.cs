using System;

namespace DataAccessLayer.Models
{
    public class SuratMuatanUdara 
    {
        private string code;

        public int Kode { get; set; }
        public string ShiperName { get; set; }
        public string RecieverName { get; set; }
        public string OriginCity { get; set; }
        public string DetinationCity { get; set; }
        public string OriginPortCode { get; set; }
        public string DetinationPortCode { get; set; }
        public string ShiperAddress { get; set; }
        public string RecieverAddress { get; set; }
        public string ShipherHandphone { get; set; }
        public string RecieverHandphone { get; set; }
        public virtual string Code
        {
            get
            {
                if (string.IsNullOrEmpty(code))
                {
                    code = CodeGenerate.SMU(Kode);
                }
                return code;
            }
            set
            {
                code = value;
            }
        }
    }
}
