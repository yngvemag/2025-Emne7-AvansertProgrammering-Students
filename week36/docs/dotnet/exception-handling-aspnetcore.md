
# Exception ASP.NET Core (.NET 8/9)

> Sist oppdatert: 2025-09-03

Denne guiden viser **hva unntak (exceptions) er**, **hvorfor god unntakshåndtering er viktig**, **hvorfor du aldri skal sende stack trace til klient**, og **hvordan du implementerer moderne, global unntakshåndtering i ASP.NET Core** (minimal APIs og MVC) ved hjelp av `IExceptionHandler` og RFC 7807 (*Problem Details*). Den sammenligner også **global** vs **lokal** (per endepunkt / filter) håndtering, med eksempler.

---

## Hva er et unntak (excpetion) – og hvorfor bry seg?

Et unntak representerer en **uventet feiltilstand** som avbryter normal flyt i programmet. Ubehandlede unntak fører ofte til HTTP 500 (Internal Server Error) og potensielt inkonsistente svar.

**God unntakshåndtering gir deg:**

- **Konsistente API-svar** i et standardisert format (Problem Details / RFC 7807).
- **Sikkerhet** – ingen sensitiv informasjon lekker ut.
- **Observability** – logger med korrelasjons-ID/trace ID gjør feilsøking enklere.
- **Separation of concerns** – domenekode slipper `try/catch`-duplisering i endepunkter.

---

## Hvorfor du ikke skal returnere stack trace til klienten

- **Sikkerhet:** Stack traces kan avsløre interne klassenavn, filbaner, database-/serverdetaljer og biblioteker som letter angrep.
- **Stabil kontrakt:** Klienten skal forholde seg til **forutsigbare feiltyper**, ikke interne implementasjonsdetaljer.
- **Støy:** Stack traces hjelper utviklere, men forvirrer klienter. Logg dem internt via `ILogger`/APM (f.eks. Serilog, Seq, OpenTelemetry).

**Kort regel:** *Stack trace til logger – ikke til klient.*

---
<div style="page-break-after: always;"></div>

## Anbefalt tilnærming i .NET 8/9

ASP.NET Core har støtte for global unntakshåndtering via `IExceptionHandler` + `UseExceptionHandler()` og innebygget Problem Details-tjeneste.

### 1) Registrer Problem Details og Exception Handler (Program.cs)

```csharp
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Om man ønsker å tilpasse ProblemDetails kan man legge dette til 
// 1) ProblemDetails – RFC 7807 som standard feil-respons
builder.Services.AddProblemDetails(options =>
{
    // Ikke lek ut detaljer i prod
    options.IncludeExceptionDetails = (ctx, ex) =>
        builder.Environment.IsDevelopment();

    // Legg til traceId i alle problem responses
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
    };
});

// 2) Din globale exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// I utvikling er det greit å se dev-side (alternativt stol kun på ProblemDetails)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// 3) Aktiver global exception handling
app.UseExceptionHandler(); // Bruker registrert IExceptionHandler
//app.UseExceptionHandler(_ => {})

app.MapGet("/boom", () =>
{
    // Demo: kast en feil
    throw new InvalidOperationException("Nope");
});

app.Run();
```
<div style="page-break-after: always;"></div>

### 2) Implementér din `GlobalExceptionHandler`

Under er en videreføring av koden du allerede har, men med noen forbedringer: flere mappings, Problem Details og tydelig logging.

```csharp
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PersonRestAPI.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1) Logg alltid hele exception (inkl. stack trace) til logg-backend
        _logger.LogError(exception,
            "Request feilet på maskin {Machine}. TraceId:{TraceId}",
            Environment.MachineName, httpContext.TraceIdentifier);

        // 2) Map exception -> (statusCode, title, (optional) detail, (optional) extensions)
        var (statusCode, title, detail, extensions) = MapException(exception);

        // 3) Returnér ProblemDetails. Ikke send stack trace til klient.
        var problem = Results.Problem(
            statusCode: statusCode,
            title: title,
            detail: detail,
            extensions: MergeExtensions(extensions, new Dictionary<string, object?>
            {
                ["traceId"] = httpContext.TraceIdentifier
            })
        );

        await problem.ExecuteAsync(httpContext);
        return true; // Stopp videre pipeline
    }

    private static (int statusCode, string title, string? detail, Dictionary<string, object?>? extensions)
        MapException(Exception ex)
        => ex switch
        {
            // 400 – feil input
            ArgumentNullException ane => (StatusCodes.Status400BadRequest,
                "Ugyldig eller manglende parameter",
                $"Parameter: {ane.ParamName ?? "(ukjent)"}",
                null),

            ArgumentException ae => (StatusCodes.Status400BadRequest,
                "Ugyldig argument",
                ae.Message,
                null),

            // 401/403 – tilgang
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden,
                "Ikke tilgang",
                "Du har ikke rettigheter til å utføre denne handlingen.",
                null),

            // 404 – ikke funnet (eksempel på domenefeil)
            PersonNotFoundException pnfe => (StatusCodes.Status404NotFound,
                "Ressurs ikke funnet",
                pnfe.Message,
                new() { ["personId"] = pnfe.PersonId }),

            KeyNotFoundException => (StatusCodes.Status404NotFound,
                "Ressurs ikke funnet",
                "Objektet finnes ikke.",
                null),

            // 409 – konflikt (eksempel)
            InvalidOperationException ioe => (StatusCodes.Status409Conflict,
                "Ugyldig operasjon",
                ioe.Message,
                null),

            // 400 – HTTP parsing feil
            BadHttpRequestException bhe => (StatusCodes.Status400BadRequest,
                "Ugyldig forespørsel",
                bhe.Message,
                null),

            // 500 – alt annet
            _ => (StatusCodes.Status500InternalServerError,
                "Intern feil",
                "Vi jobber med saken.",
                null)
        };

    private static Dictionary<string, object?> MergeExtensions(
        Dictionary<string, object?>? a,
        Dictionary<string, object?> b)
    {
        a ??= new();
        foreach (var kv in b)
            a[kv.Key] = kv.Value;
        return a;
    }
}

// Eksempel på domenespesifikk exception
public sealed class PersonNotFoundException : Exception
{
    public PersonNotFoundException(Guid personId)
        : base($"Person med id {personId} finnes ikke.") => PersonId = personId;

    public Guid PersonId { get; }
}
```

**Hva er nytt her?**

- Bruk av **Problem Details** i alle svar (RFC 7807).
- **Utvidelser (extensions)** for å sende `traceId` og domene-relevante biter (f.eks. `personId`) uten å lekke stack trace.
- Flere **mappings** til semantisk riktige HTTP-statuskoder.

---
<div style="page-break-after: always;"></div>

## Sammenligning: Global vs. Lokal unntakshåndtering

| Tilnærming | Fordeler | Ulemper | Når bruke |
|---|---|---|---|
| **Global (`IExceptionHandler` + `UseExceptionHandler`)** | Én kilde til sannhet, konsistent respons (Problem Details), gjelder alle endepunkter, bra for minimal APIs og MVC. | Mindre granularitet for helt spesielle endepunkt-behov. | **Som standard i alle API-prosjekter.** |
| **MVC Exception Filters (`IExceptionFilter`/`IAsyncExceptionFilter`)** | Fungerer fint i MVC; kan scope’e til controllers/areas. | Gjelder **ikke** minimal APIs, og kan blandes med middleware på uforutsigbare måter. | Når du **kun** bruker MVC og vil ha controller-spesifikke policies. |
| **Lokal `try/catch` i endepunkt** | Raskt for enkeltstående, spesielle tilfeller. | Duplisering, inkonsistente svar, lett å glemme logging/traceId. | Kun ved helt spesielle forretningsregler – ellers unngå. |

**Anbefaling:** Bruk **global** håndtering som baseline. Supplér med lokale `try/catch` kun der forretningslogikk krever det (f.eks. *happy-path fallback*), men returnér fortsatt Problem Details for feil.

---
<div style="page-break-after: always;"></div>

## Eksempel: Minimal API med domenefeil

```csharp
app.MapGet("/persons/{id:guid}", (Guid id) =>
{
    var person = GetPerson(id); // henter evt. null
    if (person is null)
        throw new PersonNotFoundException(id);

    return Results.Ok(person);
});
```

Feilen fanges globalt og klienten får en respons som ligner på:

```json
{
  "type": "about:blank",
  "title": "Ressurs ikke funnet",
  "status": 404,
  "detail": "Person med id 11111111-2222-3333-4444-555555555555 finnes ikke.",
  "traceId": "0HMP..."
}
```

---

## Integrasjon med logging (Serilog/Seq/OpenTelemetry)

- **Logg alt i handleren** via `ILogger`. Legg til `TraceId` som scope eller i melding.
- Valgfritt: Sett opp Serilog sink til **Seq** for søkbarhet over tid.
- Vurder **OpenTelemetry** for distribuerte traces på tvers av tjenester.

```csharp
// appsettings.json (Serilog – eksempel)
{
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/app-.log", "rollingInterval": "Day" } }
    ],
    "Enrich": [ "FromLogContext" ]
  }
}
```

---
<div style="page-break-after: always;"></div>

## Ofte stilte spørsmål

**Bør jeg returnere `exception.Message` til klienten?**  
Som hovedregel: **nei** i produksjon. Bruk en trygg, kortfattet `title`/`detail`. I utvikling kan du eksponere mer ved å sette `IncludeExceptionDetails` til `true` når `Environment.IsDevelopment()`.

**Hva med valideringsfeil?**  
Returnér **400** med en *Validation Problem Details*-struktur (modellvalidering, FluentValidation). Det kan du håndtere lokalt i endepunkt eller ved å kaste en egen `ValidationException` som global handler oversetter til `status=400` og `errors`-felt i `extensions`.

---
<div style="page-break-after: always;"></div>

## Oppsummering

- Bruk **global unntakshåndtering** med `IExceptionHandler` + `UseExceptionHandler()`.
- Returnér kun **Problem Details** til klient – **ikke** stack trace.
- Logg **hele exception** med `ILogger`/APM. Legg ved **traceId** i responsen.
- Map kjente domenefeil til semantisk riktige HTTP-statuskoder (400/401/403/404/409/500).

---

## Fullstendig eksempel (Program.cs + handler)

**Program.cs**

```csharp
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PersonRestAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(options =>
{
    options.IncludeExceptionDetails = (ctx, ex) => builder.Environment.IsDevelopment();
    options.CustomizeProblemDetails = ctx =>
        ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.MapGet("/demo/arg", () => throw new ArgumentNullException("name"));
app.MapGet("/demo/not-found", () => throw new PersonNotFoundException(Guid.NewGuid()));
app.MapGet("/demo/conflict", () => throw new InvalidOperationException("Kan ikke endre status."));
app.MapGet("/demo/boom", () => throw new Exception("Noe gikk skikkelig galt."));

app.Run();
```

**GlobalExceptionHandler.cs** – se implementasjonen i kapittelet over.

---

## Migreringstips fra eldre oppsett

- Har du brukt `app.UseExceptionHandler(builder => ...)` med manuell JSON? Bytt til **`AddProblemDetails()` + `Results.Problem(...)`** eller `IProblemDetailsService` for konsistens.
- Bruk **`AddExceptionHandler<THandler>()` + `app.UseExceptionHandler()`** i stedet for en egendefinert middleware, med mindre du har helt spesielle behov.
- Pass på rekkefølgen i pipeline: `UseExceptionHandler()` bør komme **tidlig**, før routing/autorisasjon som kan kaste.

---

Lykke til! Med dette oppsettet får du **sikre, konsistente og moderne** feilsvar – og enklere feilsøking.
