using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("smu")]
    internal class smu :BaseNotify  
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

          [DbColumn("Kode")] 
          public string Kode 
          { 
               get{return _kode;} 
               set{ 

                    SetProperty(ref _kode, value);
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

          private int  _id;
           private string  _kode;
           private DateTime  _createddate;
      }
}


