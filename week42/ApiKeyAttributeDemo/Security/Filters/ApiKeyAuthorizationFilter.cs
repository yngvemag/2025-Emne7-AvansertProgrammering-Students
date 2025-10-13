using ApiKeyAttributeDemo.Security.Options;
using ApiKeyAttributeDemo.Security.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace ApiKeyAttributeDemo.Security.Filters;

public class ApiKeyAuthorizationFilter(IApiKeyValidator validator, IOptions<ApiKeyOptions> options)
    : IAsyncAuthorizationFilter
{
    private readonly IOptions<ApiKeyOptions> _options = options;
    private readonly IApiKeyValidator _validator = validator;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        string headerName = _options.Value.HeaderName ?? "X-API-KEY";
        bool hasHeader = context.HttpContext.Request.Headers.TryGetValue(headerName, out var headerValue);
        
        // Check if the header is present
        if(!hasHeader || string.IsNullOrWhiteSpace(headerValue))
        {
            context.HttpContext.Response.StatusCode = 401; // Unauthorized
            await context.HttpContext.Response.WriteAsync("API Key is missing");
            return;
        }
        
        // validate api-key
        ApiKeyValidationResult result = await _validator.ValidateAsync(headerValue!);

        if (!result.IsValid)
        {
            context.Result = new UnauthorizedObjectResult(new {error = "Invalid API Key"});
            return;
        }
        
        context.HttpContext.Items["AppId"] = result.AppId;
        context.HttpContext.Items["Scopes"] = result.Scopes?.ToList() ?? [];
    }
}