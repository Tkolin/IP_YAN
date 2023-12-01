using System;
using System.Collections.Generic;
using System.Linq;
using IP_YAN.DateBase;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySqlConnector;

namespace IP_YAN.Model;

public class DataBaseManager
{
    /// Настройки подключения
    public static MySqlConnectionStringBuilder ConnectionString =
        new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            Database = "roomdb",
            UserID = "root",
            Password = "tkl909" // "tkl909"//"nrjkby99"
        };

    // Получение
    public static List<T> GetData<T>(string query, Func<MySqlDataReader, T> mapFunction)
    {
        List<T> data = new List<T>();
        using (var connection = new MySqlConnection(ConnectionString.ConnectionString))
        {
            connection.Open();
            using (var comand = connection.CreateCommand())
            {
                comand.CommandText = query;
                using (var reader = comand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(mapFunction(reader));
                    }
                }
            }

            connection.Close();
        }

        return data;
    }

    public static List<RoomType> GetRoomTypes()
    {
        string query = "SELECT * FROM room_type";
        return GetData(query, reader => new RoomType(
            reader.GetInt32("ID"),
            reader.GetString("name")
        ));
    }

    public static List<Role> GetRoles()
    {
        string query = "SELECT * FROM role";
        return GetData(query, reader => new Role(
            reader.GetInt32("ID"),
            reader.GetString("name")
        ));
    }

    public static List<TypeOfService> GetTypeOfServices()
    {
        string query = "SELECT * FROM type_of_service";
        return GetData(query, reader => new TypeOfService(
            reader.GetInt32("ID"),
            reader.GetString("name")
        ));
    }

    public static List<Room> GetRooms()
    {
        string query = @"SELECT r.ID, r.room_type_id, r.description, r.cost, rt.name as roomTypeName
                     FROM room r
                     INNER JOIN room_type rt ON r.room_type_id = rt.ID";
        return GetData(query, reader => new Room(
            reader.GetInt32("ID"),
            reader.GetInt32("room_type_id"),
            reader.GetString("description"),
            reader.GetDecimal("cost"),
            reader.GetString("roomTypeName")
        ));
    }

    public static List<Visitor> GetVisitors()
    {
        string query = "SELECT * FROM visitor";
        return GetData(query, reader => new Visitor(
            reader.GetInt32("ID"),
            reader.GetString("first_name"),
            reader.GetString("last_name"),
            reader.GetString("phone_number"),
            reader.GetDateTime("date_of_birth")
        ));
    }

    public static List<Staff> GetStaff()
    {
        string query = @"SELECT s.ID, s.first_name, s.last_name, s.phone_number, 
                            s.date_of_birth, s.role_ID, r.name as roleName
                     FROM staff s
                     INNER JOIN role r ON s.role_ID = r.ID";
        return GetData(query, reader => new Staff(
            reader.GetInt32("ID"),
            reader.GetString("first_name"),
            reader.GetString("last_name"),
            reader.GetString("phone_number"),
            reader.GetDateTime("date_of_birth"),
            reader.GetInt32("role_ID"),
            reader.GetString("roleName")
        ));
    }

    public static List<Booking> GetBookings()
    {
        string query = @"SELECT b.ID, b.room_ID, b.visitor_ID, b.final_price,
                            b.date_of_entry, b.date_of_departure,
                            CONCAT(v.first_name, ' ', v.last_name) as visitorFirstLastName
                     FROM booking b
                     INNER JOIN visitor v ON b.visitor_ID = v.ID";
        return GetData(query, reader => new Booking(
            reader.GetInt32("ID"),
            reader.GetInt32("room_ID"),
            reader.GetInt32("visitor_ID"),
            reader.GetDecimal("final_price"),
            reader.GetDateTime("date_of_entry"),
            reader.GetDateTime("date_of_departure"),
            reader.GetString("visitorFirstLastName")
        ));
    }

    public static List<Service> GetServices()
    {
        string query = @"SELECT s.ID, s.type_of_service_ID, s.date_of_service, s.time, s.price,
                            s.booking_ID, s.staff_ID, ts.name as typeOfServiceName,
                            CONCAT(st.first_name, ' ', st.last_name) as staffFirstLastName
                     FROM service s
                     INNER JOIN type_of_service ts ON s.type_of_service_ID = ts.ID
                     INNER JOIN staff st ON s.staff_ID = st.ID";
        return GetData(query, reader => new Service(
            reader.GetInt32("ID"),
            reader.GetInt32("type_of_service_ID"),
            reader.GetDateTime("date_of_service"),
            reader.GetInt32("time"),
            reader.GetDecimal("price"),
            reader.GetInt32("booking_ID"),
            reader.GetInt32("staff_ID"),
            reader.GetString("typeOfServiceName"),
            reader.GetString("staffFirstLastName")
        ));
    }

    /// Добавление Удаление Обнавление
    public static void AddEntity<T>(T entity, string tableName, Dictionary<string, object> parameters)
    {
        string columns = string.Join(", ", parameters.Keys);
        string values = string.Join(", ", parameters.Keys.Select(key => "@" + key));

        string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

        using (var connection = new MySqlConnection(ConnectionString.ConnectionString))
        {
            connection.Open();
            using (var command = new MySqlCommand(query, connection))
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue("@" + parameter.Key, parameter.Value);
                }

                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public static void UpdateEntity<T>(T entity, string tableName, Dictionary<string, object> parameters, string idColumnName)
    {
        string setClause = string.Join(", ", parameters.Keys.Select(key => $"{key} = @{key}"));
        string query = $"UPDATE {tableName} SET {setClause} WHERE {idColumnName} = @{idColumnName}";

        using (var connection = new MySqlConnection(ConnectionString.ConnectionString))
        {
            connection.Open();
            using (var command = new MySqlCommand(query, connection))
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue("@" + parameter.Key, parameter.Value);
                }
                command.Parameters.AddWithValue("@" + idColumnName, parameters[idColumnName]);

                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public static void DeleteEntity(string tableName, string idColumnName, int id)
    {
        string query = $"DELETE FROM {tableName} WHERE {idColumnName} = @ID";

        using (var connection = new MySqlConnection(ConnectionString.ConnectionString))
        {
            connection.Open();
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ID", id);

                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
    public static void AddRoomType(RoomType roomType)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "name", roomType.Name }
        };

        AddEntity(roomType, "room_type", parameters);
    }

    public static void UpdateRoomType(RoomType roomType)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "name", roomType.Name }
        };

        UpdateEntity(roomType, "room_type", parameters, "ID");
    }

    public static void DeleteRoomType(int roomTypeId)
    {
        DeleteEntity("room_type", "ID", roomTypeId);
    }

// Аналогичные методы для остальных таблиц

    public static void AddRole(Role role)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "name", role.Name }
        };

        AddEntity(role, "role", parameters);
    }

    public static void UpdateRole(Role role)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "name", role.Name }
        };

        UpdateEntity(role, "role", parameters, "ID");
    }

    public static void DeleteRole(int roleId)
    {
        DeleteEntity("role", "ID", roleId);
    }

    public static void AddTypeOfService(TypeOfService typeOfService)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "name", typeOfService.Name }
        };

        AddEntity(typeOfService, "type_of_service", parameters);
    }

    public static void UpdateTypeOfService(TypeOfService typeOfService)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "name", typeOfService.Name }
        };

        UpdateEntity(typeOfService, "type_of_service", parameters, "ID");
    }

    public static void DeleteTypeOfService(int typeOfServiceId)
    {
        DeleteEntity("type_of_service", "ID", typeOfServiceId);
    }

    public static void AddVisitor(Visitor visitor)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "first_name", visitor.FirstName },
            { "last_name", visitor.LastName },
            { "phone_number", visitor.PhoneNumber },
            { "date_of_birth", visitor.DateOfBirth }
        };

        AddEntity(visitor, "visitor", parameters);
    }

    public static void UpdateVisitor(Visitor visitor)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "first_name", visitor.FirstName },
            { "last_name", visitor.LastName },
            { "phone_number", visitor.PhoneNumber },
            { "date_of_birth", visitor.DateOfBirth }
        };

        UpdateEntity(visitor, "visitor", parameters, "ID");
    }

    public static void DeleteVisitor(int visitorId)
    {
        DeleteEntity("visitor", "ID", visitorId);
    }
    public static void AddStaff(Staff staff)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "first_name", staff.FirstName },
            { "last_name", staff.LastName },
            { "phone_number", staff.PhoneNumber },
            { "date_of_birth", staff.DateOfBirth },
            { "role_ID", staff.RoleID }
        };

        AddEntity(staff, "staff", parameters);
    }

    public static void UpdateStaff(Staff staff)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "first_name", staff.FirstName },
            { "last_name", staff.LastName },
            { "phone_number", staff.PhoneNumber },
            { "date_of_birth", staff.DateOfBirth },
            { "role_ID", staff.RoleID }
        };

        UpdateEntity(staff, "staff", parameters, "ID");
    }

    public static void DeleteStaff(int staffId)
    {
        DeleteEntity("staff", "ID", staffId);
    }
    public static void AddBooking(Booking booking)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "room_ID", booking.RoomID },
            { "date_of_entry", booking.DateOfEntry },
            { "date_of_departure", booking.DateOfDeparture },
            { "visitor_ID", booking.VisitorID },
            { "final_price", booking.FinalPrice }
        };

        AddEntity(booking, "booking", parameters);
    }

    public static void UpdateBooking(Booking booking)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "room_ID", booking.RoomID },
            { "date_of_entry", booking.DateOfEntry },
            { "date_of_departure", booking.DateOfDeparture },
            { "visitor_ID", booking.VisitorID },
            { "final_price", booking.FinalPrice }
        };

        UpdateEntity(booking, "booking", parameters, "ID");
    }

    public static void DeleteBooking(Booking? bookingId)
    {
        DeleteEntity("booking", "ID", bookingId);
    }
    public static void AddService(Service service)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "type_of_service_ID", service.TypeOfServiceID },
            { "date_of_service", service.DateOfService },
            { "time", service.Time },
            { "price", service.Price },
            { "booking_ID", service.BookingID },
            { "staff_ID", service.StaffID }
        };

        AddEntity(service, "service", parameters);
    }

    public static void UpdateService(Service service)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "type_of_service_ID", service.TypeOfServiceID },
            { "date_of_service", service.DateOfService },
            { "time", service.Time },
            { "price", service.Price },
            { "booking_ID", service.BookingID },
            { "staff_ID", service.StaffID }
        };

        UpdateEntity(service, "service", parameters, "ID");
    }

    public static void DeleteService(int serviceId)
    {
        DeleteEntity("service", "ID", serviceId);
    }
}