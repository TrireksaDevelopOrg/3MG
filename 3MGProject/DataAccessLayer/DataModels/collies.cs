using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("collies")]
    internal class collies :BaseNotify ,Icollies 
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

          [DbColumn("PtiId")] 
          public int PtiId 
          { 
               get{return _ptiid;} 
               set{ 

                    SetProperty(ref _ptiid, value);
                     }
          } 

          [DbColumn("Pcs")] 
          public int Pcs 
          { 
               get{return _pcs;} 
               set{ 

                    SetProperty(ref _pcs, value);
                     }
          } 

          [DbColumn("Weight")] 
          public double Weight 
          { 
               get{return _weight;} 
               set{ 

                    SetProperty(ref _weight, value);
                     }
          } 

          [DbColumn("Long")] 
          public double Long 
          { 
               get{return _long;} 
               set{ 

                    SetProperty(ref _long, value);
                     }
          } 

          [DbColumn("Hight")] 
          public double Hight 
          { 
               get{return _hight;} 
               set{ 

                    SetProperty(ref _hight, value);
                     }
          } 

          [DbColumn("Wide")] 
          public double Wide 
          { 
               get{return _wide;} 
               set{ 

                    SetProperty(ref _wide, value);
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

          [DbColumn("IsSended")] 
          public string IsSended 
          { 
               get{return _issended;} 
               set{ 

                    SetProperty(ref _issended, value);
                     }
          } 

          [DbColumn("Content")] 
          public string Content 
          { 
               get{return _content;} 
               set{ 

                    SetProperty(ref _content, value);
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
           private int  _ptiid;
           private int  _pcs;
           private double  _weight;
           private double  _long;
           private double  _hight;
           private double  _wide;
           private string  _typeofweight;
           private string  _issended;
           private string  _content;
           private double  _price;
      }
}


