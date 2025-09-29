# Paginering

Paginering er en viktig del av mange webapplikasjoner, spesielt når store datasett er involvert. I stedet for å hente alle postene fra databasen og filtrere dem i programmet, kan paginering på databasesiden gi betydelig bedre ytelse. Det handler om å hente data i små porsjoner (sider) i stedet for å hente alt på én gang. Dette gir bedre ressursbruk og hastighet, spesielt i applikasjoner som håndterer store datamengder.

## Hvorfor paginering er viktig:
- **Ytelse**: Forbedrer responstiden ved å hente kun den nødvendige mengden data.
- **Skalerbarhet**: Lar applikasjonen håndtere store datasett effektivt uten å bruke for mye minne.
- **Brukervennlighet**: Forbedrer brukeropplevelsen ved å vise data i håndterbare porsjoner.

## Steg for å implementere paginering med Entity Framework

### 1. Oppdater Query

For å legge til paginering, bruker du `Skip()` og `Take()`-metoder fra LINQ. `Skip()` hopper over et bestemt antall elementer, mens `Take()` henter et angitt antall elementer etter `Skip()`. Her er et eksempel på hvordan du kan gjøre det:

```csharp
int pageNumber = 2;  // Eksempel på sidetall
int pageSize = 10;   // Antall poster per side

var paginatedData = dbContext.YourEntity
    .OrderBy(x => x.Id)          // Viktig å sortere data for paginering
    .Skip((pageNumber - 1) * pageSize)  // Hopp over de første elementene
    .Take(pageSize)              // Hent det ønskede antall elementer
    .ToList();
```
<div style="page-break-after: always;"></div>

### 2. Lag en pagineringsmodell

For å sende informasjon om paginering til klienten, kan det være nyttig å lage en modell som inneholder informasjon som totalt antall sider, gjeldende side, antall elementer per side, og om det er tidligere/neste side tilgjengelig.

Eksempel på en pagineringsmodell:
```csharp	
public class PagedResult<T>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public List<T> Items { get; set; }
}

```
Denne modellen kan brukes til å sende både data og tilhørende pagineringsinformasjon til klienten.

### 3. Implementer pagineringslogikken

Når du har opprettet pagineringsmodellen, kan du implementere selve pagineringen i koden din. Her er et eksempel:
```csharp
public PagedResult<YourEntity> GetPaginatedData(int pageNumber, int pageSize)
{
    var totalCount = dbContext.YourEntity.Count();
    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

    var data = dbContext.YourEntity
        .OrderBy(x => x.Id)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    return new PagedResult<YourEntity>
    {
        CurrentPage = pageNumber,
        TotalPages = totalPages,
        PageSize = pageSize,
        TotalCount = totalCount,
        Items = data
    };
}

```
Denne logikken henter den nødvendige siden med data basert på pageNumber og pageSize, beregner totalt antall sider, og returnerer alt som en PagedResult-modell.

### 4. Klientsiden

På klientsiden, basert på responsen du får (spesielt HasPrevious og HasNext), kan du aktivere eller deaktivere "Neste" og "Forrige" knappene for paginering.

Tips:

1. Validering: Sørg alltid for å validere pagineringsparametrene som pageNumber og pageSize for å unngå ugyldige eller ekstreme verdier som kan føre til feil eller ytelsesproblemer.

2. Maksimal sidestørrelse: For å forhindre for store forespørsler, bør du ha en maksimal sidestørrelse, som for eksempel 100 elementer per side.

3. Ytelse: Hvis datasettet er stort, kan det bli dyrt å bruke Count()-operasjoner hver gang. I slike tilfeller kan du vurdere å bruke cachingstrategier eller asynkrone metoder for å forbedre ytelsen.

4. Metadata: Hvis du ikke vil sende metadata som totalt antall sider med hver forespørsel, kan du vurdere å sende denne metadataen gjennom HTTP-headere.

Denne oversikten dekker pagineringens grunnleggende konsepter og gir en trinnvis tilnærming til hvordan du kan implementere paginering på server- og klientsiden.