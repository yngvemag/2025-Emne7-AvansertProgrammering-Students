# Dokumentasjon: StudentBloggApiKeyAuthentication i ASP.NET Core

## 1. Introduksjon

`StudentBloggApiKeyAuthentication` er en egendefinert autentiseringshandler for ASP.NET Core som validerer **API Keys** sendt i HTTP-forespørsler. Løsningen passer for maskin-til-maskin-tilgang (tjenester, skript, integrasjoner) der brukere ikke logger inn med brukernavn/passord, men identifiserer seg med en nøkkel.

Med denne handleren kan du:

- Beskytte endepunkter med `[Authorize(AuthenticationSchemes = "ApiKey")]` eller globale policies.
- Lese nøkkel fra enten `Authorization`-header (scheme `ApiKey`) **eller** fra en egen header (f.eks. `X-API-Key`).
- Koble validering mot appsettings, database eller ekstern tjeneste.
- Tilordne **claims** (f.eks. `appId`, `scope`) for videre autorisasjon.

> Merk: API Keys identifiserer typisk **applikasjoner**, ikke sluttbrukere. For brukeridentitet, vurder OAuth2/OIDC eller Basic/Token-basert auth.

---

## 2. Hva må klienten sende?

Du kan støtte ett eller flere formater. De to vanligste er:

### 2.1 `Authorization`-header

```
Authorization: ApiKey <din-api-nøkkel>
```

### 2.2 Egen header

```
X-API-Key: <din-api-nøkkel>
```

### 2.3 Eksempelverdier

- API Key: `sk_live_2b8c0f7a...` (eksempel)
- `Authorization` header:  
  `Authorization: ApiKey sk_live_2b8c0f7a...`
- `X-API-Key` header:  
  `X-API-Key: sk_live_2b8c0f7a...`

### 2.4 Test med cURL

```bash
# Via Authorization-header
curl -H "Authorization: ApiKey sk_live_2b8c0f7a..." https://api.dittdomene.no/api/securedata

# Via X-API-Key-header
curl -H "X-API-Key: sk_live_2b8c0f7a..." https://api.dittdomene.no/api/securedata
```

---

## 3. Oversikt: AuthenticationHandler vs IMiddleware

Akkurat som for Basic, bør API Key-validering implementeres som en **AuthenticationHandler** for å integrere med:

- `[Authorize]`-attributter
- Policies/roller/claims
- `HttpContext.User` (ClaimsPrincipal)

Dette gir en standardisert og testbar tilnærming.

---

## 4. Implementasjon

Nedenfor følger to varianter:

1) **Enkel (statisk)** – leser gyldige nøkler fra `appsettings.json`.
2) **Avansert (validator-tjeneste)** – abstraherer validering (database, cache, ekstern tjeneste).

### 4.1 appsettings.json (statisk liste)

```json
{
  "Authentication": {
    "ApiKeys": [
      { "key": "sk_test_123", "appId": "test-app", "scopes": [ "posts:read" ] },
      { "key": "sk_live_ABC", "appId": "prod-reporter", "scopes": [ "posts:read", "posts:write" ] }
    ]
  }
}
```

### 4.2 Handler (enkel versjon)

```csharp
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

public class ApiKeyItem
{
    public string key { get; set; } = "";
    public string appId { get; set; } = "";
    public string[] scopes { get; set; } = Array.Empty<string>();
}

public class StudentBloggApiKeyAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _config;
    public const string SchemeName = "ApiKey";
    private const string HeaderName = "X-API-Key";
    private const string AuthorizationPrefix = "ApiKey ";

    public StudentBloggApiKeyAuthentication(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration config) : base(options, logger, encoder, clock)
    {
        _config = config;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // 1) Ekstraher nøkkel fra Authorization: ApiKey <key> eller X-API-Key
        string? apiKey = null;

        if (Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
        {
            var authHeader = authHeaderValues.ToString();
            if (authHeader.StartsWith(AuthorizationPrefix, StringComparison.OrdinalIgnoreCase))
            {
                apiKey = authHeader.Substring(AuthorizationPrefix.Length).Trim();
            }
        }

        if (string.IsNullOrWhiteSpace(apiKey) && Request.Headers.TryGetValue(HeaderName, out var keyHeader))
        {
            apiKey = keyHeader.ToString().Trim();
        }

        if (string.IsNullOrWhiteSpace(apiKey))
            return Task.FromResult(AuthenticateResult.Fail("API Key mangler."));

        // 2) Last gyldige nøkler fra config
        var items = _config.GetSection("Authentication:ApiKeys").Get<List<ApiKeyItem>>() ?? new();

        var match = items.FirstOrDefault(x => x.key == apiKey);
        if (match is null)
            return Task.FromResult(AuthenticateResult.Fail("Ugyldig API Key."));

        // 3) Bygg claims (identifiser appen + scopes)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, match.appId),
            new Claim("appId", match.appId),
        };

        foreach (var scope in match.scopes)
            claims.Add(new Claim("scope", scope));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
```

### 4.3 Program.cs – registrering

```csharp
// builder.Services.AddAuthorization(); // som regel allerede på plass
builder.Services.AddAuthentication(StudentBloggApiKeyAuthentication.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, StudentBloggApiKeyAuthentication>(
        StudentBloggApiKeyAuthentication.SchemeName, options => { });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
```

### 4.4 Beskytte endepunkter

```csharp
[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    // Gjelder kun for ApiKey-skjemaet
    [Authorize(AuthenticationSchemes = StudentBloggApiKeyAuthentication.SchemeName)]
    [HttpGet("secure")]
    public IActionResult GetSecure()
    {
        var appId = User.FindFirstValue("appId");
        return Ok(new { message = $"Hei {appId}, dette er en beskyttet ressurs." });
    }

    // Eksempel på scope-krav via policy
    [Authorize(AuthenticationSchemes = StudentBloggApiKeyAuthentication.SchemeName, Policy = "CanWritePosts")]
    [HttpPost("secure")]
    public IActionResult CreateSecure([FromBody] object payload)
    {
        return Ok(new { status = "created" });
    }
}
```

### 4.5 Legge til policy som sjekker scope

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanWritePosts", policy =>
        policy.RequireClaim("scope", "posts:write"));
});
```

---

## 5. Avansert variant: Validator-tjeneste (DB, cache, ekstern)

For større systemer bør validering ikke ligge i handleren, men abstraheres bak et grensesnitt. Dette forenkler testing, nøkkelrotasjon, rate limiting, logging, osv.

### 5.1 Grensesnitt

```csharp
public interface IApiKeyValidator
{
    Task<(bool ok, string appId, IEnumerable<string> scopes)> ValidateAsync(string apiKey, CancellationToken ct);
}
```

### 5.2 Implementasjonseksempel (fra database)

```csharp
public class DbApiKeyValidator : IApiKeyValidator
{
    private readonly YourDbContext _db;
    public DbApiKeyValidator(YourDbContext db) => _db = db;

    public async Task<(bool ok, string appId, IEnumerable<string> scopes)> ValidateAsync(string apiKey, CancellationToken ct)
    {
        var record = await _db.ApiKeys
            .Where(k => k.KeyHash == Hash(apiKey) && k.IsActive)
            .Select(k => new { k.AppId, Scopes = k.Scopes.Select(s => s.Value) })
            .FirstOrDefaultAsync(ct);

        if (record is null) return (false, "", Array.Empty<string>());
        return (true, record.AppId, record.Scopes);
    }

    private static string Hash(string value) => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value)));
}
```

### 5.3 Handler som bruker validatoren

```csharp
public class StudentBloggApiKeyAuthenticationV2 : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "ApiKey";
    private const string HeaderName = "X-API-Key";
    private const string AuthorizationPrefix = "ApiKey ";

    private readonly IApiKeyValidator _validator;
    public StudentBloggApiKeyAuthenticationV2(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IApiKeyValidator validator) : base(options, logger, encoder, clock)
    {
        _validator = validator;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? apiKey = null;

        if (Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
        {
            var authHeader = authHeaderValues.ToString();
            if (authHeader.StartsWith(AuthorizationPrefix, StringComparison.OrdinalIgnoreCase))
            {
                apiKey = authHeader.Substring(AuthorizationPrefix.Length).Trim();
            }
        }

        if (string.IsNullOrWhiteSpace(apiKey) && Request.Headers.TryGetValue(HeaderName, out var keyHeader))
        {
            apiKey = keyHeader.ToString().Trim();
        }

        if (string.IsNullOrWhiteSpace(apiKey))
            return AuthenticateResult.Fail("API Key mangler.");

        var (ok, appId, scopes) = await _validator.ValidateAsync(apiKey, HttpContext.RequestAborted);
        if (!ok) return AuthenticateResult.Fail("Ugyldig API Key.");

        var claims = new List<Claim> { new Claim("appId", appId), new Claim(ClaimTypes.NameIdentifier, appId) };
        claims.AddRange(scopes.Select(s => new Claim("scope", s)));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
```

### 5.4 Registrering av V2 med validator

```csharp
builder.Services.AddScoped<IApiKeyValidator, DbApiKeyValidator>();

builder.Services.AddAuthentication(StudentBloggApiKeyAuthenticationV2.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, StudentBloggApiKeyAuthenticationV2>(
        StudentBloggApiKeyAuthenticationV2.SchemeName, options => { });
```

---

## 6. Feilhåndtering og tilbakemeldinger

Standard atferd for mislykket autentisering er **401 Unauthorized**. Du kan i tillegg konfigurere problem-details i pipeline eller skrive responser i handleren, men unngå å lekke sensitiv info.

Vanlige feil og årsaker:

- **401 – API Key mangler**: Ingen `Authorization` eller `X-API-Key`-header.
- **401 – Ugyldig API Key**: Nøkkelen finnes ikke/er deaktivert.
- **403 – Ikke nok tilganger**: Autentisert, men mangler `scope`/policy.

---

## 7. Sikkerhetspraksis

- **Bruk HTTPS** – API Keys sendes i klartekst i headeren og må beskyttes på transportlaget.
- **Hash nøkler i lagring** – lagre `SHA-256` (eller bedre) hash i DB, ikke rå nøkler.
- **Rotasjon** – støtte flere aktive nøkler per app og utløpsdato.
- **Rate limiting** – begrens misbruk pr. nøkkel (f.eks. via YARP/NGINX/ASP.NET rate limiting).
- **Logging og revisjon** – logg `appId` og viktige hendelser (ikke selve nøkkelen).
- **Least privilege** – gi kun nødvendige `scopes` per nøkkel.
- **Ikke i URL** – aldri send API Keys som query-parametere.
- **CORS** – om API brukes fra nettleser, vurder CORS nøye; API Keys i frontend er sårbart.

---

## 8. Eksempler på bruk i Controller og policy

```csharp
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    [Authorize(AuthenticationSchemes = "ApiKey", Policy = "CanReadReports")]
    [HttpGet("daily")]
    public IActionResult GetDailyReport()
    {
        var appId = User.FindFirstValue("appId");
        return Ok(new { generatedFor = appId, date = DateTime.UtcNow, data = new [] { 1, 2, 3 } });
    }
}
```

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanReadReports", p => p.RequireClaim("scope", "reports:read"));
});
```

---

## 9. Enhetstesting (skisse)

- **Handler**: Mock `IConfiguration`/`IApiKeyValidator`, bygg `DefaultHttpContext` med headers, kall `HandleAuthenticateAsync()`, verifiser `AuthenticateResult` og claims.
- **Policy**: Bruk `AuthorizationHandler`-tester med en `ClaimsPrincipal` som har/ikke har riktige `scope`-claims.
- **Controller**: Test at endepunktet returnerer 401/403/200 avhengig av setup.

---

## 10. Oppsummering av flyt

1. Klienten sender `Authorization: ApiKey <key>` eller `X-API-Key: <key>`.
2. Handleren ekstraherer nøkkel og validerer (config/DB/ekstern).
3. Ved suksess opprettes `ClaimsPrincipal` med `appId` og `scope`-claims.
4. `[Authorize]` og policies håndhever tilgang.
5. Feil gir 401 (mangler/ugyldig) eller 403 (utilstrekkelig scope).

---

## 11. Referanser

- Microsoft Docs – Authentication handlers i ASP.NET Core
- OWASP API Security Top 10
- RFC 7235 – HTTP Authentication Framework
