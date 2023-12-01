using System;

namespace IP_YAN.DateBase;

public class Staff
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    
    public int RoleID  { get; set; }
    public string RoleName { get; set; }

    public Staff (
        int id, string firstName, string lastName,
        string phoneNumber, DateTime dateOfBirth,
        int roleId, string roleName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        RoleID = roleId;
        RoleName = roleName;
    }

}