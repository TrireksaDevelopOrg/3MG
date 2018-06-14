using System;
 
 namespace DataAccessLayer.DataModels 
{ 

     public interface Iinvoicedetail  
   {
         int Id {  get; set;} 

         int InvoiceId {  get; set;} 

         int SMUId {  get; set;} 

     }
}


