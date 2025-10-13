using ApiKeyAttributeDemo.Security.Filters;
using ApiKeyAttributeDemo.Security.Options;
using ApiKeyAttributeDemo.Security.Validators;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiKeyAttributeDemo.Security.Extensions;

public static class ApiKeyServiceCollectionExtensions
{
    public static IServiceCollection AddApiKeyAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiKeyOptions>(configuration.GetSection(nameof(ApiKeyOptions)));
        //services.Configure<ApiKeyOptions>(configuration.GetSection(ApiKeyOptions.SectionName));
        
        services.AddScoped<IApiKeyValidator, ConfigApiKeyValidator>();
        services.AddScoped<IAsyncAuthorizationFilter,ApiKeyAuthorizationFilter>();
        
        return services;
    }
}