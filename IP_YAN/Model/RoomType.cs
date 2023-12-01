namespace IP_YAN.DateBase;

public class RoomType
{
    public int Id { get; set; }
    public string Name { get; set; }

    public RoomType(int id, string name)
    {
        Id = id;
        Name = name;
    }
}