using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("manifestdetails")]
    public class manifestdetails :BaseNotify  
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

          [DbColumn("manifestoutgoingId")] 
          public int manifestoutgoingId 
          { 
               get{return _manifestoutgoingid;} 
               set{ 

                    SetProperty(ref _manifestoutgoingid, value);
                     }
          } 

          [DbColumn("SMUId")] 
          public int SMUId 
          { 
               get{return _smu_id;} 
               set{ 

                    SetProperty(ref _smu_id, value);
                     }
          } 

          private int  _id;
           private int  _manifestoutgoingid;
           private int  _smu_id;
      }
}


