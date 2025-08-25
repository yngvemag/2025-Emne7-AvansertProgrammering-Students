
# Introduksjon til Web API i C# og .NET

## Hva er en Web API?

En **Web API** (Application Programming Interface) er et sett med HTTP-baserte endepunkter som lar klienter (nettlesere, mobilapper, andre servere) kommunisere med applikasjonen din.  
I .NET brukes ofte **ASP.NET Core** for å bygge moderne, skalerbare og plattformuavhengige API-er.

Typiske formål med et Web API:

- Tilby data og funksjonalitet til forskjellige typer klienter.
- Bygge RESTful-tjenester som bruker HTTP-metoder (GET, POST, PUT, DELETE).
- Koble sammen microservices og tredjeparts integrasjoner.

---

## Muligheter i ASP.NET Core Web API

ASP.NET Core gir flere måter å bygge API-er på:

### 1. **Controller-baserte Web API-er**

- Tradisjonell tilnærming med **MVC-stil**.
- Bruker `[ApiController]` og `[Route]`-attributter.
- Støtter model binding, validering og filters.

Eksempel:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IEnumerable<Product> GetAll() => _repository.GetAll();

    [HttpPost]
    public IActionResult Create(Product product)
    {
        _repository.Add(product);
        return CreatedAtAction(nameof(GetAll), product);
    }
}
```

Fordeler:

- Klar struktur, lett å organisere i større applikasjoner.
- Bruker attributter for ruter og validering. 
<div style="page-break-after: always;"></div>

### 2. **Minimal APIs (fra .NET 6)**

- En enklere måte å definere API-endepunkter på uten behov for controllere.
- Passer godt for små tjenester eller microservices.

Eksempel:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/products", () => new[] { "Apple", "Banana" });
app.MapPost("/products", (Product p) => Results.Created($"/products/{p.Id}", p));

app.Run();
```

Fordeler med Minimal APIs:

- Mindre boilerplate-kode.
- Rask å sette opp, godt egnet for små prosjekter.
- Lett å kombinere med DI og middleware.

### 3. **gRPC**

- Høyytelses RPC-rammeverk som bruker HTTP/2 og Protocol Buffers.
- Passer godt for intern kommunikasjon mellom mikrotjenester.
- Mindre egnet for rene REST-klienter, men svært effektivt for system-til-system kommunikasjon.

### 4. **SignalR**

- Rammeverk for sanntids-kommunikasjon over WebSockets.
- Perfekt for applikasjoner som trenger push-varsler, live chat, dashboards osv.

### 5. **OpenAPI/Swagger-dokumentasjon**

- ASP.NET Core kan automatisk generere OpenAPI-spesifikasjon (Swagger) for API-dokumentasjon og testing.
- Bruk Swashbuckle eller Scalar for å få en interaktiv dokumentasjon.

---

## Funksjoner ASP.NET Core Web API støtter

- **Dependency Injection** innebygd.
- **Middleware pipeline** for logging, autentisering, feilhåndtering osv.
- **Model binding og validering** med `DataAnnotations`.
- **Autentisering og autorisasjon** (JWT, OAuth2, Identity).
- **Resilience & Caching** (Polly-integrasjon, output caching).
- **Versjonering av API-er** (Microsoft.AspNetCore.Mvc.Versioning-pakken).

---

## FastEndpoints – et alternativ

Et annet rammeverk som blir mer populært er **[FastEndpoints](https://fast-endpoints.com)**:

- Gir et alternativ til controller og minimal APIs med en litt annen arkitektur.
- Høy ytelse og tydelig struktur.
- Definerer endepunkter som egne klasser i stedet for attributter på kontrolleren.

Eksempel:

```csharp
public class GetProductsEndpoint : EndpointWithoutRequest<IEnumerable<Product>>
{
    public override void Configure()
    {
        Get("/products");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendAsync(new[] { new Product("Apple"), new Product("Banana") }, cancellation: ct);
    }
}
```

FastEndpoints kan være et godt valg dersom du ønsker høy ytelse og tydelig separasjon av ansvar.

---

## Andre måter å kommunisere med API-er

I tillegg til REST-baserte API-er kan man bruke andre protokoller og standarder:

- **GraphQL**: Klienter spesifiserer hvilke data de ønsker, og serveren returnerer nøyaktig dette. Gir fleksibilitet og kan redusere antall kall.
- **SOAP**: Eldre protokoll basert på XML. Brukes fortsatt i noen virksomheter med etablerte systemer.
- **OData**: Bygger på REST, men gir innebygd spørringsfunksjonalitet via URL-parametere.
- **gRPC**: Som nevnt, effektiv binær kommunikasjon over HTTP/2.

Disse teknologiene kan kombineres avhengig av behov og krav til ytelse, standardisering og integrasjon.

---

## Oppsummering

- **Web API i .NET** gir fleksibilitet til å bygge moderne tjenester.
- Du kan velge **Controller-basert API**, **Minimal APIs**, **gRPC**, **SignalR**, eller **FastEndpoints**.
- Andre alternativer inkluderer **GraphQL**, **SOAP**, og **OData**.
- Valget avhenger av prosjektets behov, kompleksitet og ytelseskrav.

ASP.NET Core gir deg alle mulighetene for å bygge både enkle og komplekse API-løsninger på en strukturert måte.
