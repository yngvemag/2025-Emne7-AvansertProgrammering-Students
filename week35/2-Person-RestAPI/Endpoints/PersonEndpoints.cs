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