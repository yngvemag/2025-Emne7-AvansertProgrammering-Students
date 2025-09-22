using StudentBloggAPI.Features.Common.Interfaces;
using StudentBloggAPI.Features.Users.Interfaces;

namespace StudentBloggAPI.Features.Users;

public class UserService(
    ILogger<UserService> logger,
    IMapper<UserDto, User> userMapper,
    IUserRepository userRepository)
    : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<UserService> _logger = logger;
    private readonly IMapper<UserDto, User> _userMapper = userMapper;

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
        var users = await _userRepository.GetPagedAsync(pageNumber, pageSize);

        // return users
        //     .Select(u => _userMapper.MapToDto(u))
        //     .ToList();

        return users.Select(_userMapper.MapToDto).ToList();

        // List<UserDto> usersDto = new();
        // foreach (var user in users)
        // {
        //     usersDto.Add(_userMapper.MapToDto(user));
        // }
        // return usersDto;

    }
}