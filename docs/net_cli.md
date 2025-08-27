# .NET CLI Command Cheat Sheet

This document provides a comprehensive overview of essential .NET CLI commands, with descriptions and examples for each.

## Table of Contents
 
- [Setup and Installation](#setup-and-installation)
- [Project Management](#project-management)
- [Building and Running](#building-and-running)
- [Testing](#testing)
- [Publishing and Deployment](#publishing-and-deployment)
- [NuGet Package Management](#nuget-package-management)
- [Entity Framework Core](#entity-framework-core)
- [Tool Management](#tool-management)
- [Advanced Operations](#advanced-operations)
- [Workflow Examples](#workflow-examples)
- [Best Practices](#best-practices)

## Setup and Installation
_Install and configure the .NET SDK on your machine to start developing .NET applications. This section covers essential setup commands to get you started with .NET development._

### Installation

```powershell
# Check installed .NET versions
dotnet --list-sdks
dotnet --list-runtimes

# Check current .NET version
dotnet --version

# Check detailed .NET information
dotnet --info
```
<div style="page-break-after:always;"></div>

### Environment Setup

```powershell
# Set DOTNET_ROOT environment variable (Windows)
$env:DOTNET_ROOT = "C:\Program Files\dotnet"

# Set DOTNET_ROOT environment variable (Linux/macOS)
# export DOTNET_ROOT=/usr/local/share/dotnet

# Configure telemetry opt-out
$env:DOTNET_CLI_TELEMETRY_OPTOUT = "true"

# Enable detailed error messages
$env:DOTNET_DETAILED_ERRORS = "1"
```

## Project Management
_Create, manage, and organize your .NET projects efficiently using built-in templates and project structure commands._

### Creating Projects

```powershell
# List available templates
dotnet new list

# Create a new console application
dotnet new console -n MyConsoleApp

# Create a new web API project
dotnet new webapi -n MyWebApi

# Create a new ASP.NET Core MVC project
dotnet new mvc -n MyMvcApp

# Create a new Blazor WebAssembly project
dotnet new blazorwasm -n MyBlazorApp

# Create a new ASP.NET Core Web App
dotnet new webapp -n MyWebApp

# Create a new Class Library
dotnet new classlib -n MyLibrary

# Create a new xUnit test project
dotnet new xunit -n MyTests

# Create a new project with a specific framework
dotnet new console -n MyNetApp -f net8.0
```
<div style="page-break-after:always;"></div>

### Solution Management

```powershell
# Create a new solution
dotnet new sln -n MySolution

# Add projects to a solution
dotnet sln MySolution.sln add MyProject/MyProject.csproj

# Add multiple projects to a solution
dotnet sln MySolution.sln add **/*.csproj

# List projects in a solution
dotnet sln MySolution.sln list

# Remove a project from a solution
dotnet sln MySolution.sln remove MyProject/MyProject.csproj
```

### Project References

```powershell
# Add a project reference
dotnet add MyApp/MyApp.csproj reference MyLibrary/MyLibrary.csproj

# Remove a project reference
dotnet remove MyApp/MyApp.csproj reference MyLibrary/MyLibrary.csproj

# List references
dotnet list MyApp/MyApp.csproj reference
```
<div style="page-break-after:always;"></div>

## Building and Running
_Compile, execute, and watch your .NET applications during development. These commands form the core of your daily development workflow._

### Building Projects

```powershell
# Build a project
dotnet build

# Build with specific configuration
dotnet build -c Release

# Build with specific framework
dotnet build -f net8.0

# Build with MSBuild verbosity
dotnet build -v detailed

# Clean build outputs
dotnet clean

# Build and output to a specific directory
dotnet build -o ./build-output
```

### Running Applications

```powershell
# Run a project
dotnet run

# Run in specific configuration
dotnet run -c Release

# Run with arguments
dotnet run -- --arg1 value1 --arg2 value2

# Run a specific project in the solution
dotnet run --project MyProject/MyProject.csproj

# Run using a specific framework
dotnet run -f net8.0
```
<div style="page-break-after:always;"></div>

### Watch Mode

```powershell
# Run with file watcher
dotnet watch run

# Run tests with file watcher
dotnet watch test

# Run with specific configuration
dotnet watch run -c Release

# Specify a project to watch
dotnet watch --project MyProject/MyProject.csproj run
```

## Testing
_Run automated tests for your .NET applications to ensure code quality and catch regressions early._

### Running Tests

```powershell
# Run all tests in a project
dotnet test

# Run tests with specific configuration
dotnet test -c Release

# Run a specific test
dotnet test --filter "FullyQualifiedName=Namespace.TestClass.TestMethod"

# Run tests in a specific class
dotnet test --filter "FullyQualifiedName~Namespace.TestClass"

# Run tests matching criteria
dotnet test --filter "Category=UnitTest"

# Run tests with verbosity
dotnet test -v normal

# Run tests with code coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info

# Run tests and generate report
dotnet test --logger "trx;LogFileName=test_results.trx"
```
<div style="page-break-after:always;"></div>

## Publishing and Deployment
_Prepare your .NET application for deployment to various environments by creating optimized, production-ready builds._

### Publishing Applications

```powershell
# Publish for current platform
dotnet publish -c Release

# Publish self-contained app
dotnet publish -c Release --self-contained -r win-x64

# Publish single-file app
dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true

# Publish trimmed app
dotnet publish -c Release -r win-x64 /p:PublishTrimmed=true

# Publish with ReadyToRun compilation
dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=true

# Publish to specific folder
dotnet publish -c Release -o ./publish-output

# Publish with specific .NET version
dotnet publish -c Release -f net8.0
```

### Common Runtime Identifiers (RIDs)

```powershell
# Windows
dotnet publish -r win-x64
dotnet publish -r win-x86
dotnet publish -r win-arm64

# macOS
dotnet publish -r osx-x64
dotnet publish -r osx-arm64

# Linux
dotnet publish -r linux-x64
dotnet publish -r linux-arm
dotnet publish -r linux-arm64
```
<div style="page-break-after:always;"></div>

## NuGet Package Management
_Work with NuGet packages to add external dependencies, libraries, and tools to your projects._

### Managing Packages

```powershell
# Add a package to a project
dotnet add package Microsoft.EntityFrameworkCore

# Add a package with specific version
dotnet add package Newtonsoft.Json --version 13.0.1

# Add a package from a specific source
dotnet add package MyPackage --source https://myget.org/F/my-feed/api/v3/index.json

# Remove a package
dotnet remove package Newtonsoft.Json

# List installed packages
dotnet list package

# List outdated packages
dotnet list package --outdated

# Restore packages
dotnet restore
```

### Working with NuGet Configuration

```powershell
# Add a NuGet source
dotnet nuget add source https://myget.org/F/my-feed/api/v3/index.json -n MyFeed

# Remove a NuGet source
dotnet nuget remove source MyFeed

# Enable a NuGet source
dotnet nuget enable source MyFeed

# Disable a NuGet source
dotnet nuget disable source MyFeed

# List NuGet sources
dotnet nuget list source
```
<div style="page-break-after:always;"></div>

### Creating and Publishing Packages

```powershell
# Pack a project into a NuGet package
dotnet pack -c Release

# Pack with specific version
dotnet pack -c Release /p:Version=1.2.3

# Pack with symbols
dotnet pack -c Release --include-symbols

# Pack to a specific directory
dotnet pack -c Release -o ./nupkgs

# Publish package to NuGet
dotnet nuget push ./nupkgs/MyPackage.1.2.3.nupkg -k your-api-key -s https://api.nuget.org/v3/index.json
```
<div style="page-break-after:always;"></div>

## Entity Framework Core
_Manage databases, migrations, and data models using Entity Framework Core's CLI tools._

### Installation

```powershell
# Install EF Core CLI tools globally
dotnet tool install --global dotnet-ef

# Update EF Core CLI tools
dotnet tool update --global dotnet-ef

# Add EF Core design package to project
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### Migrations

```powershell
# Create a new migration
dotnet ef migrations add InitialCreate

# Remove the last migration
dotnet ef migrations remove

# List migrations
dotnet ef migrations list

# Generate SQL script from migrations
dotnet ef migrations script

# Generate SQL script for specific migrations
dotnet ef migrations script Migration1 Migration2
```
<div style="page-break-after:always;"></div>

### Database Operations

```powershell
# Apply migrations to the database
dotnet ef database update

# Apply specific migration
dotnet ef database update Migration1

# Drop the database
dotnet ef database drop

# Create a DbContext design-time
dotnet ef dbcontext scaffold "Server=localhost;Database=MyDb;User=sa;Password=P@ssw0rd;" Microsoft.EntityFrameworkCore.SqlServer -o Models
```

## Tool Management
_Install, update, and manage global .NET tools that enhance your development workflow._

### Managing .NET Tools

```powershell
# Install a tool globally
dotnet tool install -g dotnet-outdated-tool

# Install a tool locally to the project
dotnet new tool-manifest # if tool-manifest doesn't exist
dotnet tool install dotnet-format

# Update a global tool
dotnet tool update -g dotnet-outdated-tool

# Uninstall a global tool
dotnet tool uninstall -g dotnet-outdated-tool

# List installed global tools
dotnet tool list -g

# List installed local tools
dotnet tool list
```
<div style="page-break-after:always;"></div>

### Useful .NET Tools

```powershell
# Code formatting tool
dotnet tool install -g dotnet-format
dotnet format

# API documentation generator
dotnet tool install -g docfx
docfx init -q

# Code metrics
dotnet tool install -g dotnet-counters
dotnet counters monitor -p <process_id>

# Code coverage reporter
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"./coverage.cobertura.xml" -targetdir:"./coverage-report" -reporttypes:Html
```

## Advanced Operations
_Perform advanced tasks for specialized development scenarios._

### Cross-Platform Development

```powershell
# Check platform compatibility
dotnet list package --framework net8.0-windows

# Show target frameworks
dotnet msbuild -t:ShowTargetFrameworks

# Create a globalization-ready app
dotnet new globalization
```

### Performance Profiling

```powershell
# Install the profiling tool
dotnet tool install -g dotnet-trace

# Start tracing a process
dotnet trace collect -p <process_id>

# Start tracing with specific providers
dotnet trace collect -p <process_id> --providers Microsoft-DotNETCore-SampleProfiler
```
<div style="page-break-after:always;"></div>

### Security Tools

```powershell
# Audit packages for vulnerabilities
dotnet list package --vulnerable

# Scan project for security issues
# Using Security Code Scan (after installing the package)
dotnet tool install -g security-scan
security-scan YourProject.csproj
```

## Workflow Examples
_Common patterns and procedures for using .NET CLI effectively in different scenarios._

### CI/CD Pipeline Example

```powershell
# Restore dependencies
dotnet restore

# Build the project
dotnet build -c Release

# Run tests
dotnet test -c Release --no-build

# Publish the application
dotnet publish -c Release --no-build -o ./publish

# Run tests with code coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Microservice Development Workflow

```powershell
# Create a solution
dotnet new sln -n MyMicroservices

# Create API projects
dotnet new webapi -n OrderService
dotnet new webapi -n ProductService
dotnet new webapi -n UserService

# Add projects to solution
dotnet sln add **/*.csproj

# Add shared library
dotnet new classlib -n Shared
dotnet add OrderService/OrderService.csproj reference Shared/Shared.csproj
dotnet add ProductService/ProductService.csproj reference Shared/Shared.csproj
dotnet add UserService/UserService.csproj reference Shared/Shared.csproj
```
<div style="page-break-after:always;"></div>

## Best Practices
_Guidelines for effectively using the .NET CLI in development workflows._

### Project Structure

```
MyProject/
├── src/
│   ├── MyProject.API/         # Web API project
│   ├── MyProject.Core/        # Business logic and domain models
│   ├── MyProject.Data/        # Data access layer
│   └── MyProject.Shared/      # Shared libraries
├── tests/
│   ├── MyProject.UnitTests/
│   └── MyProject.IntegrationTests/
├── docs/                      # Documentation
├── tools/                     # Scripts and tools
├── .gitignore
├── README.md
└── MyProject.sln
```

### Command Aliases

Create PowerShell aliases for common commands:

```powershell
# Set aliases in your PowerShell profile
function dnb { dotnet build $args }
function dnr { dotnet run $args }
function dnt { dotnet test $args }
function dnw { dotnet watch $args }
function dnp { dotnet publish $args }
```

### Working with Multiple Projects

```powershell
# Build specific projects
dotnet build src/MyProject.API/MyProject.API.csproj src/MyProject.Core/MyProject.Core.csproj

# Use solution filters for large solutions
dotnet sln create-filter --name api src/MyProject.API/MyProject.API.csproj
dotnet build MySolution.api.slnf
```