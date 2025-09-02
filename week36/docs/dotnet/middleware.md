[ASP.NET Core Middleware (Microsoft)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0)

# Middleware i ASP.NET Core

## Hva er Middleware?

I ASP.NET Core kan middleware brukes for ulike formål, inkludert, men ikke begrenset til, logging, autentisering, CORS (Cross-Origin Resource Sharing), caching, og feilhåndtering. Middleware er komponenter som behandler inngående HTTP-forespørsler og utgående responser i en ASP.NET Core-applikasjon.

Middleware-komponenter blir utført i den rekkefølgen de er lagt til i pipeline, og de kan utføre handlinger både før og etter at de etterfølgende middleware-komponentene er kjørt.

## Nøkkelpunkter for Middleware

1. **Kjede av Ansvar**: 
   Middleware fungerer som en kjede av ansvar. Hver middleware har muligheten til å utføre noen handlinger, deretter enten kortslutte kjeden og returnere et svar direkte til klienten, eller passere kontrollen til neste middleware i kjeden.

2. **Rekkefølge er Viktig**: 
   Rekkefølgen middleware legges til i pipeline er viktig. En forespørsel vil passere gjennom middleware-komponentene i den rekkefølgen de ble lagt til i `Startup.cs` (eller `Program.cs` i nyere versjoner). Dette betyr at hvis en middleware avslutter responsen tidlig, kan middleware lagt til etter den ikke kjøre.

3. **Request og Response**: 
   Middleware har tilgang til både HTTP-forespørsel og -respons, slik at de kan modifisere begge.

4. **Anvendelser**: 
   Det er mange forskjellige bruksområder for middleware, inkludert feilhåndtering, logging, autentisering, CORS, routing, osv.

## Typiske Middleware Eksempler

1. **Autentisering**: For å sikre API-endepunkter eller webapplikasjoner.
2. **CORS**: For å håndtere CORS-policyer når du kjører frontend og backend separat.
3. **Caching**: For å lagre innhold i cache for å forbedre ytelsen.
4. **Feilhåndtering**: For å håndtere feil på en generell måte.
5. **Ruting**: For å dirigere HTTP-forespørsler til spesifikke kontrollere/metoder.
6. **Rate Limiting**: For å begrense antall forespørsler fra en enkelt kilde.
7. **Request og Response Manipulering**: For å endre innkommende forespørsler eller utgående svar.

## Konfigurering i `Startup.cs`

Her er hvordan du kan legge til noen av disse i `Configure`-metoden i `Startup.cs`:

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    // Logging
    app.UseSerilogRequestLogging(); // Dette forutsetter at du har Serilog konfigurert

    // Feilhåndtering
    app.UseExceptionHandler("/error"); // Generisk feilhåndtering

    // Autentisering
    app.UseAuthentication();

    // Autorisasjon
    app.UseAuthorization();

    // CORS
    app.UseCors(builder =>
    {
        builder.WithOrigins("http://example.com").AllowAnyMethod().AllowAnyHeader();
    });

    // Caching
    app.UseResponseCaching();

    // Rate Limiting (Dette forutsetter en ekstern pakke som `AspNetCoreRateLimit`)
    app.UseIpRateLimiting();

    // Ruting
    app.UseRouting();

    // Endpoint definisjoner
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```
Merk: Rekkefølgen middleware er lagt til i konfigurasjonen er viktig. For eksempel, UseAuthentication og UseAuthorization må komme etter UseRouting, men før UseEndpoints.

### Konfigurering i ConfigureServices
Du må også legge til tjenester i ConfigureServices-metoden hvis middleware krever det. For eksempel:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ... Andre tjenester

    // Caching
    services.AddResponseCaching();

    // Rate Limiting
    services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
    services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
    services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

    // ... Andre tjenester
}
```

### Oppsummering
Dette er en generell oversikt over hvordan middleware fungerer i ASP.NET Core. Middleware er en kraftig mekanisme for å håndtere forskjellige aspekter av HTTP-forespørsler og -responser i en ASP.NET Core-applikasjon. Avhengig av din applikasjons behov, kan du tilpasse og utvide middleware-pipelinen for å implementere egendefinerte funksjoner og forbedre applikasjonens robusthet og sikkerhet.