# Lagdelt Arkitektur og moderne API-arkitekturmønstre

Denne guiden gir en **grundig innføring** i lagdelt arkitektur, hvorfor vi bruker det, hvordan det fungerer i en **ASP.NET Core Web API**, og hvordan det sammenlignes med **Clean Architecture** og **Vertical Slicing**.  

Målet er å gi deg en dyp forståelse slik at du kan velge riktig arkitekturmønster for dine prosjekter.

---

## 1. Hva er lagdelt arkitektur?

**Lagdelt arkitektur** (også kalt **n-tier architecture**) er et av de mest brukte arkitekturmønstrene for bedriftsapplikasjoner.  
Hovedideen er å **dele programvaren inn i lag** med klare ansvarsområder. Hvert lag kommuniserer kun med laget rett under seg.

Dette gir:

- **Separasjon av bekymringer (SoC)** – lettere å vedlikeholde og forstå.
- **Modularitet** – endringer i ett lag påvirker ikke andre lag.
- **Skalerbarhet** – man kan skalere deler av systemet uavhengig.
- **Enklere testing** – mocke lag isolert.

En klassisk .NET Web API har ofte følgende lag:

| Lag | Beskrivelse | Typiske komponenter |
|-----|-------------|----------------------|
| **Presentasjonslag (Presentation Layer / API)** | Håndterer kommunikasjon med klienter (API-endepunkter, UI). | Controllers, API Endpoints |
| **Forretningslogikklag (Business Logic Layer / Service Layer)** | Inneholder all forretningslogikk og regler. | Services, Domain Entities |
| **Dataaksesslag (Data Access Layer)** | Ansvarlig for interaksjon med databasen. | Repository-klasser, EF DbContext |
| **Datalag (Data Layer)** | Selve datakilden. | MySQL, SQL Server, etc. |

---
<div style="page-break-after: always;"></div>

## 2. Hvordan lagene kommuniserer

For å sikre tydelig ansvar er det viktig at **hver komponent kun kommuniserer med laget rett under seg**.

Eksempel med en REST API i ASP.NET Core:

```
Client → Controller → Service → Repository → Database
```

**Forklaring:**

1. **Controller** mottar HTTP-forespørsel og sender den videre til et **Service-lag**.
2. **Service-laget** implementerer forretningslogikken og validering.
3. **Repository-laget** håndterer CRUD-operasjoner mot databasen (f.eks. med Entity Framework).
4. **Database** lagrer og henter data.

---

## 3. Fordeler og ulemper

### Fordeler

- **Skalerbarhet:** Lagene kan skaleres uavhengig, f.eks. flere API-servere uten å endre database.
- **Vedlikehold:** Mindre risiko ved endringer, da hvert lag er isolert.
- **Teamarbeid:** Ulike team kan jobbe på ulike lag samtidig.
- **Gjenbruk:** F.eks. forretningslogikk kan brukes av både Web API og mobilapp.

### Ulemper

- **Kompleksitet:** Små prosjekter kan bli overkompliserte.
- **Ytelsesoverhead:** Flere lag kan gjøre applikasjonen tregere hvis det ikke designes riktig.
- **Over-engineering:** For mange abstraksjoner (f.eks. overflødige repositories) kan gjøre koden unødvendig kompleks.

---
<div style="page-break-after: always;"></div>

## 4. Lagene i detalj

### 4.1 Presentasjonslag

- API-endepunkter, webgrensesnitt eller mobilklient.
- **I ASP.NET Core:** Controllers eller Minimal APIs.
- Ansvar: Motta input, returnere output.
- Skal være **tynt** – ingen forretningslogikk her.

Eksempel på Controller:

```csharp
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await _service.GetAllAsync();
        return Ok(students);
    }
}
```

---

### 4.2 Forretningslogikklag (Service Layer)

- Inneholder **regler og logikk** for applikasjonen.
- Skal **ikke** vite noe om API eller database, kun domenet.
- Bruk **interfaces** for å holde lagene løst koblet.

Eksempel på Service:

```csharp
public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetAllAsync();
}

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;

    public StudentService(IStudentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<StudentDto>> GetAllAsync()
    {
        var students = await _repository.GetAllAsync();
        return students.Select(s => new StudentDto(s.Id, s.FirstName, s.LastName));
    }
}
```

---

### 4.3 Dataaksesslag (Repository Layer)

- Håndterer **CRUD** og annen interaksjon med databasen.
- Implementeres ofte med **Entity Framework Core**.
- Gir et abstraksjonslag slik at service-laget ikke avhenger direkte av EF.

Eksempel på Repository:

```csharp
public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllAsync();
}

public class StudentRepository : IStudentRepository
{
    private readonly SchoolContext _context;

    public StudentRepository(SchoolContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students.AsNoTracking().ToListAsync();
    }
}
```

---
<div style="page-break-after: always;"></div>

## 5. Clean Architecture vs Lagdelt Arkitektur

**Clean Architecture** bygger videre på lagdelt arkitektur, men med et **sterkere fokus på avhengighetsregler**.

| Lagdelt Arkitektur | Clean Architecture |
|--------------------|--------------------|
| Avhengigheter går ofte begge veier. | Avhengigheter går **kun innover** (mot domenet). |
| Service-laget kan avhenge av datalaget. | Datalaget avhenger av domenet, **ikke omvendt**. |
| Klassisk struktur for Web API. | Mer avansert, testbar og fleksibel. |
| Raskere å komme i gang. | Mer oppsett, men lettere å vedlikeholde på sikt. |

**Hovedprinsipp i Clean Architecture:**

- Domenet er **helt isolert** fra infrastruktur og eksterne teknologier.
- API, databaser, UI er "ytterste lag" som kan byttes uten å påvirke domenet.

---

## 6. Vertical Slicing

I stedet for å strukturere koden etter lag, kan man strukturere etter **features**.

Eksempel:

```
/Features
   /Students
      GetStudents.cs
      CreateStudent.cs
      UpdateStudent.cs
   /Courses
      GetCourses.cs
      CreateCourse.cs
```

**Fordeler:**

- Alt for en funksjon ligger samlet – lettere å finne frem.
- Godt egnet for **CQRS** og **MediatR**.

**Ulemper:**

- Kan være uvant for team som er vant til lagdelt arkitektur.
- Mindre gjenbruk mellom features hvis ikke designes riktig.

---
<div style="page-break-after: always;"></div>

## 7. Hva bør du velge?

| Scenario | Anbefalt mønster |
|----------|------------------|
| Lite/middels prosjekt, teamet er nytt på arkitektur | **Lagdelt arkitektur** |
| Stort prosjekt med komplekse regler og lang levetid | **Clean Architecture** |
| Mikroservices eller hendelsesdrevne systemer | **Vertical Slicing + CQRS** |

Ofte starter man med **lagdelt arkitektur**, og etter hvert når domenet vokser, kan man migrere mot **Clean Architecture**.

---

## 8. Best Practices

1. **Hold Controllerene tynne.** Ingen forretningslogikk der.
2. **Bruk interfaces** mellom lag for løse koblinger.
3. **Dependency Injection (DI)** er standard i ASP.NET Core.
4. **DTO-er for API-respons** – ikke returner EF-entiteter direkte.
5. **AsNoTracking() for lesing** – bedre ytelse når data kun skal leses.
6. **Separate mapper for hvert lag:**  

   ```
   /API (controllers)
   /Application (services, DTOs)
   /Infrastructure (repositories, EF)
   /Domain (entities, business rules)
   ```

7. **Unit testing:** mock repository og test service-laget isolert.

---
<div style="page-break-after: always;"></div>

## 9. Arkitektureksempel for ASP.NET Core Web API

```
MyProject
│
├── API
│   ├── Controllers
│   └── Program.cs
│
├── Application
│   ├── Services
│   ├── Interfaces
│   └── DTOs
│
├── Domain
│   └── Entities
│
└── Infrastructure
    ├── Data
    └── Repositories
```

Dette oppsettet kombinerer prinsippene fra **lagdelt arkitektur** og **Clean Architecture**.

---

## 10. Oppsummering

- **Lagdelt arkitektur:** Tydelig separasjon, lett å forstå, perfekt for å starte med.
- **Clean Architecture:** Mer struktur, bedre for komplekse prosjekter.
- **Vertical Slicing:** Strukturert etter features, fleksibelt for moderne mikrotjenester.

> Start enkelt med lagdelt arkitektur, og flytt deg gradvis mot Clean Architecture når applikasjonen vokser.

---
