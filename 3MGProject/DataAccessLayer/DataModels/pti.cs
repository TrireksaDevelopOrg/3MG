using System; 
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("pti")]
    public class pti :BaseNotify  
   {
          [DbColumn("PTINumber")] 
          public int PTINumber 
          { 
               get{return _ptinumber;} 
               set{ 

                    SetProperty(ref _ptinumber, value);
                     }
          } 

          [PrimaryKey("Id")] 
          [DbColumn("Id")] 
          public int Id 
          { 
               get{return _id;} 
               set{ 

                    SetProperty(ref _id, value);
                     }
          } 

          [DbColumn("ShiperID")] 
          public int ShiperID 
          { 
               get{return _shiperid;} 
               set{ 

                    SetProperty(ref _shiperid, value);
                     }
          }


        [DbColumn("RecieverId")]
        public int RecieverId
        {
            get { return _reciever; }
            set
            {

                SetProperty(ref _reciever, value);
            }
        }

        private void SetProperty(ref object reciever, int value)
        {
            throw new NotImplementedException();
        }

        [DbColumn("PayType")] 
          public PayType PayType 
          { 
               get{return _paytype;} 
               set{ 

                    SetProperty(ref _paytype, value);
                     }
          } 

          [DbColumn("Etc")] 
          public double Etc 
          { 
               get{return _etc;} 
               set{ 

                    SetProperty(ref _etc, value);
                     }
          } 

          [DbColumn("Note")] 
          public string Note 
          { 
               get{return _note;} 
               set{ 

                    SetProperty(ref _note, value);
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

          [DbColumn("ActiveStatus")] 
          public ActivedStatus ActiveStatus 
          { 
               get{return _activestatus;} 
               set{ 

                    SetProperty(ref _activestatus, value);
                     }
          }

            public ObservableCollection<collies> Collies { get; set; }

        public customer Shiper
        {
            get { return shiper;}
            set
            {
                SetProperty(ref shiper, value);
            }
        }

        public customer Reciever
        {
            get { return reciever; }
            set
            {
                SetProperty(ref reciever, value);
            }
        }


        private string _code;

        public string Code
        {
            get {
                if(string.IsNullOrEmpty(_code))
                {
                    _code= CodeGenerate.PTI(PTINumber);
                }
                return _code;
            }
            set {SetProperty(ref _code ,value); }
        }


        private int  _ptinumber;
           private int  _id;
           private int  _shiperid;
           private PayType _paytype;
           private double  _etc;
           private string  _note;
           private DateTime  _createddate;
           private ActivedStatus _activestatus;
        private int _reciever;
        private customer shiper;
        private customer reciever;
    }
}


