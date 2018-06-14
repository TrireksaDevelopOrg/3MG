using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("pti")]
    internal class pti :BaseNotify  
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

          [DbColumn("TypeOfWeight")] 
          public string TypeOfWeight 
          { 
               get{return _typeofweight;} 
               set{ 

                    SetProperty(ref _typeofweight, value);
                     }
          } 

          [DbColumn("PayType")] 
          public string PayType 
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
          public string ActiveStatus 
          { 
               get{return _activestatus;} 
               set{ 

                    SetProperty(ref _activestatus, value);
                     }
          } 

          private int  _ptinumber;
           private int  _id;
           private int  _shiperid;
           private string  _typeofweight;
           private string  _paytype;
           private double  _etc;
           private string  _note;
           private DateTime  _createddate;
           private string  _activestatus;
      }
}


