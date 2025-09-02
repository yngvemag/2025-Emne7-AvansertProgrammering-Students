using Microsoft.AspNetCore.Mvc;
using PersonRestAPI.Models;
using PersonRestAPI.Repository.Interfaces;

namespace PersonRestAPI.Endpoints;

public static class PersonEndpoints
{
    public static void MapPersonEndpoints(this WebApplication app)
    {
        var personGroup = app.MapGroup("/persons");

        personGroup.MapGet("", GetPersonsAsync )
            .WithName("GetAllPersons")
            .WithOpenApi();
        

        personGroup.MapPost("", AddPersonAsync).WithName("AddPerson").WithOpenApi();
        
        // http://localhost:5000/persons/3
        personGroup.MapPut("/{id:long}", UpdatePersonAsync).WithName("UpdatePerson").WithOpenApi();
        
        personGroup.MapDelete("/{id:long}", DeletePersonAsync).WithName("DeletePerson").WithOpenApi();
    }
    
    
    // http://localhost:5000/persons?id=3
    private static async Task<IResult> GetPersonsAsync(
        [FromServices] IRepository<Person> repo,
        [FromQuery] long? id)
    {
        var persons = await repo.GetAllAsync();
        return id is null
            ? Results.Ok(persons)
            : Results.Ok(persons.Where(p => p.Id == id));
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
    
    private static async Task<IResult> UpdatePersonAsync(
        [FromServices] IRepository<Person> repo,
        [FromRoute] long id,
        [FromBody] Person person)
    {
        person.Id = id;
        Person? updatedPerson = await repo.UpdateAsync(person);
        return updatedPerson is null 
            ? Results.NotFound("Person not found") 
            : Results.Ok(updatedPerson);
    }
    
    private static async Task<IResult> DeletePersonAsync(
        [FromServices] IRepository<Person> repo,
        [FromRoute] long id)
    {
        var deleted = await repo.DeleteAsync(id);
        return deleted 
            ? Results.Ok() 
            : Results.NotFound("Person not found");
    }
}