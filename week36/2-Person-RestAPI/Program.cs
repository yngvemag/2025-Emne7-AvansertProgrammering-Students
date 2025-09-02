using PersonRestAPI.Endpoints;
using PersonRestAPI.Middleware;
using PersonRestAPI.Models;
using PersonRestAPI.Repository;
using PersonRestAPI.Repository.Interfaces;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddScoped<IRepository<Person>, MySqlPersonRepository>()
    .AddExceptionHandler<GlobalExceptionHandler>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// use serilog via appsettings.json
builder.Host.UseSerilog(
    (context, configuration) => configuration.ReadFrom.Configuration(context.Configuration)
    );

// Log.Logger = new LoggerConfiguration()
//     .MinimumLevel.Information()
//     .Enrich.FromLogContext()
//     .WriteTo.Console()
//     .WriteTo.File("logs/log.txt")
//     .CreateLogger();
//
// builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app
    .UseHttpsRedirection()
    .UseExceptionHandler(_ => { });

app.MapPersonEndpoints();

app.Run();

