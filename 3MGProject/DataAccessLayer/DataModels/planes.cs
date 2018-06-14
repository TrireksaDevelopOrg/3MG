using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("planes")]
    internal class planes :BaseNotify  
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

          [DbColumn("Name")] 
          public string Name 
          { 
               get{return _name;} 
               set{ 

                    SetProperty(ref _name, value);
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

          private int  _id;
           private string  _name;
           private string  _kode;
      }
}


