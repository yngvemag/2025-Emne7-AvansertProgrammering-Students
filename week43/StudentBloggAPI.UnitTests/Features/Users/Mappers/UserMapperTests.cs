using StudentBloggAPI.Features.Common.Interfaces;
using StudentBloggAPI.Features.Users;
using StudentBloggAPI.Features.Users.Dtos;
using StudentBloggAPI.Features.Users.Mappers;

namespace StudentBloggAPI.UnitTests.Features.Users.Mappers;

public class UserMapperTests
{
    private readonly IMapper<UserDto, User> _userMapper = new UserMapper();

    [Fact]
    public void MapToDto_When_UserModelIsValid_Should_Return_UserDto()
    {
        // Arrange
        // Klargjøre data som vi trenger til test
        User user = new()
        {
            Id = Guid.NewGuid(),
            Email = "ola@mail.com",
            FirstName = "Ola", LastName = "Normann",
            UserName = "ola", IsAdminUser = false,
            UpdatedAt = new DateTime(2025,10,20, 9, 40, 00),
            CreatedAt = new DateTime(2025,10,20, 9, 40, 00),
            HashedPassword = "hksajdhflaksjhfdalskdjfhaslkdjfhaø"
        };
        
        // Act
        UserDto userDto = _userMapper.MapToDto(user);

        // Assert
        Assert.NotNull(userDto);
        Assert.Equal(user.Id, userDto.Id);
        Assert.Equal(user.FirstName, userDto.FirstName);
        Assert.Equal(user.LastName, userDto.LastName);
        Assert.Equal(user.UserName, userDto.UserName);
        Assert.Equal(user.Email, userDto.Email);
        Assert.Equal(user.CreatedAt, userDto.CreatedAt);
    }

    [Fact]
    public void MapToModel_When_UserDtoIsValid_Should_Return_UserModel()
    {
        // Arrange
        // Klargjøre data som vi trenger til test
        UserDto dto = new()
        {
            Id = Guid.NewGuid(),
            Email = "ola@email.com",
            FirstName = "Ola",
            LastName = "Normann",
            UserName = "ola_normann",
            CreatedAt = new DateTime(2025, 10, 20, 9, 45, 00)
        };
        
        // Act
        User user = _userMapper.MapToModel(dto);
        
        // Assert
        Assert.NotNull(user);
        Assert.Equal(dto.Id, user.Id);
        Assert.Equal(dto.Email, user.Email);
        Assert.Equal(dto.FirstName, user.FirstName);
        Assert.Equal(dto.LastName, user.LastName);
        Assert.Equal(dto.UserName, user.UserName);
        Assert.Equal(dto.CreatedAt, user.CreatedAt);
    }
}