using System;
 
 namespace DataAccessLayer.DataModels 
{ 

     public interface Ipti  
   {
         int PTINumber {  get; set;} 

         int Id {  get; set;} 

         int ShiperID {  get; set;} 

         string TypeOfWeight {  get; set;} 

         string PayType {  get; set;} 

         double Etc {  get; set;} 

         string Note {  get; set;} 

         DateTime CreatedDate {  get; set;} 

         string ActiveStatus {  get; set;} 

     }
}


