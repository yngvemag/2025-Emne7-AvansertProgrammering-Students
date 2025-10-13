# ASP.NET Core Middleware vs. Filter Pipeline

## Introduksjon

I ASP.NET Core finnes det to viktige konsepter for å kontrollere flyten i en webapplikasjon:

- **Middleware pipeline**
- **Filter pipeline**

De henger sammen, men løser forskjellige problemer og lever på ulike nivåer i request-prosessen.  
Denne guiden gir en tydelig forklaring på hvordan de fungerer, hvordan de forholder seg til hverandre, og når du bør bruke hva.

---

## Middleware Pipeline

### Hva er middleware?

- Middleware er *komponenter* som settes opp i en **sekvensiell pipeline** i `Program.cs`.
- Hver middleware kan:
  - Motta en HTTP-request.
  - Behandle requesten (f.eks. logging, autentisering, CORS).
  - Eventuelt sende requesten videre til neste middleware i kjeden.
  - Bearbeide responsen før den sendes tilbake til klienten.

### Eksempel på middleware i `Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Viktige kjennetegn

- Middleware er **globalt** og gjelder for *alle requests*.
- Kalles **før** MVC/controller-koden.
- Godt egnet til tverrgående bekymringer (cross-cutting concerns) som logging, autentisering, rate limiting og feilbehandling.

---

## Filter Pipeline

### Hva er filter pipeline?

Når en request først har passert middleware og havner i MVC-delen (`MapControllers()`), da aktiveres **filter pipeline**.  
Filter pipeline er en mekanisme i **ASP.NET Core MVC** for å kjøre kode **før og etter** en controller-action.

### Typer filter

- **Authorization filters** – kjører tidlig, bestemmer om brukeren har lov til å kjøre action (f.eks. `[Authorize]`, `[ApiKey]`).
- **Resource filters** – kan stoppe kjeden tidlig eller short-circuite (f.eks. caching).
- **Action filters** – kjører kode før og etter en controller-action (f.eks. logging, validering).
- **Exception filters** – håndterer unntak globalt på controller-nivå.
- **Result filters** – kjøres når resultatet (`IActionResult`) genereres og før det sendes til klient.

### Eksempel: action filter

```csharp
public class LoggingActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        Console.WriteLine("Før action");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        Console.WriteLine("Etter action");
    }
}
```

### Viktige kjennetegn

- Filter pipeline kjører **etter middleware pipeline**, men **før og etter controller-actions**.
- Fungerer på **MVC-nivå** (dvs. API- og MVC-kontrollere).
- Kan brukes globalt, per-controller eller per-action.

---

## Sammenhengen mellom Middleware og Filters

En typisk request flyt ser slik ut:

```
Client
   ↓
[Middleware pipeline]   -> Logging, Auth, Routing, etc.
   ↓
[Filter pipeline]       -> Authorization, Action, Exception, Result filters
   ↓
Controller Action       -> Din metode (f.eks. GetProducts)
   ↓
[Filter pipeline]       -> Result filters
   ↓
[Middleware pipeline]   -> Eventuell etter-behandling (f.eks. response logging)
   ↓
Client
```

- **Middleware** → jobber på et *lavt nivå* med HTTP-request/response.  
- **Filters** → jobber på et *høyere nivå*, tett integrert med MVC og controller actions.

---

## Når bør du bruke hva?

- **Middleware**:
  - Når du trenger global logikk som gjelder alle requests (uavhengig av MVC eller ikke).
  - Eksempler: autentisering, CORS, logging, rate limiting, global error handling.

- **Filters**:
  - Når du trenger logikk tett knyttet til controller actions.
  - Eksempler: autorisasjon på controller-nivå, validering, caching, result-transformasjon.

---

## Oppsummering

- **Middleware pipeline**: lavnivå, global, jobber med HTTP-request/response før MVC.  
- **Filter pipeline**: høynivå, knyttet til MVC, kjører før/etter controller-actions.  
- Sammen utgjør de et kraftig verktøy for å kontrollere flyten i en ASP.NET Core-applikasjon.

---
