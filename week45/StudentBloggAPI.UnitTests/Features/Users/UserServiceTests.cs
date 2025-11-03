using Microsoft.Extensions.Logging;
using Moq;
using StudentBloggAPI.Features.Common.Interfaces;
using StudentBloggAPI.Features.Users;
using StudentBloggAPI.Features.Users.Dtos;
using StudentBloggAPI.Features.Users.Interfaces;

namespace StudentBloggAPI.UnitTests.Features.Users;

/*
 *  ILogger<UserService> logger,
    IMapper<UserDto, User> userMapper, 
    IMapper<UserRegistrationDto, User> userRegistrationMapper,
    IUserRepository userRepository,
    ICurrentUser currentUser) : IUserService
 * 
 */

public class UserServiceTests
{
    private IUserService _userService;
    
    private Mock<IMapper<UserRegistrationDto, User>> _userRegistrationMapper = new Mock<IMapper<UserRegistrationDto, User>>();
    private Mock<IMapper<UserDto, User>> _userDtoMapper = new Mock<IMapper<UserDto, User>>();
    private Mock<IUserRepository> _userRepository = new Mock<IUserRepository>();
    private Mock<ICurrentUser> _currentUser = new Mock<ICurrentUser>();
    private Mock<ILogger<UserService>> _logger = new Mock<ILogger<UserService>>();

    public UserServiceTests()
    {
        SetupMocks();
    }
    private void SetupMocks()
    {
        _userRegistrationMapper = new Mock<IMapper<UserRegistrationDto, User>>();
        _userDtoMapper = new Mock<IMapper<UserDto, User>>();
        _userRepository = new Mock<IUserRepository>();
        _currentUser = new Mock<ICurrentUser>();
        _logger = new Mock<ILogger<UserService>>();
    }
    
    private IUserService CreateUserService() =>  new UserService(
        _logger.Object,
        _userDtoMapper.Object,
        _userRegistrationMapper.Object,
        _userRepository.Object,
        _currentUser.Object
    );
        

    [Fact]
    public async Task GetByIdAsync__WhenUserExists_ShouldReturnUserDto()
    {
        // Arrange
        var userService = CreateUserService();
        User user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User",
            Email = "test@mail.com", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow,
            HashedPassword = "lksjdøfalkajsdøflakjs", IsAdminUser = false
        };
        
        UserDto userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        // setup => _userRepository.GetByIdAsync, _userMapper.MapToDto
        _userRepository.Setup( x => x.GetByIdAsync(user.Id))
            .ReturnsAsync(user);
        _userDtoMapper.Setup( x => x.MapToDto(user))
            .Returns(userDto);
        

        // Act
        var result = await userService.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userDto.Id, result!.Id);
        Assert.Equal(userDto.UserName, result.UserName);
        Assert.Equal(userDto.FirstName, result.FirstName);
        Assert.Equal(userDto.LastName, result.LastName);
        Assert.Equal(userDto.Email, result.Email);
    }
    
    [Fact]
    public async Task GetPagedAsync_WhenTwoUsersExist_ShouldReturnTwoUserDtos()
    {
        // Arrange
        var userService = CreateUserService();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        
        
        List<User> users =
        [
            new User(){Id = userId1},
            new User(){Id = userId2},
        ];

        List<UserDto> userDtos =
        [
            new UserDto(){Id = userId1},
            new UserDto(){Id = userId2}
        ];

        // setup => _userRepository.GetPagedAsync, _userMapper.MapToDto
        _userRepository.Setup( x => x.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(users);
        _userDtoMapper.Setup( x => x.MapToDto(users[0]))
            .Returns(userDtos[0]);
        _userDtoMapper.Setup( x => x.MapToDto(users[1]))
            .Returns(userDtos[1]);

        // Act
        var result = await userService.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>());

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equivalent(userDtos[0], resultList[0]);
        Assert.Equivalent(userDtos[1], resultList[1]);
    }

    [Theory]
    [InlineData("ole", "Ole", "Normann", "ole@mail.com", false)]
    [InlineData("kari", "Kari", "Normann", "kari@mail.com", false)]
    public async Task GetByIdAsync_WithDifferentUsers_ShouldReturnCorrectUserDdt(
        string userName, string firstName, string lastName, string email, bool isAdminUser)
    {
        // arrange
        var userService = CreateUserService();
        var userId = Guid.NewGuid();
        var user = new User()
        {
            Id = userId, UserName = userName,  FirstName = firstName, 
            LastName = lastName, Email = email, IsAdminUser = isAdminUser,
            CreatedAt = DateTime.UtcNow,  UpdatedAt = DateTime.UtcNow, HashedPassword = "høahølkdhsføklash"
        };

        var userDto = new UserDto()
        {
            Id = user.Id,
            UserName = userName,  FirstName = firstName,  LastName = lastName,  Email = email,
            CreatedAt = user.CreatedAt
        };
        
        _userRepository.Setup( x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _userDtoMapper.Setup( x => x.MapToDto(user)).Returns(userDto);

        // act
        var result = await userService.GetByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equivalent(userDto, result);
    }
}