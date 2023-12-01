namespace IP_YAN.DateBase;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Role(int id, string name)
    {
        Id = id;
        Name = name;
    }
}