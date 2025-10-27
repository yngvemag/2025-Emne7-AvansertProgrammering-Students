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

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();

app.Run();

public abstract partial class Program { }