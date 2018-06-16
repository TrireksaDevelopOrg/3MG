using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("customer")]
    public class customer :BaseNotify
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

          [DbColumn("ContactName")] 
          public string ContactName 
          { 
               get{return _contactname;} 
               set{ 

                    SetProperty(ref _contactname, value);
                     }
          } 

          [DbColumn("Phone1")] 
          public string Phone1 
          { 
               get{return _phone1;} 
               set{ 

                    SetProperty(ref _phone1, value);
                     }
          } 

          [DbColumn("Phone2")] 
          public string Phone2 
          { 
               get{return _phone2;} 
               set{ 

                    SetProperty(ref _phone2, value);
                     }
          } 

          [DbColumn("Handphone")] 
          public string Handphone 
          { 
               get{return _handphone;} 
               set{ 

                    SetProperty(ref _handphone, value);
                     }
          } 

          [DbColumn("Address")] 
          public string Address 
          { 
               get{return _address;} 
               set{ 

                    SetProperty(ref _address, value);
                     }
          } 

          [DbColumn("Email")] 
          public string Email 
          { 
               get{return _email;} 
               set{ 

                    SetProperty(ref _email, value);
                     }
          }



        private CustomerType customerType;
        [DbColumn("CustomerType")]
        public CustomerType CustomerType
        {
            get { return customerType; }
            set { SetProperty(ref customerType , value); }
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
           private string  _name;
           private string  _contactname;
           private string  _phone1;
           private string  _phone2;
           private string  _handphone;
           private string  _address;
           private string  _email;
           private DateTime  _createddate;
      }
}


