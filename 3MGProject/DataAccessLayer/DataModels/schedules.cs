using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("schedules")]
    public class schedules :BaseNotify  
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

          [DbColumn("Tanggal")] 
          public DateTime Tanggal 
          { 
               get{return _tanggal;} 
               set{ 

                    SetProperty(ref _tanggal, value);
                     }
          } 

          [DbColumn("Start")] 
          public TimeSpan Start 
          { 
               get{return _start;} 
               set{ 

                    SetProperty(ref _start, value);
                     }
          } 

          [DbColumn("End")] 
          public TimeSpan End 
          { 
               get{return _end;} 
               set{ 

                    SetProperty(ref _end, value);
                     }
          } 

          [DbColumn("PlaneId")] 
          public int PlaneId 
          { 
               get{return _planeid;} 
               set{ 

                    SetProperty(ref _planeid, value);
                     }
          } 

          [DbColumn("Complete")] 
          public bool Complete 
          { 
               get{return _complete;} 
               set{ 

                    SetProperty(ref _complete, value);
                     }
          } 

          [DbColumn("PortFrom")] 
          public int PortFrom 
          { 
               get{return _portid;} 
               set{ 

                    SetProperty(ref _portid, value);
                     }
          }

        [DbColumn("PortTo")]
        public int PortTo
        {
            get { return _portTo; }
            set
            {

                SetProperty(ref _portTo, value);
            }
        }


        private double _capacities;

        [DbColumn("Capasities")]
        public double Capacities
        {
            get { return _capacities; }
            set { SetProperty(ref _capacities , value); }
        }

      

        [DbColumn("FlightNumber")]
        public string FlightNumber
        {
            get {
                return flightNumber; }
            set { SetProperty(ref flightNumber ,value); }
        }

        [DbColumn("CreatedDate")] 
          public DateTime CreatedDate 
          { 
               get{return _createddate;} 
               set{ 

                    SetProperty(ref _createddate, value);
                     }
          }

        public virtual string User { get; set; }

        
        private int  _id;
           private DateTime  _tanggal;
           private TimeSpan  _start;
           private TimeSpan  _end;
           private int  _planeid;
           private bool  _complete;
           private int  _portid;
           private DateTime  _createddate;
        private int _portTo;
        private string flightNumber;
    }
}


