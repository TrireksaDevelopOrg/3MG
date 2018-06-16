using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("deposit")]
    public class deposit :BaseNotify  
   {
          [PrimaryKey("Id")] 
          [DbColumn("Id")] 
          public int Id 
          { 
               get{return _id;} 
               set{ 

                    SetProperty(ref _id, value);
                     }
          } 

          [DbColumn("Jumlah")] 
          public double Jumlah 
          { 
               get{return _jumlah;} 
               set{ 

                    SetProperty(ref _jumlah, value);
                     }
          } 

          [DbColumn("PaymentType")] 
          public PaymentType PaymentType 
          { 
               get{return _paymenttype;} 
               set{ 

                    SetProperty(ref _paymenttype, value);
                     }
          } 

          [DbColumn("CustomerId")] 
          public int CustomerId 
          { 
               get{return _customerid;} 
               set{ 

                    SetProperty(ref _customerid, value);
                     }
          } 

          [DbColumn("CreatedDate")] 
          public DateTime CreatedDate 
          { 
               get{return _createddate;} 
               set{ 

                    SetProperty(ref _createddate, value);
                     }
          }


        [DbColumn("TanggalBayar")]
        public DateTime TanggalBayar
        {
            get { return _tanggalBayar; }
            set
            {

                SetProperty(ref _tanggalBayar, value);
            }
        }

        private int  _id;
           private double  _jumlah;
           private PaymentType  _paymenttype;
           private int  _customerid;
           private DateTime  _createddate;
        private DateTime _tanggalBayar;
    }
}


