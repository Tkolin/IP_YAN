namespace IP_YAN.DateBase;

public class TypeOfService
{
    public int Id { get; set; }
    public string Name { get; set; }
    public TypeOfService(int id, string name)
    {
        Id = id;
        Name = name;
    }
}