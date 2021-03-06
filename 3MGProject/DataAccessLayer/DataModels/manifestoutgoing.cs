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



          [DbColumn("CreatedDate")] 
          public DateTime CreatedDate 
          { 
               get{return _createddate;} 
               set{ 

                    SetProperty(ref _createddate, value);
                     }
          }




        private bool isTakeOff;

        [DbColumn("IsTakeOff")]
        public bool IsTakeOff
        {
            get { return isTakeOff; }
            set { isTakeOff = value; }
        }

        [DbColumn("ActiveStatus")]
        public ActivedStatus ActiveStatus
        {
            get { return _activestatus; }
            set
            {

                SetProperty(ref _activestatus, value);
            }
        }


       
        public string User { get;  set; }

        private int  _id;
           private int  _schedulesid;
           private DateTime  _createddate;
        private ActivedStatus _activestatus;
    }
}


