using StudentBloggAPI.Features.Users.Interfaces;

namespace StudentBloggAPI.Features.Users;

public class UserService : IUserService
{
    public async Task<UserDto?> AddAsync(UserDto entity)
    {
        throw new NotImplementedException();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<UserDto?> DeleteByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UserDto entity)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UserDto>> GetPagedAsync(int pageNumber, int pageSize)
    {
        await Task.Delay(10);
        
        List<UserDto> users = new();
        users.Add(
                new UserDto()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Doe",
                    UserName = "johndoe",
                    Email = "john@mail.com",
                    CreatedAt = DateTime.UtcNow
                }
            );
        
        return users;
    }
}