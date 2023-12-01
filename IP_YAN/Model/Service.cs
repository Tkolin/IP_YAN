using System;

namespace IP_YAN.DateBase;

public class Service
{
    public int Id { get; set; }
    public int TypeOfServiceID { get; set; }
    public DateTime DateOfService { get; set; }
    public int Time { get; set; }
    public decimal Price { get; set; }
    public int BookingID { get; set; }
    public int StaffID { get; set; }
    
    public string TypeOfServiceName { get; set; }
    public string StaffFirstLastName { get; set; }

    public Service(
        int id, int typeOfServiceID, DateTime dateOfService,
        int time, decimal price, int bookingID, 
        int staffID, 
        string typeOfServiceName, string staffFirstLastName)
    {  
        Id = id;
        TypeOfServiceID = typeOfServiceID  ;
        DateOfService = dateOfService  ;
        Time = time  ;
        Price = price  ;
        BookingID = bookingID  ;
        StaffID = staffID  ;
        TypeOfServiceName = typeOfServiceName  ;
        StaffFirstLastName = staffFirstLastName  ;
    }
}