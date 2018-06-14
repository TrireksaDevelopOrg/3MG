using System;
 
 namespace DataAccessLayer.DataModels 
{ 

     public interface Ischedules  
   {
         int Id {  get; set;} 

         DateTime Tanggal {  get; set;} 

         TimeSpan Start {  get; set;} 

         TimeSpan End {  get; set;} 

         int PlaneId {  get; set;} 

         string Complete {  get; set;} 

         int PortId {  get; set;} 

         DateTime CreatedDate {  get; set;} 

     }
}


