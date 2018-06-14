using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("ports")]
    internal class ports :BaseNotify  
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

          [DbColumn("Code")] 
          public string Code 
          { 
               get{return _code;} 
               set{ 

                    SetProperty(ref _code, value);
                     }
          } 

          [DbColumn("City")] 
          public string City 
          { 
               get{return _city;} 
               set{ 

                    SetProperty(ref _city, value);
                     }
          } 

          [DbColumn("portscol")] 
          public string portscol 
          { 
               get{return _portscol;} 
               set{ 

                    SetProperty(ref _portscol, value);
                     }
          } 

          [DbColumn("CityId")] 
          public int CityId 
          { 
               get{return _cityid;} 
               set{ 

                    SetProperty(ref _cityid, value);
                     }
          } 

          private int  _id;
           private string  _name;
           private string  _code;
           private string  _city;
           private string  _portscol;
           private int  _cityid;
      }
}


