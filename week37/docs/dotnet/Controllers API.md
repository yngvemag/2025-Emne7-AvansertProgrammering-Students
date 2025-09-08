# ASP.NET Core Minimal APIs

ASP.NET Core har introdusert Minimal APIs i versjon 6 som en ny måte å bygge HTTP APIer på med et fokus på minimal overhead og seremoni. Dette betyr raskere oppstart og enklere kode for mange scenarier. La oss se på forskjellene mellom controller-baserte APIer og Minimal APIs:

| Kriterium                | Controller-basert API                                      | Minimal API                                                        |
|--------------------------|------------------------------------------------------------|---------------------------------------------------------------------|
| **Syntaks og Seremoni**   | MVC-mønster med controller-klasser og attributter.          | Definer endepunkter med få linjer kode direkte i Program.cs.         |
| **Startup-konfigurasjon** | Konfigurer i Startup.cs.                                   | Alt settes opp i Program.cs.                                        |
| **Mellomvare**            | Legges til i Configure i Startup.cs.                       | Flytende syntaks med funksjoner som MapGet etter mellomvare.         |
| **Innbygget funksjonalitet** | MVC-funksjoner som model binding og validering.            | Noen funksjoner kan kreve ekstra arbeid.                            |
| **Utvidelighet**          | Klare steder for egendefinert funksjonalitet.               | Potensielt mer utfordrende pga. kondensert natur.                   |
| **Bruksområde**           | Større, mer komplekse APIer.                               | Små til mellomstore APIer eller rask prototyping.                   |

---

## Controller

En controller i ASP.NET Core er i utgangspunktet en klasse som håndterer HTTP-forespørsler. Den fungerer som en mellommann mellom modellen (som representerer data) og visningen (som representerer UI eller output format, for eksempel JSON for et REST API).

### Steg for å lage en grunnleggende controller:

1. **Opprett en ny Controller:**
   - Tradisjonelt plasseres controllers i en "Controllers" mappe i rotmappen av prosjektet ditt.
   - En controller-klasse bør ende med "Controller" i navnet, for eksempel `ProductsController`.

2. **Arv fra Controller Base Class:**
   - For et API, vil din controller ofte arve fra `ControllerBase` klassen.
   - For en MVC-applikasjon med visninger, vil du arve fra `Controller` klassen.

3. **Definer Ruter:**
   - Bruk attributter som `[Route]` for å definere ruten til din controller og dens handlinger.

4. **Legg til Handlinger:**
   - En handling er i utgangspunktet en metode i controlleren som håndterer en bestemt HTTP-forespørsel (GET, POST, PUT, DELETE, etc.).
   - Bruk attributter som `[HttpGet]`, `[HttpPost]`, etc., for å definere hvilken type HTTP-forespørsel hver handling håndterer.

5. **Returner Data:**
   - I et API, vil handlingene ofte returnere data som JSON ved hjelp av metoder som `Ok()` eller `NotFound()`.

Her er et enkelt eksempel på en controller for et produkt-API:

```csharp
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private static List<string> products = new List<string>
    {
        "Product A",
        "Product B",
        "Product C"
    };

    // GET /products
    [HttpGet]
    public ActionResult<IEnumerable<string>> Get()
    {
        return Ok(products);
    }

    // GET /products/1
    [HttpGet("{id}")]
    public ActionResult<string> Get(int id)
    {
        if (id < 0 || id >= products.Count)
        {
            return NotFound();
        }

        return Ok(products[id]);
    }
}
````
<div style="page-break-after: always;"></div>

### **Eksempler på ruter:**
For Get metoden som returnerer alle produkter:

* [HttpGet] uten spesifikke ruteinformasjon vil følge standard rutingskonvensjonen satt av controllerens [Route("[controller]")] attributt.
  * URL: /products
    * For Get metoden som returnerer et produkt basert på dets ID:

* [HttpGet("{id}")] betyr at en ID-parameter er forventet i ruten.
  * URL: /products/{id}
    * Her erstatter {id} en faktisk verdi, f.eks. /products/1 for det første produktet. 
    * 

Så, for ProductsController, vil de respektive ressurs-URLene være:
* Hente alle produkter: GET /products
* Hente et spesifikt produkt basert på ID: GET /products/{id}

Merk at [Route("[controller]")] bruker en "token" [controller] som automatisk erstattes med controllerens navn minus "Controller"-delen. Så, for ProductsController, blir det products (alltid i små bokstaver).

<div style="page-break-after: always;"></div>

## Tokens i ASP.NET Core
I ASP.NET Core bruker man slike "placeholders" (ofte referert til som "tokens") i ruteattributter for å dynamisk generere rute-URLer. Her er en tabell over de mest brukte tokens med deres betydning:

| Token        | Beskrivelse                                                                                                           |
|--------------|-----------------------------------------------------------------------------------------------------------------------|
| [controller] | Navnet på controlleren, minus "Controller" suffikset. For eksempel, for `UsersController`, ville dette bli `Users`.    |
| [action]     | Navnet på action-metoden som blir kalt.                                                                               |
| [area]       | Områdenavnet, hvis du bruker arealbasert routing.                                                                     |
| {id}         | Et plassholdersegment som kan fange opp en verdiparameter kalt `id`.                                                   |
| {...}        | En annen valgfri parameter i ruten, f.eks. `{name}`, ville fange opp en verdiparameter kalt `name`.                    |


Eksempler:
1. [Route("[controller]/[action]")]:
   * For UsersController og en metode kalt GetUser, vil URL være /Users/GetUser.
2. [Route("[controller]/[action]/{id?}")]:
   * For UsersController og en metode kalt GetUser, kan man ha en URL som /Users/GetUser/5, hvor 5 er en optional id.
3. [Route("[area]/[controller]/[action]")]:
   * Hvis du har en Admin-area, en UsersController, og en metode kalt ManageUsers, ville URL være /Admin/Users/ManageUsers.
Det er viktig å merke seg at du kan kombinere disse tokens i en rekke forskjellige måter for å lage komplekse og tilpassede ruter for ditt API eller webapplikasjon.

I tillegg, mens disse tokens gir en fin måte å dynamisk definere ruter på, anbefales det noen ganger å eksplisitt definere ruter for klarhet, spesielt for API-er hvor klare og konsistente endepunkter er avgjørende.