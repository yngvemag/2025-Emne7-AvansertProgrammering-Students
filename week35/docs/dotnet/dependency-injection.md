# Dependency Injection i C# ASP.NET Core

## Hva er Dependency Injection?

**Dependency Injection (DI)** er et designmønster som brukes for å implementere Inversion of Control (IoC), hvor ansvaret for å lage instanser av objekter overføres fra klassen selv til en ekstern enhet som kalles en container eller injector. Dette hjelper med å redusere avhengigheter (dependencies) mellom klasser, gjøre koden mer testbar, og oppnå en løsere kobling mellom komponenter.

## Hvordan fungerer Dependency Injection?

Når en klasse trenger en avhengighet, som regel et annet objekt, definerer den disse avhengighetene som parametere i konstruktøren sin. Istedenfor at klassen selv oppretter disse objektene, vil en DI-container, som ASP.NET Core sitt innebygde DI-system, automatisk injisere de nødvendige objektene når klassen opprettes.

## Eksempel på Dependency Injection

Tenk deg at du har en tjeneste `IEmailService` som skal sendes til en `NotificationService`. Uten DI, måtte `NotificationService` selv opprettet en instans av `IEmailService`, men med DI ser koden slik ut:

```csharp
public interface IEmailService
{
    void SendEmail(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    public void SendEmail(string to, string subject, string body)
    {
        // Implementasjon for å sende e-post
    }
}

public class NotificationService
{
    private readonly IEmailService _emailService;

    public NotificationService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public void Notify(string message)
    {
        // Bruk _emailService til å sende en e-post
        _emailService.SendEmail("recipient@example.com", "Notification", message);
    }
}
```
Her ser vi at NotificationService tar en IEmailService som en avhengighet gjennom konstruktøren. Når NotificationService blir opprettet av DI-containeren, vil en instans av EmailService (eller en annen implementasjon av IEmailService) bli injisert automatisk.

## Registrering av tjenester i DI-containeren
I ASP.NET Core må du registrere tjenester i DI-containeren før de kan injiseres. Dette gjøres i Startup.cs-filen, i ConfigureServices-metoden.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Registrer EmailService som implementasjon av IEmailService
    services.AddTransient<IEmailService, EmailService>();
    
    // Registrer NotificationService
    services.AddTransient<NotificationService>();
}

```

### Metoder for å registrere tjenester
ASP.NET Core gir tre hovedmåter å registrere tjenester på, avhengig av hvor lenge de skal leve:

#### Transient: 
Tjenester registrert som transient opprettes hver gang de blir etterspurt. Dette passer for lettvektsstateless tjenester.
```csharp
services.AddTransient<IEmailService, EmailService>();
```
#### Scoped: 
Tjenester registrert som scoped opprettes én gang per HTTP-forespørsel. Dette er nyttig for tjenester som bør ha en viss tilstand innenfor en enkelt HTTP-forespørsel.

```csharp
services.AddScoped<IEmailService, EmailService>();
```
#### Singleton: 
Tjenester registrert som singleton opprettes én gang og gjenbrukes gjennom hele applikasjonens levetid. Dette er nyttig for delte ressurser som konfigurasjoner.

```csharp
services.AddSingleton<IEmailService, EmailService>();
```

### Hvordan bruke injiserte tjenester
Når en tjeneste er registrert i DI-containeren, kan den injiseres i en controller eller annen tjeneste på følgende måte:

```csharp

public class HomeController : Controller
{
    private readonly NotificationService _notificationService;

    public HomeController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public IActionResult Index()
    {
        _notificationService.Notify("Welcome to our website!");
        return View();
    }
}
```
Her vil NotificationService automatisk bli injisert i HomeController via konstruktøren.

Oppsummering
Dependency Injection er en kjernefunksjon i ASP.NET Core som bidrar til å gjøre applikasjonene dine mer modulære, vedlikeholdbare og testbare. Ved å registrere tjenestene dine i DI-containeren, kan du enkelt kontrollere hvordan og når objekter opprettes og deles innenfor applikasjonen.