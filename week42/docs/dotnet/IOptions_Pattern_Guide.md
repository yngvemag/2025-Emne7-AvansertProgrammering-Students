# The Options Pattern (`IOptions`) i .NET (ASP.NET Core / .NET 8)

> En praktisk guide til **hva** Options Pattern er, **hvorfor** du bør bruke det, og **hvordan** du setter det opp i en webapplikasjon (Minimal API og Controllers). Inkluderer binding fra `appsettings.json`, validering, *named options*, `IOptions`, `IOptionsSnapshot`, `IOptionsMonitor`, og tips til produksjon.

---

## 1. Hva er Options Pattern?

**Options Pattern** er en måte å håndtere sterktypet konfigurasjon på i .NET. I stedet for å lese magiske strenger fra konfigurasjon overalt i koden, **binder** du en konfigseksjon til en **POCO-klasse** og injiserer den der du trenger den.

**Fordeler**:

- Sterk typing og IntelliSense (tryggere og enklere å endre).
- Enkelt å teste (mock/overstyr verdier).
- Samler relaterte innstillinger på ett sted (enkelt vedlikehold).
- Støtter **validering** og **runtime-oppdatering** (med `IOptionsMonitor`).

---

## 2. Grunnleggende oppsett (Minimal API)

### 2.1. Definer en Options-klasse

```csharp
public class MyAppOptions
{
    public const string SectionName = "MyApp"; // valgfritt, men praktisk
    public string Title { get; set; } = "Demo";
    public int PageSize { get; set; } = 10;
    public string? ApiBaseUrl { get; set; }
}
```

### 2.2. Konfigurasjon (`appsettings.json`)

```json
{
  "MyApp": {
    "Title": "Min app",
    "PageSize": 20,
    "ApiBaseUrl": "https://api.example.com"
  }
}
```
<div style="page-break-after: always;"></div> 

### 2.3. Registrer binding i `Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

// Binder "MyApp" til MyAppOptions og aktiverer validering ved oppstart (se seksjon 5)
builder.Services.AddOptions<MyAppOptions>()
    .Bind(builder.Configuration.GetSection(MyAppOptions.SectionName))
    .ValidateDataAnnotations()       // valgfritt hvis du bruker [Required], [Range], osv.
    .ValidateOnStart();              // valider ved oppstart (kaster ved feil)

var app = builder.Build();
```

### 2.4. Bruk i endepunkter (Minimal API)

```csharp
app.MapGet("/info", (Microsoft.Extensions.Options.IOptions<MyAppOptions> opts) =>
{
    var o = opts.Value;
    return Results.Ok(new { o.Title, o.PageSize, o.ApiBaseUrl });
});
```

---

## 3. Bruk i Controllers (MVC / Web API)

```csharp
[ApiController]
[Route("api/[controller]")]
public class InfoController : ControllerBase
{
    private readonly MyAppOptions _opts;
    public InfoController(Microsoft.Extensions.Options.IOptions<MyAppOptions> options)
    {
        _opts = options.Value;
    }

    [HttpGet]
    public IActionResult Get() => Ok(new { _opts.Title, _opts.PageSize, _opts.ApiBaseUrl });
}
```

Registrering i `Program.cs` er identisk (seksjon 2.3).

---
<div style="page-break-after: always;"></div> 

## 4. Tre varianter: `IOptions`, `IOptionsSnapshot`, `IOptionsMonitor`

| Variant | Scope | Oppdateringer | Typisk bruk |
|---|---|---|---|
| `IOptions<T>` | Singleton-ish | **Statisk**: leses én gang ved DI-resolusjon | Enkle apper hvor innstillinger ikke endres under runtime |
| `IOptionsSnapshot<T>` | **Scoped** (per request) | **Reload on request** (ved filendring + `reloadOnChange: true`) | Web-apper der du vil plukke opp endringer per request uten å restarte |
| `IOptionsMonitor<T>` | Singleton | **Umiddelbare** endringer + **OnChange** callback | Tjenester som må reagere på endringer (f.eks. bytte API-nøkkel dynamisk) |

### 4.1. IOptionsSnapshot-eksempel (Controllers)

```csharp
public class InfoController : ControllerBase
{
    private readonly MyAppOptions _opts;
    public InfoController(Microsoft.Extensions.Options.IOptionsSnapshot<MyAppOptions> options)
    {
        _opts = options.Value; // Snapshot per HTTP-request
    }
}
```

### 4.2. IOptionsMonitor med OnChange (bakgrunnstjeneste eller singleton)

```csharp
public class DynamicService
{
    private MyAppOptions _current;

    public DynamicService(Microsoft.Extensions.Options.IOptionsMonitor<MyAppOptions> monitor)
    {
        _current = monitor.CurrentValue;
        monitor.OnChange(newOptions => _current = newOptions);
    }

    public string CurrentTitle => _current.Title;
}
```

> For at endringer i `appsettings.json` skal plukkes opp under kjøring: sørg for at `CreateBuilder` settes opp med `reloadOnChange: true` (standard i WebApplicationBuilder) **og** at kilden faktisk endres på disk.

---

## 5. Validering av options

### 5.1. DataAnnotation-baserte attributter

```csharp
using System.ComponentModel.DataAnnotations;

public class SmtpOptions
{
    [Required, EmailAddress] public string From { get; set; } = default!;
    [Required] public string Host { get; set; } = default!;
    [Range(1, 65535)] public int Port { get; set; } = 25;
    public bool UseSsl { get; set; } = true;
}
```

Registrering:

```csharp
builder.Services.AddOptions<SmtpOptions>()
    .Bind(builder.Configuration.GetSection("Smtp"))
    .ValidateDataAnnotations()
    .ValidateOnStart(); // valider ved oppstart
```

### 5.2. Egendefinert validering (predicate)

```csharp
builder.Services.AddOptions<MyAppOptions>()
    .Bind(builder.Configuration.GetSection("MyApp"))
    .Validate(o => o.PageSize > 0 && o.PageSize <= 500, "PageSize must be 1..500")
    .ValidateOnStart();
```

### 5.3. IValidateOptions<T>

```csharp
public class MyAppOptionsValidator : Microsoft.Extensions.Options.IValidateOptions<MyAppOptions>
{
    public Microsoft.Extensions.Options.ValidateOptionsResult Validate(string? name, MyAppOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Title))
            return Microsoft.Extensions.Options.ValidateOptionsResult.Fail("Title is required.");
        return Microsoft.Extensions.Options.ValidateOptionsResult.Success;
    }
}

// Registrer
builder.Services.AddSingleton<Microsoft.Extensions.Options.IValidateOptions<MyAppOptions>, MyAppOptionsValidator>();
```

---
<div style="page-break-after: always;"></div> 

## 6. Named Options (flere instanser av samme type)

Når du trenger **flere profiler** av samme options-type (f.eks. ulike SMTP-oppkoblinger).

```csharp
builder.Services.AddOptions<SmtpOptions>("Primary")
    .Bind(builder.Configuration.GetSection("Smtp:Primary"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<SmtpOptions>("Backup")
    .Bind(builder.Configuration.GetSection("Smtp:Backup"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

Bruk i kode:

```csharp
public class Mailer
{
    private readonly SmtpOptions _primary;
    private readonly SmtpOptions _backup;

    public Mailer(Microsoft.Extensions.Options.IOptionsSnapshot<SmtpOptions> snapshot)
    {
        _primary = snapshot.Get("Primary");
        _backup  = snapshot.Get("Backup");
    }
}
```

---
<div style="page-break-after: always;"></div> 

## 7. PostConfigure og ConfigureOptions

### 7.1. PostConfigure (overstyr/berik verdier etter binding)

```csharp
builder.Services.PostConfigure<MyAppOptions>(o =>
{
    if (string.IsNullOrEmpty(o.ApiBaseUrl))
        o.ApiBaseUrl = "https://localhost:5001";
});
```

### 7.2. IConfigureOptions<T> (enkapsuler setup i en klasse)

```csharp
public class MyAppOptionsSetup : Microsoft.Extensions.Options.IConfigureOptions<MyAppOptions>
{
    private readonly IConfiguration _config;
    public MyAppOptionsSetup(IConfiguration config) => _config = config;

    public void Configure(MyAppOptions options)
    {
        _config.GetSection(MyAppOptions.SectionName).Bind(options);
        if (options.PageSize <= 0) options.PageSize = 10;
    }
}

// Registrer
builder.Services.AddSingleton<Microsoft.Extensions.Options.IConfigureOptions<MyAppOptions>, MyAppOptionsSetup>();
```

> `IConfigureOptions<T>` kjører **før** validering; `IPostConfigureOptions<T>` kjører **etter**.

---
<div style="page-break-after: always;"></div> 

## 8. Struktur-anbefaling (Infrastructure)

Hold options samlet under *Infrastructure/Options* for gjenbruk:

```
/src
 ├─ /Infrastructure
 │   └─ /Options
 │       ├─ MyAppOptions.cs
 │       ├─ SmtpOptions.cs
 │       ├─ ...(flere)
 │
 ├─ Program.cs
 └─ appsettings.json
```

For større løsninger kan du ha `Infrastructure/Options`, `Infrastructure/Configuration` og `Infrastructure/Extensions` (f.eks. `AddMyAppOptions(this IServiceCollection, IConfiguration)` som samler bindingskall).

---
<div style="page-break-after: always;"></div> 

## 9. Helhetlig eksempel (Minimal API)

**`MyAppOptions.cs`**

```csharp
public class MyAppOptions
{
    public const string SectionName = "MyApp";
    public string Title { get; set; } = "Demo";
    public int PageSize { get; set; } = 10;
    public string? ApiBaseUrl { get; set; }
}
```

**`appsettings.json`**

```json
{
  "MyApp": {
    "Title": "Min app",
    "PageSize": 25,
    "ApiBaseUrl": "https://api.example.com"
  }
}
```

**`Program.cs`**

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<MyAppOptions>()
    .Bind(builder.Configuration.GetSection(MyAppOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var app = builder.Build();

app.MapGet("/info", (Microsoft.Extensions.Options.IOptions<MyAppOptions> opts) =>
{
    var o = opts.Value;
    return Results.Ok(new { o.Title, o.PageSize, o.ApiBaseUrl });
});

app.Run();
```

---
<div style="page-break-after: always;"></div> 

## 10. Tips til produksjon

- **Hemmelige nøkler** i Key Vault/Secret Manager, ikke i repo.
- Bruk **`ValidateOnStart()`** for å fange feil tidlig ved deploy.
- For runtime-endringer: `IOptionsMonitor` + `OnChange` (og `reloadOnChange: true`).  
- Logg hvilke profiler (navn) du bruker ved **named options**.
- Hold options **små og fokuserte** (separer ansvar per feature/tjeneste).

---

## 11. Feilsøking

- Får du `Value cannot be null`? Sjekk at **seksjonsnavn** matcher `appsettings.json`.
- Valideringsfeil ved oppstart? Se logger; `ValidateOnStart` kaster med detaljer.
- Endringer slår ikke inn? Sjekk at filen faktisk endres på disk og at du bruker `IOptionsSnapshot`/`IOptionsMonitor` riktig.

---

### Oppsummering

- **Options Pattern** = sterktypet konfigurasjon via DI.
- Bind konfig → valider → bruk via `IOptions*`-varianter som passer ditt scenario.
- Strukturer options under **Infrastructure** og unngå magic strings spredt i koden.

God tur videre med `IOptions` 🚀
