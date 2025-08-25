# `async` og `await` i C#

## Hva er `async` og `await`?

`async` og `await` er nøkkelord i C# som brukes for å arbeide med asynkrone operasjoner på en enkel og lettforståelig måte. Asynkron programmering lar deg utføre oppgaver som tar tid, som I/O-operasjoner eller nettverkskall, uten å blokkere hovedtråden (vanligvis UI-tråden i applikasjoner). Dette resulterer i en mer responsiv applikasjon som kan håndtere flere oppgaver samtidig.

- **`async`:** Nøkkelordet `async` brukes for å merke en metode som asynkron. Det signaliserer til kompilatoren at metoden inneholder asynkrone operasjoner som skal behandles på en ikke-blokkerende måte.
  
- **`await`:** Nøkkelordet `await` brukes inne i en `async`-merket metode for å pause utførelsen av metoden til den asynkrone operasjonen er fullført. `await` frigjør kontrollen tilbake til den som kaller metoden mens den venter, noe som gjør at andre oppgaver kan kjøre i mellomtiden.

## Hvordan fungerer `async` og `await`?

Når en metode er merket med `async`, kan du bruke `await` for å vente på oppgaver som kjører asynkront. Når kompilatoren ser `await`, deler den metoden i to deler:

1. **Før `await`:** Denne delen av metoden kjører synkront.
2. **Etter `await`:** Denne delen kjører når den asynkrone operasjonen er fullført.

Her er et grunnleggende eksempel:

```csharp
public async Task<string> FetchDataAsync()
{
    // Simulerer en asynkron operasjon som f.eks. en nettverkskall
    await Task.Delay(2000); // Venter 2 sekunder
    return "Data retrieved from server";
}
```

I dette eksempelet blir metoden FetchDataAsync asynkron ved å bruke async. Metoden simulerer en langvarig operasjon med Task.Delay(2000), som venter i 2 sekunder før den returnerer en streng. await sikrer at metoden ikke blokkerer hovedtråden mens den venter.

<div style="page-break-after: always;"></div>

# Bruke async og await i Minimal API Endpoints
Når du lager Minimal API endpoints, kan du bruke async og await for å gjøre kall til eksterne tjenester eller databaser asynkront, noe som forbedrer ytelsen og skalerbarheten til API-et.

## Eksempel på et Minimal API Endpoint
```csharp
var app = WebApplication.CreateBuilder(args).Build();

app.MapGet("/data", async () =>
{
    var data = await FetchDataAsync();
    return Results.Ok(data);
});

app.Run();

public async Task<string> FetchDataAsync()
{
    // Simulerer en asynkron operasjon som f.eks. en nettverkskall
    await Task.Delay(2000); // Venter 2 sekunder
    return "Data retrieved from server";
}
```

I dette eksempelet definerer vi en Minimal API-endpoint som heter /data. Endepunktet kaller en asynkron metode FetchDataAsync ved å bruke await, slik at API-et kan håndtere andre forespørsler mens det venter på at FetchDataAsync skal fullføre.

<div style="page-break-after: always;"></div>

# Bruke async og await i Tjenester
Hvis du bruker Dependency Injection (DI) for å håndtere tjenestene dine, kan du også gjøre metodene asynkrone.

## Eksempel på en Asynkron Tjeneste
```csharp
public interface IDataService
{
    Task<string> GetDataAsync();
}

public class DataService : IDataService
{
    public async Task<string> GetDataAsync()
    {
        // Simulerer en asynkron database eller API-kall
        await Task.Delay(2000);
        return "Data from service";
    }
}

// Registrer tjenesten i DI-containeren
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<IDataService, DataService>();

var app = builder.Build();

app.MapGet("/service-data", async (IDataService dataService) =>
{
    var data = await dataService.GetDataAsync();
    return Results.Ok(data);
});

app.Run();
```

I dette eksempelet har vi en tjeneste DataService som implementerer et asynkront grensesnitt IDataService. Metoden GetDataAsync er asynkron og bruker await til å vente på en simulerte langvarig operasjon. Endepunktet /service-data kaller GetDataAsync asynkront, noe som gjør API-et mer effektivt.

## Viktige punkter å huske

* Unngå async void: Med unntak av asynkrone hendelseshandlere, bør du alltid bruke Task eller Task<T> som returtype for asynkrone metoder.
* Error Handling: Husk å bruke try-catch rundt asynkrone kall for å håndtere unntak på en sikker måte.
* Ytelse: Bruk asynkrone operasjoner for I/O-bundne operasjoner, som nettverkskall eller databaseforespørsler, for å unngå blokkering av tråder.

Ved å bruke async og await kan du skrive mer responsive, skalerbare applikasjoner i C# som effektivt håndterer samtidige operasjoner.

<div style="page-break-after: always;"></div>

# Hvorfor er det viktig å bruke `async` og `await` i en controller eller et endepunkt i et REST API i .NET?

Å bruke `async` og `await` i en controller eller et endepunkt i et REST API i .NET er viktig av flere grunner, spesielt når man arbeider med asynkrone operasjoner som databaser, eksterne API-kall, filoperasjoner, og annen I/O. Her er noen av de viktigste grunnene:

## 1. Unngå blokkering av tråder
Når du bruker `async` og `await`, frigjøres tråden til å håndtere andre forespørsler mens den venter på en ressurs (f.eks. et databasekall eller en API-forespørsel) som kanskje tar tid å returnere. Dette betyr at du kan håndtere flere forespørsler samtidig uten å måtte opprette flere tråder. Dette forbedrer ytelsen og skalerbarheten til applikasjonen.

## 2. Forbedret skalerbarhet
I en webapplikasjon med mange samtidige forespørsler kan bruk av synkrone operasjoner føre til at tråder blir blokkert mens de venter på ressurser, noe som reduserer antallet samtidige forespørsler applikasjonen kan håndtere. Asynkrone operasjoner gjør det mulig for serveren å håndtere mange flere samtidige forespørsler, noe som gjør applikasjonen mer skalerbar.

## 3. Responsivitet
Ved å bruke asynkrone metoder unngår du å låse hovedtråden, noe som gjør at applikasjonen forblir responsiv, selv under tunge belastninger. Dette er spesielt viktig i UI-applikasjoner, men gjelder også for webapplikasjoner hvor rask respons på klientforespørsler er kritisk.

## 4. Forbedret ressursutnyttelse
Asynkrone operasjoner lar applikasjonen utnytte CPU-ressurser bedre. Mens en tråd venter på en I/O-operasjon, kan CPU-en brukes til å kjøre andre oppgaver i mellomtiden.

## 5. Forbedret brukeropplevelse
Ved å bruke `async` og `await` kan applikasjonen håndtere lange operasjoner (som å hente data fra en database) uten å blokkere brukeren. Dette kan føre til bedre brukeropplevelse, siden brukerne ikke opplever forsinkelser eller "henging" i applikasjonen.

## 6. Unngå Deadlocks
I .NET kan bruk av synkrone operasjoner i en asynkron kontekst føre til deadlocks, spesielt i webapplikasjoner som kjører på ASP.NET. Ved å bruke `async` og `await` unngår du denne typen problemer, da asynkron koding sørger for at operasjonene blir riktig utført uten risiko for å låse tråden.

## 7. Best Practices og kodeforståelse
Å bruke `async` og `await` anses som en beste praksis i moderne .NET-applikasjoner. Det gjør koden enklere å forstå, vedlikeholde og mindre utsatt for feil relatert til samtidige operasjoner.

### Eksempel:
```csharp
public async Task<IActionResult> GetData()
{
    var data = await _myService.GetDataAsync();
    return Ok(data);
}
```

