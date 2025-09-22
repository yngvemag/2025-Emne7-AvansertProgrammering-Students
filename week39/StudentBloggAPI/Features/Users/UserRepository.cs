using StudentBloggAPI.Features.Users.Interfaces;

namespace StudentBloggAPI.Features.Users;

public class UserRepository : IUserRepository
{
    public async Task<User?> AddAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> UpdateAsync(Guid id, User entity)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> DeleteByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<User>> GetPagedAsync(int pageNumber, int pageSize)
    {
        await Task.Delay(10);
        
        List<User> users = new();
        users.Add(
            new User()
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john@mail.com",
                HashedPassword = "søakjdføaslkdfjasøldfjk",
                IsAdminUser = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            }
        );
        
        return users;
    }
}