using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StudentBloggAPI.Data;
using StudentBloggAPI.Features.Common.Interfaces;
using StudentBloggAPI.Features.Users;
using StudentBloggAPI.Features.Users.Dtos;
using StudentBloggAPI.Features.Users.Interfaces;
using StudentBloggAPI.Features.Users.Mappers;
using StudentBloggAPI.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StudentBloggAPI.Health;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// Add services to the container.
builder.Services
    .AddScoped<IUserService, UserService>()
    .AddScoped<IUserRepository, UserRepository>();

builder.Services
    .AddScoped<IMapper<UserDto, User>, UserMapper>()
    .AddScoped<IMapper<UserRegistrationDto, User>, UserRegistrationMapper>();

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, StudentBloggBasicAuthentication>("BasicAuthentication", null);

builder.Services.AddDbContext<StudentBloggDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
});

// Register fluent validation
builder.Services
    .AddValidatorsFromAssemblyContaining<Program>()
    .AddFluentValidationAutoValidation(config => config.DisableDataAnnotationsValidation = true);

// legger til health check
builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database");

builder.Services.AddControllers();

builder.Services.AddOpenApi();

// CORS for Blazor WASM dev origins
const string WasmDevCors = "WasmDevCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(WasmDevCors, policy =>
        policy.WithOrigins(
                "http://localhost:5041", // Blazor WASM dev server ports
                "https://localhost:7041"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
        // .AllowCredentials() // Enable if you later send cookies/authorization headers cross-site
    );
});

var app = builder.Build();

// app.MapHealthChecks("/healthz", new HealthCheckOptions
// {
//     Predicate = r => r.Tags.Contains("live"),
//     ResultStatusCodes =
//     {
//         [HealthStatus.Healthy] = StatusCodes.Status200OK,
//         [HealthStatus.Degraded] = StatusCodes.Status200OK,
//         [HealthStatus.Unhealthy] = StatusCodes.Status200OK
//     }
// });


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app
    .UseCors(WasmDevCors)
    .UseHealthChecks("/healthz")
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();

app.Run();

public abstract partial class Program { }