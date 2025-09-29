using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using StudentBloggAPI.Features.Users;
using StudentBloggAPI.Features.Users.Interfaces;

namespace StudentBloggAPI.Middleware;

public class StudentBloggBasicAuthentication(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IUserService userService) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly IUserService _userService = userService;
    
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            // sjekke om Authorization header finnes
            // vi ser etter: "Authorization: Basic eW1hOmhlbW1lbGlnIQ==" (Basic yma:hemmelig!)
            var authHeader = Request.Headers["Authorization"].ToString();  
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
            {
                return AuthenticateResult.Fail("Missing or invalid Authorization header");
            }
            
            // hente ut base64 delen av headeren (Basic b2xhOm9sYSE=) => "b2xhOm9sYSE="  > bytes
            var credentialBytes = Convert.FromBase64String(authHeader.Replace("Basic ", "", StringComparison.OrdinalIgnoreCase));
            var credentials = System.Text.Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            
            var username = credentials[0];
            var password = credentials[1];

            User? user = await _userService.AuthenticateUserAsync(username, password);
            
            if (user is null) return AuthenticateResult.Fail("Invalid username or password");
            
            var role = user.IsAdminUser ? "Admin" : "User";
            // opprette claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, role )
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception e)
        {
            return AuthenticateResult.Fail("Missing or invalid Authorization header");
        }
        
        
    }
}