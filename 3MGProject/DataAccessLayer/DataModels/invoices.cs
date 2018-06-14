using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("invoices")]
    internal class invoices :BaseNotify  
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

          [DbColumn("Number")] 
          public int Number 
          { 
               get{return _number;} 
               set{ 

                    SetProperty(ref _number, value);
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

          [DbColumn("InvoiceStatus")] 
          public string InvoiceStatus 
          { 
               get{return _invoicestatus;} 
               set{ 

                    SetProperty(ref _invoicestatus, value);
                     }
          } 

          [DbColumn("DeadLine")] 
          public DateTime DeadLine 
          { 
               get{return _deadline;} 
               set{ 

                    SetProperty(ref _deadline, value);
                     }
          } 

          [DbColumn("InvoicePayType")] 
          public string InvoicePayType 
          { 
               get{return _invoicepaytype;} 
               set{ 

                    SetProperty(ref _invoicepaytype, value);
                     }
          } 

          [DbColumn("CreateDate")] 
          public DateTime CreateDate 
          { 
               get{return _createdate;} 
               set{ 

                    SetProperty(ref _createdate, value);
                     }
          } 

          [DbColumn("PaidDate")] 
          public DateTime PaidDate 
          { 
               get{return _paiddate;} 
               set{ 

                    SetProperty(ref _paiddate, value);
                     }
          } 

          private int  _id;
           private int  _number;
           private int  _customerid;
           private string  _invoicestatus;
           private DateTime  _deadline;
           private string  _invoicepaytype;
           private DateTime  _createdate;
           private DateTime  _paiddate;
      }
}


