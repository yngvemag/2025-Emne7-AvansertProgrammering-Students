# CustomWebApplicationFactory - Dokumentasjon

## Hva er WebApplicationFactory<Program>?

`WebApplicationFactory<Program>` er en innebygd ASP.NET Core klasse som brukes til integrasjonstesting. Den:

- **Starter en in-memory test server** som kjører din hele applikasjon
- **Laster opp hele DI container** med alle tjenestene dine
- **Simulerer HTTP requests** uten å starte en ekte web server
- **Gir en HttpClient** som kan brukes til å teste API endepunktene

```csharp
// Standard bruk:
public class MyTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MyTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(); // Får HttpClient til testing
    }
}
```

## Hvorfor en CustomWebApplicationFactory?

### Problem med standard WebApplicationFactory

- Bruker **ekte database forbindelser**
- Kaller **ekte eksterne tjenester**  
- **Ikke isolerte tester** - testene påvirker hverandre
- **Trege tester** - må vente på database operasjoner

### Løsningen - CustomWebApplicationFactory

```csharp
public class CustomWebApplicationFactory: WebApplicationFactory<Program>
{
    private Mock<IUserRepository> UserRepositoryMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            // Erstatter ekte IUserRepository med en mock
            services.AddSingleton(UserRepositoryMock.Object);
        });
    }
}
```

### Fordelene

✅ **Kontrollerte tester** - Du bestemmer hva repository returnerer  
✅ **Raske tester** - Ingen database kall  
✅ **Isolerte tester** - Testene påvirker ikke hverandre  
✅ **Forutsigbare resultater** - Testene er ikke avhengige av database tilstand  

## Når brukes CustomWebApplicationFactory?

### 1. **Controller Testing**

Test hele HTTP request/response syklusen:

```csharp
[Fact]
public async Task GetUsers_ReturnsOkResult()
{
    // Arrange - Setup mock data
    var users = new List<User> 
    { 
        new User { Id = Guid.NewGuid(), UserName = "testuser" } 
    };
    
    _factory.UserRepositoryMock
        .Setup(x => x.GetPagedAsync(1, 10))
        .ReturnsAsync(users);

    // Act - Send HTTP request
    var response = await _client.GetAsync("/api/users?page=1&size=10");

    // Assert - Check response
    Assert.True(response.IsSuccessStatusCode);
}
```

### 2. **Authentication Testing**

Test sikkerhet og autorisering:

```csharp
[Fact]
public async Task DeleteUser_WithoutAuth_Returns401()
{
    // Act
    var response = await _client.DeleteAsync("/api/users/123");

    // Assert
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
}
```

### 3. **Validation Testing**

Test input validering:

```csharp
[Fact]
public async Task CreateUser_InvalidData_ReturnsBadRequest()
{
    // Arrange
    var invalidUser = new { UserName = "" }; // Tom brukernavn

    // Act
    var response = await _client.PostAsJsonAsync("/api/users", invalidUser);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
}
```

## Hvordan brukes den?

### 1. **Setup i test klassen:**

```csharp
public class UsersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UsersControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
}
```

### 2. **Bruk i test metoder:**

```csharp
[Fact]
public async Task RegisterUser_ValidData_ReturnsCreated()
{
    // Arrange - Setup hva mock skal returnere
    var newUser = new User { Id = Guid.NewGuid(), UserName = "newuser" };
    
    _factory.UserRepositoryMock
        .Setup(x => x.AddAsync(It.IsAny<User>()))
        .ReturnsAsync(newUser);

    var registerRequest = new 
    { 
        UserName = "newuser",
        Email = "test@example.com",
        Password = "SecurePass123!"
    };

    // Act - Send POST request
    var response = await _client.PostAsJsonAsync("/api/users/register", registerRequest);

    // Assert - Verify response
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    
    // Verify mock was called
    _factory.UserRepositoryMock.Verify(
        x => x.AddAsync(It.IsAny<User>()), 
        Times.Once);
}
```

## Sammenligning: Unit vs Integration Testing

| Aspekt | Unit Testing | Integration Testing (CustomWebApplicationFactory) |
|--------|--------------|---------------------------------------------------|
| **Scope** | En enkelt metode/klasse | Hele HTTP pipeline |
| **Dependencies** | Alle mockes | Kun kritiske tjenester mockes |
| **Testing** | Forretningslogikk | HTTP routing, serialisering, validering |
| **Speed** | Meget rask | Rask (raskere enn ekte database) |
| **Setup** | Enkel | Noe mer kompleks |

## Best Practices

### ✅ **Gjør dette:**

- Mock kun eksterne avhengigheter (database, API kall)
- Test vanlige HTTP scenarios (200, 400, 401, 404)
- Bruk `IClassFixture` for å dele factory mellom tester
- Verify at mock metoder blir kalt som forventet

### ❌ **Ikke gjør dette:**

- Mock hele tjeneste laget - test ekte implementasjoner
- Test kun "happy path" - test også error scenarios
- Glem å cleanup mock setup mellom tester

## Eksempel på Komplett Test

```csharp
[Fact]
public async Task GetUserById_ExistingUser_ReturnsUser()
{
    // Arrange
    var userId = Guid.NewGuid();
    var expectedUser = new User 
    { 
        Id = userId, 
        UserName = "testuser",
        Email = "test@example.com"
    };

    _factory.UserRepositoryMock
        .Setup(x => x.GetByIdAsync(userId))
        .ReturnsAsync(expectedUser);

    // Act
    var response = await _client.GetAsync($"/api/users/{userId}");
    var content = await response.Content.ReadAsStringAsync();
    var user = JsonSerializer.Deserialize<UserDto>(content);

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.NotNull(user);
    Assert.Equal(expectedUser.UserName, user.UserName);
    Assert.Equal(expectedUser.Email, user.Email);
}
```

## Konklusjon

`CustomWebApplicationFactory` gir deg det beste fra begge verdener:

- **Realistisk testing** av hele HTTP pipeline
- **Kontrollert miljø** med mockede dependencies
- **Rask utførelse** uten eksterne avhengigheter

Dette gjør integrasjonstesting både pålitelig og vedlikeholdbar for ASP.NET Core applikasjoner.
