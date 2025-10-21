using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StudentBloggAPI.Features.Users;
using StudentBloggAPI.Features.Users.Dtos;
using StudentBloggAPI.Features.Users.Interfaces;

namespace StudentBloggAPI.UnitTests.Features.Users;

public class UsersControllerTests
{
    private readonly UsersController _userController;
    
    private readonly Mock<IUserService> _userServiceMock = new  Mock<IUserService>();
    private readonly Mock<ILogger<UsersController>> _loggerMock = new Mock<ILogger<UsersController>>();

    public UsersControllerTests()
    {
        _userController = new UsersController(
            _loggerMock.Object, 
            _userServiceMock.Object);
    }

    [Fact]
    public async Task GetPagedUsersAsync_WhenDefaultPageSizeAndOneUserExists_ShouldReturnOneUser()
    {
        // Arrange
        List<UserDto> userDtos =
        [
            new UserDto
            {
                Id = Guid.NewGuid(), UserName = "ola", FirstName = "Ola", LastName = "Normann",
                Email = "ola@mail.com", CreatedAt = DateTime.UtcNow
            }
        ];
        
        _userServiceMock.Setup(
            x => x.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(userDtos);
        
        // Act
        var result = await _userController.GetPagedUsersAsync();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<UserDto>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedUsers = Assert.IsType<List<UserDto>>(okResult.Value);

        var dto = userDtos.FirstOrDefault();
        Assert.NotNull(dto);
        Assert.Equal("ola", dto.UserName);
        
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDto = new UserDto
        {
            Id = userId,
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User",
            Email = "test@mail.com", CreatedAt = DateTime.UtcNow
        };
        // setup => _userService.GetByIdAsync(id);
        _userServiceMock.Setup( x => x.GetByIdAsync(userId))
            .ReturnsAsync(userDto);
        
        // Act
        var result = await _userController.GetUserByIdAsync(userId);
        
        // Assert
        var actionResult = Assert.IsType<ActionResult<UserDto>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);
        
        Assert.NotNull(returnedUser);
        Assert.Equal("testuser", returnedUser.UserName);
        Assert.Equal(userId, returnedUser.Id);
    }

    [Fact]
    public async Task RegisterUserAsync_WhenUserRegistrationIsValid_ShouldReturnRegisteredUser()
    {
        // Arrange
        var registrationDto = new UserRegistrationDto
        {
            UserName = "ola", FirstName = "Ola", LastName = "Normann", Email = "ola@mail.com",
            Password = "hemmelig!"
        };

        var createdUser = new UserDto
        {
            Id = Guid.NewGuid(), FirstName = "Ola", UserName = "ola", Email = "ola@mail.com", LastName = "Normann",
            CreatedAt = DateTime.UtcNow
        };
        // setup => _userService.RegisterAsync(userRegistrationDto);
        _userServiceMock.Setup(x => x.RegisterAsync(registrationDto))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _userController.RegisterUserAsync(registrationDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<UserDto>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);
        
        Assert.NotNull(returnedUser);
        Assert.Equivalent(createdUser, returnedUser);
    }


}