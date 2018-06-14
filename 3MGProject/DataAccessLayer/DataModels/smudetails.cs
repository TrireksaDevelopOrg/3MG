using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("smudetails")]
    internal class smudetails :BaseNotify  
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

          [DbColumn("SMUId")] 
          public int SMUId 
          { 
               get{return _smuid;} 
               set{ 

                    SetProperty(ref _smuid, value);
                     }
          } 

          [DbColumn("colliesId")] 
          public int colliesId 
          { 
               get{return _colliesid;} 
               set{ 

                    SetProperty(ref _colliesid, value);
                     }
          } 

          private int  _id;
           private int  _smuid;
           private int  _colliesid;
      }
}


