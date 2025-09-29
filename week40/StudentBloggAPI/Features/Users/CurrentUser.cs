using System.Security.Claims;
using StudentBloggAPI.Features.Users.Interfaces;

namespace StudentBloggAPI.Features.Users;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    // Opps -> denne må registreres i containeren i Program.cs
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;
    public string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    public string? UserName => User?.Identity?.Name;
    public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public IEnumerable<string> Roles => User?
        .FindAll(ClaimTypes.Role)
        .Select(c => c.Value) ?? [];

    public bool IsAdmin => Roles?.Any(r => r == "Admin") ?? false;
}