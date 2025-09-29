using StudentBloggAPI.Features.Common.Interfaces;
using StudentBloggAPI.Features.Users.Dtos;
using StudentBloggAPI.Features.Users.Interfaces;

namespace StudentBloggAPI.Features.Users;

public class UserService(
    ILogger<UserService> logger,
    IMapper<UserDto, User> userMapper, 
    IMapper<UserRegistrationDto, User> userRegistrationMapper,
    IUserRepository userRepository,
    ICurrentUser currentUser) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<UserService> _logger = logger;
    private readonly IMapper<UserDto, User> _userMapper = userMapper;
    private readonly IMapper<UserRegistrationDto, User> _userRegistrationMapper = userRegistrationMapper;
    private readonly ICurrentUser _currentUser = currentUser;

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
        // Viktig å sjekke om innlogget bruker har rettigheter til å slette brukeren
        // MEN hvis vi har en admin-bruker, så kan den slette hvem som helst !!!
        if (!string.Equals(id.ToString(), _currentUser.UserId) && !_currentUser.IsAdmin)
        {   
            throw new UnauthorizedAccessException("Can't delete user");
        }
        
        var user = await _userRepository.DeleteByIdAsync(id);
        return user is null
            ? null
            : _userMapper.MapToDto(user);

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

    public async Task<UserDto?> RegisterAsync(UserRegistrationDto userRegistrationDto)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password);
        
        User user = _userRegistrationMapper.MapToModel(userRegistrationDto);
        user.Id = Guid.NewGuid();
        user.HashedPassword = hashedPassword;
        user.IsAdminUser = false;
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        
        // Save user to database
        User? addedUser = await _userRepository.AddAsync(user);
        
        return addedUser is null
            ? null
            : _userMapper.MapToDto(addedUser);
    }

    public async Task<User?> AuthenticateUserAsync(string userName, string password)
    {
        // en liten svakhet er at vi henter alle brukere med samme brukernavn
        var users = (await
                _userRepository.FindAsync(u => u.UserName == userName))
            .ToList();
        
        if (!users.Any())
            return null;

        var user = users.FirstOrDefault();
        var isValid = BCrypt.Net.BCrypt.Verify(password, user.HashedPassword);
        
        return isValid ? user : null;
    }
}