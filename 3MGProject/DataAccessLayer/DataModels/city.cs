using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("city")]
    internal class city :BaseNotify  ,Icity
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

          [DbColumn("CityName")] 
          public string CityName 
          { 
               get{return _cityname;} 
               set{ 

                    SetProperty(ref _cityname, value);
                     }
          } 

          [DbColumn("CityCode")] 
          public string CityCode 
          { 
               get{return _citycode;} 
               set{ 

                    SetProperty(ref _citycode, value);
                     }
          } 

          private int  _id;
           private string  _cityname;
           private string  _citycode;
      }
}


