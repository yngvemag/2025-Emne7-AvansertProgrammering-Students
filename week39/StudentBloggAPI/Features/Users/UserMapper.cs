using StudentBloggAPI.Features.Common.Interfaces;

namespace StudentBloggAPI.Features.Users;

public class UserMapper : IMapper<UserDto, User>
{
    public UserDto MapToDto(User model)
    {
        return new UserDto
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.UserName,
            Email = model.Email,
            CreatedAt = model.CreatedAt
        };
    }

    public User MapToModel(UserDto dto)
    {
        return new User
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            UserName = dto.UserName,
            Email = dto.Email,
            CreatedAt = dto.CreatedAt
        };
    }
}