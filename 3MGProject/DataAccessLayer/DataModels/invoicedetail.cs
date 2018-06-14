using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("invoicedetail")]
    internal class invoicedetail :BaseNotify  
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

          [DbColumn("InvoiceId")] 
          public int InvoiceId 
          { 
               get{return _invoiceid;} 
               set{ 

                    SetProperty(ref _invoiceid, value);
                     }
          } 

          [DbColumn("SMUId")] 
          public int SMUId 
          { 
               get{return _smuid;} 
               set{ 

                    SetProperty(ref _smuid, value);
                     }
          } 

          private int  _id;
           private int  _invoiceid;
           private int  _smuid;
      }
}


