using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("manifestoutgoing")]
    public class manifestoutgoing :BaseNotify  
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


          [DbColumn("CreatedDate")] 
          public DateTime CreatedDate 
          { 
               get{return _createddate;} 
               set{ 

                    SetProperty(ref _createddate, value);
                     }
          }

        private string codeManifest;

        public string ManifestCode
        {
            get {
                if (string.IsNullOrEmpty(codeManifest))
                    codeManifest = CodeGenerate.Manifest(Code);
                return codeManifest; }
            set { SetProperty(ref codeManifest ,value); }
        }

        public string User { get; internal set; }

        private int  _id;
           private int  _schedulesid;
           private int  _code;
           private DateTime  _createddate;
      }
}


