using System.Security.Claims;

namespace StudentBloggAPI.Features.Users.Interfaces;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    
    string? UserId { get; }
    
    string? UserName { get; }
    
    ClaimsPrincipal? User { get; }
    
    public IEnumerable<string> Roles { get; }
    
    public bool IsAdmin { get; }
}