using System;
 
 namespace DataAccessLayer.DataModels 
{ 
     public interface Ichangehistory  
    {
         int Id {  get; set;} 

         string UserId {  get; set;} 

         string BussinesType {  get; set;} 

         string Note {  get; set;} 

         int BussinessId {  get; set;} 

         string ChangeType {  get; set;} 

         DateTime CreatedDate {  get; set;} 
     }
}


