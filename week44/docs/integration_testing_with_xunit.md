
# Introduction to Integration Testing with xUnit and IClassFixture

## What is Integration Testing?

Integration testing is a type of software testing where individual units (often tested via unit tests) are combined and tested as a group to ensure that they work well together. It focuses on validating the interactions between different components of the application, such as the communication between controllers, services, repositories, and external systems (like databases or APIs).

In the context of **ASP.NET Core applications**, integration tests can be used to test API endpoints by sending HTTP requests to the in-memory web server. This simulates real-world usage of your API, ensuring that everything from routing to model binding and business logic works as expected.

## Setting Up an Integration Test Project

### 1. Project Setup

To start an integration test project, you typically need the following packages installed in your test project:

- **xUnit**: For writing and executing tests.
- **Moq**: For mocking dependencies.
- **Microsoft.AspNetCore.Mvc.Testing**: Provides utilities for setting up an in-memory web server for testing your ASP.NET Core application.

To install these packages, run the following commands:

```bash
dotnet add package xunit
dotnet add package Moq
dotnet add package Microsoft.AspNetCore.Mvc.Testing
```

<div style="page-break-after:always;"></div>

### 2. Creating the WebApplicationFactory

`WebApplicationFactory` is a special class provided by **Microsoft.AspNetCore.Mvc.Testing** that helps you host your ASP.NET Core application in memory. This allows you to make real HTTP requests to your application without having to run a live server.

You can create a custom class that inherits from `WebApplicationFactory` to configure your application for testing.

```csharp
public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // You can override services here (e.g., mock the database context)
        });
    }
}
```

### 3. Setting Up with `IClassFixture`

In your test class, you'll use `CustomWebApplicationFactory` to create an instance of your application and use `HttpClient` to send requests to it. By implementing `IClassFixture`, you ensure that the resources used during testing (like the HTTP client and web factory) are properly managed and shared across test methods.

```csharp
public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
}
```

### What is `IClassFixture<T>`?

The `IClassFixture<T>` interface in xUnit is used to share setup and teardown logic across tests in a test class. In your case, `IClassFixture<CustomWebApplicationFactory>` is used to share the `CustomWebApplicationFactory` (which is a custom implementation of `WebApplicationFactory`) among all the test methods in your test class.

### How `IClassFixture<T>` Works

#### 1. **Test Fixture Initialization**

   When the test class is executed, xUnit will:

- **Create an instance** of `CustomWebApplicationFactory` (your fixture) **once** at the beginning of the test class execution.
- Pass the same instance of `CustomWebApplicationFactory` to the constructor of the test class.

#### 2. **Sharing Resources**

   The `CustomWebApplicationFactory` will be **shared** across all the test methods within the test class. This ensures that:

- The in-memory web server is started only **once**.
- The **`HttpClient`** and other related resources are reused across the different test methods.

#### 3. **Disposal**

   After all tests in the class are done, xUnit will automatically **dispose** of the fixture, ensuring that any cleanup (e.g., stopping the web server, releasing resources) is handled properly.

### Example Setup

```csharp
public class UsersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UsersControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    // Test methods can now reuse _client and _factory without needing manual disposal
}
```

<div style="page-break-after:always;"></div>

### 4. Writing an Integration Test

Now that you have the setup in place, you can write integration tests that simulate real HTTP requests. Below is an example of a test that checks if the `GetUsers` endpoint returns an `OK` response.

```csharp
[Fact]
public async Task GetUsers_ShouldReturnOkResponse()
{
    // Act
    var response = await _client.GetAsync("/api/v1/Users");

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    var content = await response.Content.ReadAsStringAsync();
    Assert.NotNull(content);
}
```

### 5. Authentication and Headers

If your API requires authentication, you can easily add authentication headers to the `HttpClient`:

```csharp
string base64EncodedAuthenticationString = "b2xhOk8xYW5vcm1hbm4j";
_client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64EncodedAuthenticationString}");
```

This ensures that all requests made by the client include the necessary authentication.

<div style="page-break-after:always;"></div>


# Fact, Theory, and MemberData in xUnit

Once you've set up the project and have the basic integration test setup ready, you can now start leveraging xUnit's attributes for writing both simple and data-driven tests. Here's an explanation of each:

## 1. `[Fact]`: Simple Test

- `[Fact]` is used for writing simple, non-parameterized tests.
- It is used when you do not need to pass any input parameters to your test.

### Example

```csharp
[Fact]
public void AddNumbers_ShouldReturnSum()
{
    // Arrange
    int x = 2;
    int y = 3;

    // Act
    var result = x + y;

    // Assert
    Assert.Equal(5, result);
}
```

## 2. `[Theory]`: Data-Driven Test

- `[Theory]` allows for parameterized tests that can be run multiple times with different input values.
- This is useful when you want to test the same logic under different conditions.

### Example with `[InlineData]`

```csharp
[Theory]
[InlineData(1, 2, 3)]
[InlineData(2, 3, 5)]
[InlineData(5, 5, 10)]
public void AddNumbers_ShouldReturnCorrectSum(int a, int b, int expectedSum)
{
    // Act
    var result = a + b;

    // Assert
    Assert.Equal(expectedSum, result);
}
```

## 3. `[MemberData]`: External Data for Tests

- `[MemberData]` is used with `[Theory]` to reference an external method or property that provides test data.
- It is useful when you need more complex test data or need to dynamically generate the data.

### Example

```csharp
public class MathTests
{
    [Theory]
    [MemberData(nameof(GetTestData))]
    public void AddNumbers_ShouldReturnCorrectSum(int a, int b, int expectedSum)
    {
        // Act
        var result = a + b;

        // Assert
        Assert.Equal(expectedSum, result);
    }

    // Method to provide test data
    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] { 1, 2, 3 };
        yield return new object[] { 2, 3, 5 };
        yield return new object[] { -1, -2, -3 };
    }
}
```

<div style="page-break-after:always;"></div>

## 4. Using `[MemberData]` in Integration Tests

You can also combine `[Theory]` and `[MemberData]` in integration tests to test API endpoints with different sets of input data. Here’s an example that tests the `GetUsers` endpoint with different test users.

### Example

```csharp
[Theory]
[MemberData(nameof(TestUserData.GetTestUserData), MemberType = typeof(TestUserData))]
public async Task GetUsers_WithMemberData_ShouldReturnUsers(TestUser testUser)
{
    // Arrange
    _client.DefaultRequestHeaders.Add("Authorization", $"Basic {testUser.Base64EncodedUsernamePasseord}");

    // Act
    var response = await _client.GetAsync("/api/v1/Users");

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    var data = JsonConvert.DeserializeObject<IEnumerable<UserDTO>>(await response.Content.ReadAsStringAsync());
    Assert.Equal(testUser.ExpectedUserCount, data?.Count());
}
```

- **[Theory]**: The test is run multiple times with different user data provided by `TestUserData`.
- **MemberData**: Supplies test user data (like credentials and expected results) dynamically from the `TestUserData` class.

---

## Conclusion

### Integration Testing with xUnit

- **WebApplicationFactory**: Use this to host your ASP.NET Core app in memory for testing.
- **HttpClient**: Send real HTTP requests to your application.
- **IClassFixture**: Use this to manage and share resources like `HttpClient` and `WebApplicationFactory` across test methods.
