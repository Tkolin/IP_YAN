using System;

namespace IP_YAN.DateBase;

public class Booking
{
    public int Id { get; set; }
    public int RoomID { get; set; }
    public int VisitorID { get; set; }
    public decimal FinalPrice { get; set; }
    public DateTime DateOfEntry { get; set; }
    public DateTime DateOfDeparture { get; set; }
    public string VisitorFirstLastName { get; set; }

    public Booking(
        int id, int roomID, int visitorID,
        decimal finalPrice, DateTime dateOfEntry, DateTime dateOfDeparture,
        string visitorFirstLastName)
    {  
        Id = id;
        RoomID = roomID  ;
        VisitorID = visitorID  ;
        FinalPrice = finalPrice  ;
        DateOfEntry = dateOfEntry  ;
        DateOfDeparture = dateOfDeparture  ;
        VisitorFirstLastName = visitorFirstLastName  ;
    }
    public Booking(
        int id, int roomID, int visitorID,
        decimal finalPrice, DateTime dateOfEntry, DateTime dateOfDeparture)
    {  
        Id = id;
        RoomID = roomID  ;
        VisitorID = visitorID  ;
        FinalPrice = finalPrice  ;
        DateOfEntry = dateOfEntry  ;
        DateOfDeparture = dateOfDeparture  ;
    }
}