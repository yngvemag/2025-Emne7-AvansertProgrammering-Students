using StudentBloggAPI.Features.Common.Interfaces;
using StudentBloggAPI.Features.Users.Dtos;

namespace StudentBloggAPI.Features.Users.Mappers;

public class UserRegistrationMapper : IMapper<UserRegistrationDto, User>
{
    public UserRegistrationDto MapToDto(User model)
    {
        throw new NotImplementedException();
    }

    public User MapToModel(UserRegistrationDto dto)
    {
        return new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            UserName = dto.UserName,
            Email = dto.Email
        };
    }
}