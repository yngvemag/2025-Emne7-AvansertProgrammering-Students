# Unntakshåndtering i ASP.NET Core med Middleware

## Hva er Unntakshåndtering i Middleware?

Unntakshåndtering i ASP.NET Core kan effektivt implementeres ved hjelp av middleware, som er en komponent i applikasjonens behandlingspipeline. Middleware for unntakshåndtering fanger opp ubehandlede unntak som kastes under behandling av HTTP-forespørsler og håndterer dem på en konsistent måte, for eksempel ved å logge unntakene og returnere en egnet HTTP-respons til klienten.

## Oppsett av Unntakshåndtering i Middleware

### 1. Bruk `UseExceptionHandler` Middleware

`UseExceptionHandler` er et innebygd middleware som håndterer unntak på en global skala. Det fanger opp unntak, logger dem, og kan rute brukere til en egendefinert feilside.

#### Eksempel:
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error"); // Bruker en egendefinert feilside
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    });
}
```

### 2. Opprette en Egendefinert Feilhåndteringsside
Du kan opprette en egendefinert feilside som /Home/Error, som brukes når et unntak oppstår. Dette gir en mer brukervennlig opplevelse.

Eksempel på Controller:
```csharp
public class HomeController : Controller
{
    public IActionResult Error()
    {
        // Her kan du returnere en visning som viser feilmeldingen
        return View();
    }
}
```

### 3. Håndtering av API-feil (JSON-resultat)
For API-applikasjoner er det ofte ønskelig å returnere feil i JSON-format, heller enn å bruke en feilside.

Eksempel på bruk av UseExceptionHandler for API:
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseExceptionHandler(builder =>
    {
        builder.Run(async context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var error = context.Features.Get<IExceptionHandlerFeature>();
            if (error != null)
            {
                var result = JsonSerializer.Serialize(new { error = error.Error.Message });
                await context.Response.WriteAsync(result);
            }
        });
    });

    // Andre middleware
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```
### 4. Bruke UseDeveloperExceptionPage i Utviklingsmiljø
I utviklingsmiljøer kan du bruke UseDeveloperExceptionPage for å få detaljerte feilmeldinger og stack traces i nettleseren, noe som er nyttig for debugging.

Eksempel:
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    // Resten av pipeline
}
```
### 5. Global Feilhåndtering med Egendefinert Middleware
Du kan også lage en egendefinert middleware for mer kontroll over unntakshåndtering.

```csharp
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong: {ex}");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var result = JsonSerializer.Serialize(new { error = "Internal Server Error" });
        return context.Response.WriteAsync(result);
    }
}
```
Registrere Egendefinert Middleware:
```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseMiddleware<ExceptionMiddleware>();

    // Resten av pipeline
}
```csharp

### Oppsummering
Unntakshåndtering med middleware i ASP.NET Core gir en robust måte å fange opp og håndtere ubehandlede unntak i applikasjonen. Ved å bruke UseExceptionHandler, UseDeveloperExceptionPage, eller til og med egendefinert middleware, kan du sørge for at applikasjonen din håndterer feil på en kontrollert og brukervennlig måte.

UseExceptionHandler: Håndterer globale unntak og kan rute til egendefinerte feilsider eller returnere JSON-resultater.
UseDeveloperExceptionPage: Viser detaljerte feil i utviklingsmiljøer.
Egendefinert Middleware: Gir full kontroll over unntakshåndteringen i applikasjonen.
Ved å implementere disse teknikkene kan du forbedre påliteligheten og brukeropplevelsen av din ASP.NET Core-applikasjon.
