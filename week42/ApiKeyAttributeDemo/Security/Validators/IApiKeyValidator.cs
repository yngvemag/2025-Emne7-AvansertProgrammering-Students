namespace ApiKeyAttributeDemo.Security.Validators;

public record ApiKeyValidationResult(
    bool IsValid,
    string? AppId,
    IEnumerable<string>? Scopes);


public interface IApiKeyValidator
{
    Task<ApiKeyValidationResult> ValidateAsync(
        string apiKey, 
        CancellationToken ct = default);
}