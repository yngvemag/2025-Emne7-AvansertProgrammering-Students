# Strukturert Logging og Serilog i C# REST API

## Hva er Strukturert Logging?

**Strukturert logging** refererer til praksisen med å logge informasjon i et strukturert format, vanligvis som nøkkel-verdi-par, i stedet for bare fri tekst. Dette gjør det enklere å søke, filtrere, og analysere loggdata, spesielt i komplekse eller store applikasjoner.

Med strukturert logging kan du logge ikke bare en melding, men også konteksten rundt meldingen, som objekter, variabler, og andre relevante data, på en konsistent måte. Dette gir følgende fordeler:

- **Søkbarhet:** Du kan enkelt søke etter spesifikke hendelser basert på egenskaper som brukernavn, ordre-ID, osv.
- **Analytikk:** Du kan utføre mer avansert analyse av loggdata, for eksempel ved å lage dashbord og rapporter.
- **Feilsøking:** Det blir lettere å feilsøke problemer fordi du har mer kontekst tilgjengelig i loggene.

Eksempel på strukturert logging:

```json
{
    "Timestamp": "2024-08-27T14:35:00Z",
    "Level": "Information",
    "MessageTemplate": "User {Username} logged in",
    "Properties": {
        "Username": "johndoe",
        "IpAddress": "192.168.0.1"
    }
}
```

I dette eksempelet er Username og IpAddress logget som egne felter, noe som gjør dem enkle å filtrere på og analysere.

# Hvordan Legge til Serilog i et REST API

Serilog er et populært logging-bibliotek i C# som støtter strukturert logging. Her er hvordan du kan legge til Serilog i ditt ASP.NET Core REST API:

## 1. Installer Serilog-pakker

Først må du installere nødvendige Serilog-pakker via NuGet. De viktigste er:

* Serilog.AspNetCore
* Serilog.Sinks.Console
* Serilog.Sinks.File (valgfritt, hvis du vil logge til en fil)
* Serilog.Sinks.Seq (valgfritt, hvis du bruker Seq som logganalyseverktøy)
Du kan installere disse pakkene med følgende kommandoer:

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File  # Valgfritt
dotnet add package Serilog.Sinks.Seq   # Valgfritt
```

## 2. Konfigurer Serilog i Program.cs

Etter at pakkene er installert, må du konfigurere Serilog i Program.cs-filen:
```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

```

## 3. Bruke Serilog i Koden

Når Serilog er konfigurert, kan du bruke loggeren i dine controllere, tjenester, osv.

For eksempel i en controller:
```csharp
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{username}")]
    public IActionResult GetUser(string username)
    {
        _logger.LogInformation("Getting user information for {Username}", username);

        // Simulere henting av brukerdata
        var user = new { Username = username, FullName = "John Doe" };

        if (user == null)
        {
            _logger.LogWarning("User {Username} not found", username);
            return NotFound();
        }

        return Ok(user);
    }
}

```
Her ser du hvordan du kan bruke strukturert logging ved å passere verdier som username inn i loggeren som parametere. Serilog vil automatisk konvertere disse til strukturerte loggdata.

## 4. Se Loggene

* Konsoll: Du vil se loggene i konsollen når du kjører applikasjonen.
* Fil: Hvis du har konfigurert fil-logging, vil loggene bli lagret i den spesifiserte filen, f.eks. logs/myapp.txt.
* Seq: Hvis du bruker Seq, kan du åpne http://localhost:5341 i en nettleser for å se og analysere loggene.

<hr/>

# Konfigurere via appsettings.json

## 2. Oppdater appsettings.json

Legg til en Serilog-seksjon i appsettings.json for å konfigurere loggingsnivå, loggsinks, og andre innstillinger.

Eksempel på en appsettings.json-fil:
```json
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.MySQL", "Serilog.Sinks.Seq" ],
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Warning",
        "System": "Error"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/logs-.log",
          "rollingInterval": "Day",
          "rollOnFileSizeLimit": "True",
          "formatter": "Serilog.Formatting.Json.JsonFormatter"
        }
      },
      {
        "Name": "MySQL",
        "Args": {
          "connectionString": "server=localhost;uid=ga-app;pwd=ga-5ecret-%;database=ga_emne7_avansert;",
          "tableName": "Logs",
          "autoCreateSqlTable": true
        }
      },       {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://localhost:5341",
           "apiKey": "my-secret-api-key-123"
          }
      }

    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ]

```

### Forklaring av appsettings.json

* Using: Spesifiserer hvilke sinks (utgangskanaler) som skal brukes. I dette eksempelet brukes konsoll og fil som sinks.
* MinimumLevel: Angir minimum loggnivå for hele applikasjonen og spesifikke logger (f.eks. Microsoft).
* WriteTo: Definerer hvor loggene skal skrives. I dette tilfellet logges det til konsollen og til en fil med daglige rulleringer.
* Enrich: Lar deg legge til ekstra informasjon i loggene, som maskinnavn og tråd-ID.
* Properties: Statisk informasjon som legges til i hver loggpost, f.eks. applikasjonsnavn.

## 3. Konfigurer Serilog i Program.cs

Endre Program.cs for å lese Serilog-konfigurasjonen fra appsettings.json:
```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Leser inn Serilog-konfigurasjon fra appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Erstatt standard logging med Serilog
builder.Host.UseSerilog();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

```

## Oppsummering

Strukturert logging gir deg mer detaljerte og søkbare loggdata, noe som gjør det enklere å overvåke og feilsøke applikasjoner. Ved å bruke Serilog i ditt REST API, kan du enkelt implementere strukturert logging med støtte for ulike loggmål som konsoll, filer, og logganalyseverktøy som Seq.