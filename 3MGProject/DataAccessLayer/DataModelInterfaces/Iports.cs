using System;
 
 namespace DataAccessLayer.DataModels 
{ 

     public interface Iports  
   {
         int Id {  get; set;} 

         string Name {  get; set;} 

         string Code {  get; set;} 

         string City {  get; set;} 

         string portscol {  get; set;} 

         int CityId {  get; set;} 

     }
}


