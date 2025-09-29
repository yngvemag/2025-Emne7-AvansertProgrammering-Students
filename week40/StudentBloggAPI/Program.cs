using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StudentBloggAPI.Data;
using StudentBloggAPI.Features.Common.Interfaces;
using StudentBloggAPI.Features.Users;
using StudentBloggAPI.Features.Users.Dtos;
using StudentBloggAPI.Features.Users.Interfaces;
using StudentBloggAPI.Features.Users.Mappers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddScoped<IUserService, UserService>()
    .AddScoped<IUserRepository, UserRepository>();

builder.Services
    .AddScoped<IMapper<UserDto, User>, UserMapper>()
    .AddScoped<IMapper<UserRegistrationDto, User>, UserRegistrationMapper>();

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
    .UseAuthorization();

app.MapControllers();

app.Run();
