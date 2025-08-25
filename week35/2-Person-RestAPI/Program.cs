using PersonRestAPI.Endpoints;
using PersonRestAPI.Models;
using PersonRestAPI.Repository;
using PersonRestAPI.Repository.Interfaces;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IRepository<Person>, PersonInMemoryDatabase>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapPersonEndpoints();

app.Run();

