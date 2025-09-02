using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cms;
using PersonRestAPI.Models;
using PersonRestAPI.Repository.Interfaces;

namespace PersonRestAPI.Repository;

public class MySqlPersonRepository(
    IConfiguration config, 
    ILogger<MySqlPersonRepository> logger) : IRepository<Person>
{
    private string _connectionString = config.GetConnectionString("DefaultConnection")
                                       ?? throw new Exception("Connection string not found");
    

    public async Task<Person?> AddAsync(Person item)
    {
        await using MySqlConnection conn = new(_connectionString);
        conn.Open();

        string query = "INSERT INTO person (first_name, last_name, age) VALUES (@FirstName, @LastName, @Age)";
        MySqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@FirstName", item.FirstName);
        cmd.Parameters.AddWithValue("@LastName", item.LastName);
        cmd.Parameters.AddWithValue("@Age", item.Age);
        
        await cmd.ExecuteNonQueryAsync();

        cmd.CommandText = "SELECT LAST_INSERT_ID()";
        item.Id = Convert.ToInt64(cmd.ExecuteScalar());
        return item;
    }

    public async Task<Person?> GetByIdAsync(long id)
    {
        await using MySqlConnection conn = new(_connectionString);
        conn.Open();
        
        string query = "SELECT person_id, first_name, last_name, age FROM person where person_id = @id";
        MySqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            return new Person()
            {
                Id = reader["person_id"] as long? ?? 0,
                FirstName = reader["first_name"] as string ?? string.Empty,
                LastName = reader["last_name"] as string ?? string.Empty,
                Age = reader["age"] as int? ?? 0
                //FirstName = reader.GetString(1),
                //LastName = reader.GetString(2),
                //Age = reader.GetInt32(3)
            };
        }

        return null;
    }

    public async Task<IEnumerable<Person>> GetAllAsync()
    {
        List<Person> persons = new();
        await using MySqlConnection conn = new(_connectionString);
        conn.Open();
        
        string query = "SELECT person_id, first_name, last_name, age FROM person";
        MySqlCommand cmd = new(query, conn);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            persons.Add(new Person()
            {
                Id = reader.GetInt64(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Age = reader.GetInt32(3)
            });
        }
        return persons;
    }

    public async Task<Person?> UpdateAsync(Person item)
    {
        await using MySqlConnection conn = new(_connectionString);
        conn.Open();
        
        string query = "UPDATE person SET first_name = @FirstName, last_name = @LastName, age = @Age WHERE person_id = @Id";
        MySqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@FirstName", item.FirstName);
        cmd.Parameters.AddWithValue("@LastName", item.LastName);
        cmd.Parameters.AddWithValue("@Age", item.Age);
        cmd.Parameters.AddWithValue("@Id", item.Id);
        
        int rowsAffected = await cmd.ExecuteNonQueryAsync();
        
        return rowsAffected > 0 
            ? item 
            : null;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        await using MySqlConnection conn = new(_connectionString);
        conn.Open();
        
        string query = "DELETE FROM person WHERE person_id = @Id";
        MySqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@Id", id);
        
        int rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
        
    }
}