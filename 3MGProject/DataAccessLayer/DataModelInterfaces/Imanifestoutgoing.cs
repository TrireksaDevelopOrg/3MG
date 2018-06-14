using System;
 
 namespace DataAccessLayer.DataModels 
{ 

     public interface Imanifestoutgoing  
   {
         int Id {  get; set;} 

         int SchedulesId {  get; set;} 

         int Code {  get; set;} 

         DateTime OnOriginPort {  get; set;} 

         DateTime OnDestinationPort {  get; set;} 

         DateTime CreatedDate {  get; set;} 

     }
}


