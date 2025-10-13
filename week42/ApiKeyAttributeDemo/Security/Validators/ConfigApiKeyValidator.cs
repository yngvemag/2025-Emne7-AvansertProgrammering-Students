using ApiKeyAttributeDemo.Security.Options;
using Microsoft.Extensions.Options;

namespace ApiKeyAttributeDemo.Security.Validators;

public class ConfigApiKeyValidator(IOptions<ApiKeyOptions> options) 
    : IApiKeyValidator
{
    private readonly IOptions<ApiKeyOptions> _options = options;

    public async Task<ApiKeyValidationResult> ValidateAsync(string apiKey, CancellationToken ct = default)
    {
        var match = _options.Value.Keys.FirstOrDefault(k => k.Key == apiKey);
        if (match is null)
        {
            return await Task.FromResult(new ApiKeyValidationResult(
                IsValid: false,
                AppId: string.Empty,
                Scopes: []));
        }

        return await Task.FromResult(new ApiKeyValidationResult(
            IsValid: true,
            AppId: match.AppId,
            Scopes: match.Scopes));

    }
}