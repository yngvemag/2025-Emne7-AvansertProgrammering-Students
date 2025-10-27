using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using StudentBloggAPI.Data;
using StudentBloggAPI.Features.Users.Interfaces;

namespace StudentBloggAPI.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IUserRepository> UserRepository { get; } = new();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            var descriptorsToRemove = services
                .Where(d => d.ServiceType == typeof(StudentBloggDbContext) ||
                            d.ServiceType == typeof(IUserRepository) ||
                            d.ServiceType.Name.Contains("dbContext"))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
                services.Remove(descriptor);
            
            // legger til mock versjon
            services.AddSingleton(UserRepository.Object);
        });
    }
}