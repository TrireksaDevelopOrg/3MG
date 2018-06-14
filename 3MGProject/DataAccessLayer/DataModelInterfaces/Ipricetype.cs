using System;
 
 namespace DataAccessLayer.DataModels 
{ 

     public interface Ipricetype  
   {
         int Id {  get; set;} 

         string Name {  get; set;} 

         double Price {  get; set;} 

     }
}


