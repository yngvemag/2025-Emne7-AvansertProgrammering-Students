using System.Linq.Expressions;
using System.Net;
using Moq;
using Newtonsoft.Json;
using StudentBloggAPI.Features.Users;
using StudentBloggAPI.Features.Users.Dtos;

namespace StudentBloggAPI.IntegrationTests.Features.Users;

public class UsersControllerTests(CustomWebApplicationFactory factory) 
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetPagedUsersAsync_DefaultPageSize_ShouldReturnTwoUsers()
    {
        // Arrange
        Guid[] ids = [Guid.NewGuid(), Guid.NewGuid()];
        List<User> users =
        [
            new User
            {
                Id = ids[0],
                FirstName = "Yngve", LastName = "Magnussen", Email = "yngve@mail.com",
                UserName = "yma", CreatedAt = DateTime.UtcNow, IsAdminUser = false, UpdatedAt = DateTime.UtcNow,
                HashedPassword = "$2a$11$VNGvAV6Wi284bFLzNozUG.4qnudWxDVwaXBWLkjvPJkOin8THZ34K"
            },
            new User
            {
                Id = ids[1],
                FirstName = "Ola", LastName = "Normann", CreatedAt = DateTime.UtcNow, IsAdminUser = false,
                UpdatedAt = DateTime.UtcNow, UserName = "ola", Email = "ola@mail.com",
                HashedPassword = "$2a$11$GpwqWFPCpFJB6kS4SIguiuAlLwBDtHiu9nstIh//b7KKsxX08fD8G"
            },
        ];
        
        // Authentication Middleware (FindAsync)
        factory.UserRepository
            .Setup(u => u.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(users);

        // (GetPagedAsync)
        factory.UserRepository
            .Setup(u => u.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(users);

        // Add authentication header
        string base64EncodedAuthString = "eW1hOmhlbW1lbGlnIQ==";
        _client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64EncodedAuthString}");

        // Act
        var response = await _client.GetAsync("/api/v1/users", TestContext.Current.CancellationToken);

        // deserialize response
        var data = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(data);
        var userDtos = data as UserDto[] ?? data.ToArray();

        Assert.Equal(2, userDtos?.Length);
        Assert.Collection(userDtos!,
            user =>
            {
                Assert.Equal(users[0].FirstName, user.FirstName);
                Assert.Equal(users[0].LastName, user.LastName);
                Assert.Equal(users[0].Email, user.Email);
                Assert.Equal(users[0].UserName, user.UserName);
                Assert.Equal(users[0].Id, user.Id);
            },
            user =>
            {
                Assert.Equal(users[1].FirstName, user.FirstName);
                Assert.Equal(users[1].LastName, user.LastName);
                Assert.Equal(users[1].Email, user.Email);
                Assert.Equal(users[1].UserName, user.UserName);
                Assert.Equal(users[1].Id, user.Id);
            });
    }
}
