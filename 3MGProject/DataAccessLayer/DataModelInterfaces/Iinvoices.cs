using System;
 
 namespace DataAccessLayer.DataModels 
{ 

     public interface Iinvoices  
   {
         int Id {  get; set;} 

         int Number {  get; set;} 

         int CustomerId {  get; set;} 

         string InvoiceStatus {  get; set;} 

         DateTime DeadLine {  get; set;} 

         string InvoicePayType {  get; set;} 

         DateTime CreateDate {  get; set;} 

         DateTime PaidDate {  get; set;} 

     }
}


