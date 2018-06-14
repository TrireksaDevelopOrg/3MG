using System;
 
 namespace DataAccessLayer.DataModels 
{ 

     public interface Icustomer  
   {
         int Id {  get; set;} 

         string Name {  get; set;} 

         string ContactName {  get; set;} 

         string Phone1 {  get; set;} 

         string Phone2 {  get; set;} 

         string Handphone {  get; set;} 

         string Address {  get; set;} 

         string Email {  get; set;} 

         DateTime CreatedDate {  get; set;} 

     }
}


