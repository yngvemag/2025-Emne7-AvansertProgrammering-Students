using StudentBloggAPI.Features.Common.Interfaces;
using StudentBloggAPI.Features.Users.Dtos;

namespace StudentBloggAPI.Features.Users.Interfaces;

public interface IUserService : IBaseService<UserDto>
{
    Task<UserDto?> RegisterAsync(UserRegistrationDto userRegistrationDto);
    
    Task<User?> AuthenticateUserAsync(string userName, string password);
}