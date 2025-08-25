# Person Rest-Api

[Git-hub for Emne 7 Avansert Programmering](https://github.com/yngvemag/2025-Emne7-AvansertProgrammering-Students.git)

**1. Fullfør koden for PersonInMemoryDatabase**

```csharp
using PersonRestAPI.Models;
using PersonRestAPI.Repository.Interfaces;

namespace PersonRestAPI.Repository;

public class PersonInMemoryDatabase : IRepository<Person>
{
    private static readonly List<Person> DbStorageList = [];
    
    public async Task<Person?> AddAsync(Person item)
    {
        await Task.Delay(10);
        DbStorageList.Add(item);
        return item;
    }

    public Task<Person?> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Person>> GetAllAsync()
    {
        await Task.Delay(10);
        return DbStorageList;
    }

    public Task<Person?> UpdateAsync(Person item)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(long id)
    {
        await Task.Delay(10);
        var person = DbStorageList.FirstOrDefault(x => x.Id == id);
        if (person is null) return false;
        DbStorageList.Remove(person);
        return true;
    }
}
```
---
<div style="page-break-after: always;"></div>

**2. Fullfør koden for PersonEndpoints.cs**
   - Mappings: 
     - MapDelete()
     - MapPut()
  
    Husk å test koden med Postman/Scalar

```csharp
public static class PersonEndpoints
{
    public static void MapPersonEndpoints(this WebApplication app)
    {
        var personGroup = app.MapGroup("/persons");

        personGroup.MapGet("", GetPersonsAsync )
            .WithName("GetAllPersons")
            .WithOpenApi();

        personGroup.MapPost("", AddPersonAsync).WithName("AddPerson").WithOpenApi();
    }
    
    private static async Task<IResult> GetPersonsAsync(IRepository<Person> repo)
    {
        var persons = await repo.GetAllAsync();
        return Results.Ok(persons);
    }

    private static async Task<IResult> AddPersonAsync(
        IRepository<Person> repo, 
        Person person)
    {
        var addedPerson = await repo.AddAsync(person);
        if (addedPerson is null)
            return Results.BadRequest();
        
        return Results.Ok(addedPerson);
    }
    
}
```