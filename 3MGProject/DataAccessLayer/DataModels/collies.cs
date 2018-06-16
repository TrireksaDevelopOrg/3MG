using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;
 
 namespace DataAccessLayer.DataModels 
{ 
     [TableName("collies")]
    public class collies :BaseNotify 
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
                Biaya = (Pcs *Weight) * Price;
            }
          } 

          [DbColumn("Weight")] 
          public double Weight 
          { 
               get{

                return _weight;} 
               set{ 

                    SetProperty(ref _weight, value);
                Biaya = (Pcs * Weight) * Price;
            }
          } 


          [DbColumn("IsSended")] 
          public bool IsSended 
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
                Biaya = (Pcs * Weight) * Price;
            }
          }

        private double _biaya;

        public double Biaya
        {
            get { return _biaya; }
            set {SetProperty(ref _biaya , value); }
        }



        private int  _id;
           private int  _ptiid;
           private int  _pcs;
           private double  _weight;
           private bool  _issended;
           private string  _content;
           private double  _price;
      }
}


