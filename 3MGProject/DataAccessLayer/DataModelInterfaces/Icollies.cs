using System;
 
 namespace DataAccessLayer.DataModels 
{ 

     public interface Icollies  
   {
         int Id {  get; set;} 

         int PtiId {  get; set;} 

         int Pcs {  get; set;} 

         double Weight {  get; set;} 

         double Long {  get; set;} 

         double Hight {  get; set;} 

         double Wide {  get; set;} 

         string TypeOfWeight {  get; set;} 

         string IsSended {  get; set;} 

         string Content {  get; set;} 

         double Price {  get; set;} 

     }
}


