
# Overview of xUnit Testing

## What is xUnit?

xUnit is a free, open-source, and community-focused unit testing framework for .NET applications. It is part of the xUnit family of unit testing frameworks that are widely used across different programming languages. It offers a variety of features that simplify writing tests, supporting everything from simple unit tests to complex integration tests.

---

## Setting Up xUnit for .NET Projects

### Required NuGet Packages

To set up xUnit for your .NET project, you need to add the following NuGet packages:

1. **xUnit**: The core framework for writing and running tests.

   ```bash
   dotnet add package xunit
   ```

2. **xUnit Runner for Visual Studio**: To enable test discovery and execution inside Visual Studio.

   ```bash
   dotnet add package xunit.runner.visualstudio
   ```

3. **Moq** (optional): A popular mocking framework for .NET to isolate dependencies in your tests.

   ```bash
   dotnet add package Moq
   ```

4. **Microsoft.AspNetCore.Mvc.Testing**: For setting up integration tests with ASP.NET Core applications.

   ```bash
   dotnet add package Microsoft.AspNetCore.Mvc.Testing
   ```

---
<br><br><br><br><br><br><br><br>

## Key xUnit Annotations and Features

### 1. `[Fact]`: Simple Unit Test

- The `[Fact]` attribute marks a method as a unit test.
- It is used when you have a **single scenario** to test and no parameters are required.

#### Example

```csharp
[Fact]
public void AddNumbers_ShouldReturnCorrectSum()
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

### 2. `[Theory]`: Data-Driven Test

- The `[Theory]` attribute is used to create **parameterized tests** that can run multiple times with different inputs.
- It is useful when you want to test the same logic with multiple sets of data.

#### Example with `[InlineData]`

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

<br><br><br>

### 3. `[InlineData]`: Supplying Data for `[Theory]`

- The `[InlineData]` attribute is used to provide data directly to a `[Theory]` test method.
- You can specify multiple `[InlineData]` attributes for the same test method to test various scenarios.

#### Example

```csharp
[Theory]
[InlineData(2, 2, 4)]
[InlineData(3, 5, 8)]
public void MultiplyNumbers_ShouldReturnCorrectProduct(int x, int y, int expected)
{
    Assert.Equal(expected, x * y);
}
```

### 4. `[MemberData]`: External Data Source for Tests

- `[MemberData]` allows you to pull in test data from an external method or property.
- This is useful when you need complex or dynamic test data.

#### Example

```csharp
public class MathTests
{
    [Theory]
    [MemberData(nameof(GetTestData))]
    public void AddNumbers_ShouldReturnCorrectSum(int a, int b, int expectedSum)
    {
        Assert.Equal(expectedSum, a + b);
    }

    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] { 1, 2, 3 };
        yield return new object[] { 3, 4, 7 };
    }
}
```

<br><br><br><br><br><br><br><br>

### 5. `IClassFixture<T>`: Sharing Setup Across Tests

- `IClassFixture<T>` is used to share setup and teardown logic across all the tests in a test class.
- This is useful for **integration tests** or scenarios where you need a common setup (like initializing a database or an in-memory web server).

#### Example

```csharp
public class MyIntegrationTests : IClassFixture<MyWebAppFactory>
{
    private readonly HttpClient _client;

    public MyIntegrationTests(MyWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHomePage_ShouldReturnSuccess()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

### 6. `[Collection]`: Sharing Context Between Test Classes

- The `[Collection]` attribute is used to **share state** or **test fixtures** between multiple test classes.
- It is useful when you want to share context (e.g., a database connection) between several test classes.

#### Example

```csharp
[Collection("Database collection")]
public class DatabaseTests
{
    private readonly DatabaseFixture _fixture;

    public DatabaseTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestDatabaseConnection()
    {
        Assert.NotNull(_fixture.DbConnection);
    }
}
```

---

## Writing Integration Tests with xUnit

### Integration Tests with `WebApplicationFactory`

xUnit supports integration testing of ASP.NET Core applications using the `WebApplicationFactory<TEntryPoint>` class. This allows you to host your application in-memory and send real HTTP requests to test your endpoints.

#### Example Setup

```csharp
public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Mock services here if needed
        });
    }
}

public class MyIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MyIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnSuccess()
    {
        var response = await _client.GetAsync("/api/products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

---
<br><br><br><br><br><br><br><br><br><br>

## Common Assertions in xUnit

- `Assert.Equal(expected, actual)`: Verifies that the actual value equals the expected value.
- `Assert.NotNull(value)`: Ensures that the value is not null.
- `Assert.Throws<TException>(Action)`: Verifies that a specific exception is thrown.
- `Assert.True(condition)`: Ensures the condition is true.
- `Assert.Contains(expectedSubstring, actualString)`: Verifies that a string contains a specific substring.

---

## Summary of xUnit Features

- **xUnit** is a flexible and powerful framework for writing unit tests and integration tests for .NET applications.
- **[Fact]**: Simple test without parameters.
- **[Theory]**: Parameterized test that can run multiple times with different data.
- **[InlineData]**: Provides inline data for a theory.
- **[MemberData]**: Fetches test data from external sources.
- **IClassFixture<T>**: Shares setup and teardown logic between test methods.
- **WebApplicationFactory**: Used for integration testing in ASP.NET Core applications.

By understanding these key annotations and patterns, you can write effective and maintainable tests using xUnit.
