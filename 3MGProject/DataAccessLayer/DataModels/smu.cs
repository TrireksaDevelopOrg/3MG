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
          public int Kode 
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

        private bool isSended;
        [DbColumn("IsSended")]

        public bool IsSended
        {
            get { return isSended; }
            set {SetProperty(ref isSended ,value); }

        }

      
        private int ptiId;
        [DbColumn("PTIId")]
        public int PTIId
        {
            get { return ptiId; }
            set { SetProperty(ref ptiId ,value); }
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



        private int  _id;
           private int _kode;
           private DateTime  _createddate;
        private ActivedStatus _activestatus;
    }
}


