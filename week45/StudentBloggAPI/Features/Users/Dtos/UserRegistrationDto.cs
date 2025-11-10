using System.ComponentModel.DataAnnotations;

namespace StudentBloggAPI.Features.Users.Dtos;

public class UserRegistrationDto
{
    //[Required]
    //[MinLength(3), MaxLength(16)]
    public string UserName { get; set; } = string.Empty;
    
    //[Required]
    //[MinLength(2), MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    //[Required]
    //[MinLength(2), MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    //[EmailAddress(ErrorMessage = "")]
    public string Email { get; set; } = string.Empty;
    
    //[Required]
    public string Password { get; set; } = string.Empty;
}