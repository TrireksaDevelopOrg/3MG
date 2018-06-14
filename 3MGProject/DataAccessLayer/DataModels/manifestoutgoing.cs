using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("manifestoutgoing")]
    internal class manifestoutgoing :BaseNotify  
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

          [DbColumn("SchedulesId")] 
          public int SchedulesId 
          { 
               get{return _schedulesid;} 
               set{ 

                    SetProperty(ref _schedulesid, value);
                     }
          } 

          [DbColumn("Code")] 
          public int Code 
          { 
               get{return _code;} 
               set{ 

                    SetProperty(ref _code, value);
                     }
          } 

          [DbColumn("OnOriginPort")] 
          public DateTime OnOriginPort 
          { 
               get{return _onoriginport;} 
               set{ 

                    SetProperty(ref _onoriginport, value);
                     }
          } 

          [DbColumn("OnDestinationPort")] 
          public DateTime OnDestinationPort 
          { 
               get{return _ondestinationport;} 
               set{ 

                    SetProperty(ref _ondestinationport, value);
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
           private int  _schedulesid;
           private int  _code;
           private DateTime  _onoriginport;
           private DateTime  _ondestinationport;
           private DateTime  _createddate;
      }
}


