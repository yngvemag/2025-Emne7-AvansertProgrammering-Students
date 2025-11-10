using StudentBloggAPI.Features.Users;

namespace StudentBloggAPI.IntegrationTests.TestData.Users;

public class TestUser
{
    public User? User { get; set; }
    public string Base64EncodedUsernamePassword { get; set; }
}

public class UsersControllerTestsData
{
    public static IEnumerable<object[]> GetTestUsers() => new List<object[]>()
    {
        new object[]
        {
            new TestUser
            {
                Base64EncodedUsernamePassword = "eW1hOmhlbW1lbGlnIQ==", // "yma:hemmelig!"
                User = new User()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Yngve", LastName = "Magnussen", CreatedAt = DateTime.UtcNow, IsAdminUser = false,
                    UpdatedAt = DateTime.UtcNow, UserName = "yma", Email = "yngve@mail.com",
                    HashedPassword = "$2a$11$VNGvAV6Wi284bFLzNozUG.4qnudWxDVwaXBWLkjvPJkOin8THZ34K"
                }
            }
        },
        new object[]
        {
            new TestUser
            {
                Base64EncodedUsernamePassword = "b2xhOm9sYQ==", // "ola:ola!"
                User = new User()
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Ola", LastName = "Normann", CreatedAt = DateTime.UtcNow, IsAdminUser = false,
                        UpdatedAt = DateTime.UtcNow, UserName = "ola", Email = "ola@mail.com",
                        HashedPassword = "$2a$11$l9QLO86QyxuaJrhM2MKam.y192KcZ1.KiSIgxZA18A/GPWqKi9qQW"        
                    }
                
            }
        }
    };
}