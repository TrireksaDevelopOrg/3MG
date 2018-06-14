using System;
 
 namespace DataAccessLayer.DataModels 
{ 

     public interface Idebetdeposit  
   {
         int Id {  get; set;} 

         int SMUId {  get; set;} 

         DateTime CreatedDate {  get; set;} 

     }
}


