# ASP.NET Core Middleware vs. Filter Pipeline (Utvidet)  

> **.NET 8 – Controllers (MVC)**  
> Denne guiden forklarer **hva filter pipeline er**, **hvordan den relaterer seg til middleware**, og **konkrete måter å bruke/registrere filters** (attributt, globalt, `ServiceFilter`, `TypeFilter`). Inkluderer kodeeksempler du kan lime rett inn.

---

## 1) Oversikt: Middleware vs. Filters

### Middleware pipeline (global, lavnivå)

- Settes opp i `Program.cs` i rekkefølgen de kalles.
- Bearbeider **alle** requests (uavhengig av MVC).
- Egnet for cross-cutting concerns: HTTPS-redirect, CORS, authn, rate limiting, global error handling mv.

### Filter pipeline (MVC-nivå, høynivå)

- Aktiveres **etter** at routing har sendt en request til en controller/action (dvs. etter `app.MapControllers()` har matchet en route).
- Lar deg kjøre kode **før/etter** en action på controller-nivå.
- Filtertyper:
  - **Authorization filters** (`IAuthorizationFilter` / `IAsyncAuthorizationFilter`)
  - **Resource filters** (`IResourceFilter` / `IAsyncResourceFilter`)
  - **Action filters** (`IActionFilter` / `IAsyncActionFilter`)
  - **Exception filters** (`IExceptionFilter` / `IAsyncExceptionFilter`)
  - **Result filters** (`IResultFilter` / `IAsyncResultFilter`)

### Forenklet flyt

```
Client
  ↓
[Middleware pipeline]   -> Logging, Exception handling, AuthN, CORS, Routing, etc.
  ↓
[Filter pipeline]       -> Authorization → Resource → Action → (Controller Action) → Result → (Exception rundt)
  ↓
[Middleware pipeline]   -> Ev. responsbearbeiding (logging, headers)
  ↓
Client
```

---
<div style="page-break-after: always;"></div> 

## 2) Hvor ligger filter pipeline i forhold til middleware?

- **Middleware** kjører først og sist (rundt hele request/response-syklusen).
- Når routing peker mot en controller/action, går requesten inn i **filter pipeline** (MVC) før action-metoden kalles.
- Etter action/resultat er produsert, returnerer vi opp igjen gjennom filtere og videre ut gjennom resterende middleware.

---

## 3) Måter å bruke/registrere filters

Du har flere valg – de kan kombineres.

### A) Attributt på controller/action

- Når du har et filter som skal gjelde spesifikt et sted.
- Enten et *rått filter* via `ServiceFilter`/`TypeFilter`, eller en *wrapper-attributt* som peker til et filter.

```csharp
// 1) Direkte bruk av ServiceFilter (filter må være registrert i DI)
[ServiceFilter(typeof(LoggingActionFilter))]
public IActionResult Get() => Ok();

// 2) Wrapper-attributt som peker på et filter med TypeFilter
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiKeyAttribute : TypeFilterAttribute
{
    public ApiKeyAttribute() : base(typeof(ApiKeyAuthorizationFilter)) { }
}

// Bruk:
[ApiKey]
public IActionResult GetSecure() => Ok();
```

**Når bruke?** Når filteret er spesifikt for én eller få actions/controllere, eller når semantikken er viktig å “se” i koden (f.eks. `[ApiKey]`, `[RequireScope("products:write")]`).

---

### B) Global registrering (gjelder alle controllere/actions)

- Greit for logging, standardisering av header-svar, standard autorisasjonspolicy mv.

```csharp
builder.Services.AddControllers(options =>
{
    options.Filters.Add<LoggingActionFilter>(); // action filter
    options.Filters.Add<ApiKeyAttribute>();     // attributt som peker på authorization filter
});
```

**Når bruke?** Når du ønsker konsistent oppførsel i hele APIet og vil slippe å annotere hver action/controller.

---

### C) `ServiceFilter` vs. `TypeFilter` (DI-mønstre)

```csharp
// ServiceFilter: krever DI-registrering eksplisitt
builder.Services.AddScoped<LoggingActionFilter>();

[ServiceFilter(typeof(LoggingActionFilter))]
public IActionResult Get() => Ok();

// TypeFilter: trenger ikke eksplisitt DI-registrering av selve filteret
[TypeFilter(typeof(LoggingActionFilter))]
public IActionResult Get2() => Ok();

// Wrapper-attributt basert på TypeFilter (mest ergonomisk i praksis)
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireScopeAttribute : TypeFilterAttribute
{
    public RequireScopeAttribute(string scope) : base(typeof(RequireScopeFilter))
    {
        Arguments = new object[] { scope };
    }
}
```

**Forskjell i praksis**

- `ServiceFilter` → eksplisitt registrering i DI, godt for kontroll på lifetime & tydelig avhengighetsstyring.
- `TypeFilter` → praktisk når du vil slippe eksplisitt registrering, og/eller må sende inn **Arguments**.

---
<div style="page-break-after: always;"></div> 

## 4) Komplett minieksempel som viser alle tre måter

**Program.cs**

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    // Globalt filter
    options.Filters.Add<GlobalHeaderFilter>();
});

// For ServiceFilter (eksplisitt)
builder.Services.AddScoped<LoggingActionFilter>();

// For TypeFilter og wrapper-attributt trenger vi bare avhengighetene deres (hvis noen).
// F.eks. validator/loggere som filtrene bruker:
builder.Services.AddSingleton<ILoggerFactory, LoggerFactory>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();   // hvis aktuelt
app.UseAuthorization();

app.MapControllers();
app.Run();
```

**Filters/LoggingActionFilter.cs**

```csharp
public class LoggingActionFilter : IActionFilter
{
    private readonly ILogger<LoggingActionFilter> _logger;
    public LoggingActionFilter(ILogger<LoggingActionFilter> logger) => _logger = logger;

    public void OnActionExecuting(ActionExecutingContext context)
        => _logger.LogInformation("Før action: {Path}", context.HttpContext.Request.Path);

    public void OnActionExecuted(ActionExecutedContext context)
        => _logger.LogInformation("Etter action: {Path}", context.HttpContext.Request.Path);
}
```

**Filters/GlobalHeaderFilter.cs**

```csharp
public class GlobalHeaderFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        context.HttpContext.Response.Headers.TryAdd("X-App", "MyApi");
    }

    public void OnResultExecuted(ResultExecutedContext context) { }
}
```

**Filters/RequireScopeFilter.cs**

```csharp
public class RequireScopeFilter : IAsyncAuthorizationFilter
{
    private readonly string _scope;
    public RequireScopeFilter(string scope) => _scope = scope;

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var scopes = (IEnumerable<string>)(context.HttpContext.Items["scopes"] ?? Array.Empty<string>());
        if (!scopes.Contains(_scope))
        {
            context.Result = new ForbidResult();
        }
        return Task.CompletedTask;
    }
}
```

**Attributes/RequireScopeAttribute.cs (wrapper)**

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireScopeAttribute : TypeFilterAttribute
{
    public RequireScopeAttribute(string scope) : base(typeof(RequireScopeFilter))
    {
        Arguments = new object[] { scope };
    }
}
```

**Controllers/ProductsController.cs**

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // 1) ServiceFilter – per action
    [HttpGet("with-servicefilter")]
    [ServiceFilter(typeof(LoggingActionFilter))]
    public IActionResult GetA() => Ok(new { ok = true });

    // 2) TypeFilter – per action (uten eksplisitt DI-registrering av filteret)
    [HttpGet("with-typefilter")]
    [TypeFilter(typeof(LoggingActionFilter))]
    public IActionResult GetB() => Ok(new { ok = true });

    // 3) Wrapper-attributt – ergonomisk med argument
    [HttpPost("create")]
    [RequireScope("products:write")]
    public IActionResult Create([FromBody] object dto) => Ok(new { created = true });
}
```

---

## 5) Flere praktiske tips

- **Rekkefølge/ordering av filters**: Implementer `IOrderedFilter` og sett `Order` når rekkefølge betyr noe (lavere tall kjører først).
- **Short-circuit**: Mange filtertyper kan “avslutte tidlig” ved å sette `context.Result` (f.eks. returnere 401/403) – da kjøres ikke underliggende action.
- **Lifetimes**: Bruk `Scoped` for filters som leser fra `HttpContext`/tjenester per-request; `Singleton` bare hvis stateless og trådsikkert.
- **Global vs. lokalt**: Globalt gir konsistens; lokale attributter gir presis semantikk. Kombiner ved behov.
- **Exception handling**: Du kan ha global middleware for feil + MVC-**exception filters** for å standardisere svar på controller-nivå.
- **Minimal APIs**: Filter pipeline gjelder MVC/Controllers. For Minimal APIs bruker du typisk **middleware** og **endpoint filters** (`IEndpointFilter`) – et beslektet men annet API.

---

## 6) Oppsummering

- **Middleware**: globalt, lavnivå, før/etter hele MVC.  
- **Filters**: MVC-nivå, før/etter controller actions, med fine-grained kontroll og god testbarhet.  
- Bruk **attributter** for lokal/semantisk bruk, **global registrering** for konsistens, og **ServiceFilter/TypeFilter** for DI-støtte og parameterisering.

---
