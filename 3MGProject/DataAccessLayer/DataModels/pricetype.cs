using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("pricetype")]
    internal class pricetype :BaseNotify  
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

          [DbColumn("Price")] 
          public double Price 
          { 
               get{return _price;} 
               set{ 

                    SetProperty(ref _price, value);
                     }
          } 

          private int  _id;
           private string  _name;
           private double  _price;
      }
}


