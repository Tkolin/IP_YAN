namespace IP_YAN.DateBase;

public class Room
{
    public int Id { get; set; }
    public int RoomTypeID { get; set; }
    public string Desctiption { get; set; }
    public decimal Cost { get; set; }
    public string RoomTypeName { get; set; }

    public Room(
        int id, int roomTypeID, string desctiption,
        decimal cost, string roomTypeName)
    {  
        Id = id;
        RoomTypeID = roomTypeID  ;
        Desctiption = desctiption  ;
        Cost = cost  ;
        RoomTypeName = roomTypeName  ;
    }
}