using System;
 
 namespace DataAccessLayer.DataModels 
{ 

     public interface Ideposit  
   {
         int Id {  get; set;} 

         double Jumlah {  get; set;} 

         string PaymentType {  get; set;} 

         int CustomerId {  get; set;} 

         DateTime CreatedDate {  get; set;} 

     }
}


