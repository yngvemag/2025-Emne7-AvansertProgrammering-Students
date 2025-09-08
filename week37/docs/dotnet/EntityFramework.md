# Entity Framework Core – Komplett oversikt for MySQL (ASP.NET Core / .NET 9)

## 1) Hva er Entity Framework Core, og hvorfor bruke det?

**Entity Framework Core (EF Core)** er en **Object–Relational Mapper (ORM)** for .NET som lar deg jobbe med databasen via **C#‑klasser** og **LINQ** i stedet for rå SQL. EF Core oversetter spørringer og endringer til SQL for valgt databaseprovider (her: MySQL).

**Fordeler:**

- **Produktivitet:** Mindre boilerplate, færre SQL‑strenger.
- **Type-sikkerhet:** LINQ‑spørringer valideres ved kompilering.
- **Migrasjoner:** Kontrollerte schema-endringer over tid.
- **Portabilitet:** Støtter flere databaser; bytt provider ved behov.
- **Testbarhet:** Alternative providere (f.eks. SQLite) i tester.
- **Vedlikehold:** Strukturert data‑lag, mindre duplisering.

**Når bruke rå SQL?**

- Avanserte/ytelseskritiske spørringer som er tunge i LINQ.
- Rapportering, bulk‑operasjoner. (EF støtter `FromSqlRaw` m.m.)

---

## 2) Teknisk oversikt (MySQL + EF Core)

- **Provider (anbefalt):** `Pomelo.EntityFrameworkCore.MySql` (åpen kildekode, bredt brukt).
- **DbContext:** Håndterer tilkobling, sporing og mapping.
- **DbSet<TEntity>:** Representerer tabeller.
- **Konfigurasjon:** Konvensjoner, **Data Annotations** (attributter) eller **Fluent API** i `OnModelCreating`.
- **Relasjoner:** 1–1, 1–mange, mange–mange (implicit join‑entity støttes).
- **Lasting:** Eager (`Include`), Explicit (`Entry.Load`), Lazy (bruk varsomt i Web API).

---

## 3) Installasjon og oppsett for MySQL

### 3.1 Pakker

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet tool install --global dotnet-ef
```

<div style="page-break-after: always;"></div>

### 3.2 Connection string (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=school_db;User=root;Password=your_password;TreatTinyAsBoolean=true;"
  }
}
```

> Tips: Bruk **Secret Manager** i utvikling i stedet for å lagre passord i repo:  
> `dotnet user-secrets init` og  
> `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;..."`

### 3.3 DbContext-registrering (`Program.cs`, minimal API)

```csharp
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;

var builder = WebApplication.CreateBuilder(args);

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseMySql(connStr, ServerVersion.AutoDetect(connStr),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    )
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
    .EnableDetailedErrors(builder.Environment.IsDevelopment())
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "EF Core + MySQL ready");

app.Run();
```

---
<div style="page-break-after: always;"></div>

## 4) Domenemodeller (med **Data Annotations**) 

> Her viser vi **Data Annotations** (attributter) som alternativ/tillegg til Fluent API.

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Student
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = default!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = default!;

    [Column(TypeName = "date")]
    public DateTime EnrollmentDate { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

public class Course
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = default!;

    [Range(0, 60)]
    public int Credits { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

public class Enrollment
{
    [ForeignKey(nameof(Student))]
    public int StudentId { get; set; }
    public Student Student { get; set; } = default!;

    [ForeignKey(nameof(Course))]
    public int CourseId { get; set; }
    public Course Course { get; set; } = default!;

    [Column(TypeName = "datetime(6)")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

---

## 5) DbContext med **Fluent API** 

```csharp
using Microsoft.EntityFrameworkCore;

public class SchoolContext : DbContext
{
    public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Composite key for join entity (many-to-many)
        modelBuilder.Entity<Enrollment>()
            .HasKey(e => new { e.StudentId, e.CourseId });

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index example
        modelBuilder.Entity<Course>()
            .HasIndex(c => c.Title)
            .HasDatabaseName("IX_Course_Title");
    }
}
```

**Når velge Data Annotations vs Fluent API?**

- **Data Annotations:** Enkle regler tett på modellen (Required, MaxLength, Column).
- **Fluent API:** Avansert mapping (sammensatte nøkler, indekser, relasjonsregler).  
Ofte kombineres de – men unngå motstridende konfigurasjon.

---
<div style="page-break-after: always;"></div>

## 6) Code‑First med migrasjoner (MySQL)

**Flyt:**

1. Lag modeller og `DbContext` (som over).
2. Opprett migrasjon og oppdater databasen.

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Vanlige scenarioer:**

```bash
# Endre skjema og oppdatere
dotnet ef migrations add AddStudentIndexes
dotnet ef database update

# Generer idempotent SQL for deploy
dotnet ef migrations script -o deploy.sql

# Se generert SQL (i kode): db.Students.Where(...).ToQueryString()
```

> **Obs:** MySQL‑typer (lengder, `datetime(6)`, `decimal(p,s)`) kan være viktige for presisjon/ytelse.

---
<div style="page-break-after: always;"></div>

## 7) Database‑First (reverse engineering) med MySQL

**Forutsetning:** Eksisterende MySQL‑database/tabeller.

**Kommandolinjeeksempler (Pomelo):**

```bash
# Scaffold hele databasen til valgt namespace og mappe
dotnet ef dbcontext scaffold "Server=localhost;Port=3306;Database=school_db;User=root;Password=your_password" \
Pomelo.EntityFrameworkCore.MySql \
--output-dir Data/Models \
--context-dir Data \
--context SchoolContext \
--namespace YourApp.Data.Models \
--context-namespace YourApp.Data \
--use-database-names \
--no-onconfiguring
```

**Scaffold utvalgte tabeller:**

```bash
dotnet ef dbcontext scaffold "<connstring>" Pomelo.EntityFrameworkCore.MySql \
--table students --table courses --table enrollments \
--output-dir Data/Models --context-dir Data --context SchoolContext \
--namespace YourApp.Data.Models --context-namespace YourApp.Data \
--use-database-names --no-onconfiguring
```

**Tips:**

- Bruk `--use-database-names` om du vil beholde eksakte MySQL‑navn (inkl. case/underscore).
- Sett `--no-onconfiguring` når du konfigurerer context via DI i `Program.cs`.
- Etter scaffolding kan du **flytte konfigurasjon** til separate `IEntityTypeConfiguration<>`‑klasser for bedre struktur.

---
<div style="page-break-after: always;"></div>

## 8) Querying, projektering og lasting

```csharp
// Read-only with projection and AsNoTracking
var recent = await db.Enrollments
    .Where(e => e.CreatedAt >= DateTime.UtcNow.AddMonths(-1))
    .Select(e => new
    {
        Student = e.Student.FirstName + " " + e.Student.LastName,
        Course = e.Course.Title,
        e.CreatedAt
    })
    .AsNoTracking()
    .ToListAsync();
```

```csharp
// Eager loading
var student = await db.Students
    .Include(s => s.Enrollments)
        .ThenInclude(e => e.Course)
    .AsNoTracking()
    .FirstOrDefaultAsync(s => s.Id == id);
```

```csharp
// Update entity
var course = await db.Courses.FindAsync(id);
if (course is null) return Results.NotFound();
course.Title = "Object-Oriented Programming";
await db.SaveChangesAsync();
```

---
<div style="page-break-after: always;"></div>

## 9) Best practices (MySQL + Web API)

### Ytelse

- Bruk `AsNoTracking()` for lesing. 
  - Forteller EF Core ikke å spore entitetene.
  - Ingen Change Tracking, så EF Core bruker mindre minne og gir bedre ytelse.
  - Resultatet er read-only objekter – hvis du prøver å oppdatere dem, skjer ingenting før du manuelt legger dem til i sporing igjen
  - Bruk det når:
    - Du bare skal lese data (read-only spørringer).
    - Du lager API-er som bare returnerer data til klienten.
    - Du jobber med store dataset der sporing blir dyrt.
    - Du vil projisere til DTO-er (Data Transfer Objects).
- Projiser til lette DTO‑er (`Select`) – unngå å materialisere store grafer.
- Unngå Lazy Loading i Web API (N+1); bruk `Include` eller projeksjon.
- Paginér (`Skip`/`Take`) og filtrér i DB. Sett indekser i migrasjoner.
- For hot paths: vurder **kompilerte spørringer**.
- Vær eksplisitt på **kolonnetyper** og lengder (MySQL).

### Robusthet

- Aktiver retry (`EnableRetryOnFailure()`).
- Bruk transaksjoner for flere endringer som hører sammen.
- Logg spørringer; bruk `ToQueryString()` i feilsøking.
- Håndter samtidighetskonflikter (radversjon/timestamp).

### Sikkerhet/drift

- Hemmeligheter i **User Secrets** / **Key Vault** (ikke i repo).
- CI/CD: generér idempotente skript for produksjon.
- Least‑privilege DB‑bruker.
- Husk parameterisering ved bruk av `FromSqlRaw` (unngå injeksjon).

### Testing

- Bruk **SQLite in‑memory** eller en **test‑MySQL** via container for integrasjonstester.
- Seed testdata i test‑setup. Test via ekte provider der det er mulig.

---
<div style="page-break-after: always;"></div>

## 10) `dotnet ef` – installasjon og kommandoer 

> Installer verktøyet globalt én gang, oppdater ved behov.

**Installasjon/oppdatering**

- `dotnet tool install --global dotnet-ef`
- `dotnet tool update --global dotnet-ef`
- Verifiser: `dotnet ef --version`

| Kategori | Kommando | Hva den gjør | Vanlige valg (utdrag) | Eksempel (MySQL) |
|---|---|---|---|---|
| **Tool** | `dotnet tool install --global dotnet-ef` | Installerer EF CLI‑verktøyet | – | `dotnet tool install --global dotnet-ef` |
| **Tool** | `dotnet tool update --global dotnet-ef` | Oppdaterer EF CLI‑verktøyet | – | `dotnet tool update --global dotnet-ef` |
| **Migrasjon** | `dotnet ef migrations add <Name>` | Lager ny migrasjon basert på modellendringer | `--output-dir`, `--context` | `dotnet ef migrations add InitialCreate --context SchoolContext` |
| **Migrasjon** | `dotnet ef migrations list` | Viser migrasjoner | `--context` | `dotnet ef migrations list` |
| **Migrasjon** | `dotnet ef migrations remove` | Fjerner siste migrasjon (ikke på prod) | `--context` | `dotnet ef migrations remove` |
| **DB** | `dotnet ef database update` | Kjører migrasjoner mot DB | `--context`, `--connection` | `dotnet ef database update --context SchoolContext` |
| **DB** | `dotnet ef database drop` | Sletter databasen | `--force`, `--context` | `dotnet ef database drop --force` |
| **Script** | `dotnet ef migrations script` | Genererer SQL‑skript (idempotent m/ `--idempotent`) | `-o`, `--idempotent` | `dotnet ef migrations script --idempotent -o deploy.sql` |
| **Scaffold (DB‑first)** | `dotnet ef dbcontext scaffold <conn> <provider>` | Genererer modeller/DbContext fra eksisterende DB | `--output-dir`, `--context`, `--table`, `--schema`, `--use-database-names`, `--no-onconfiguring` | `dotnet ef dbcontext scaffold "<conn>" Pomelo.EntityFrameworkCore.MySql --output-dir Data/Models --context SchoolContext --use-database-names --no-onconfiguring` |
| **Info** | `dotnet ef dbcontext info` | Viser info om DbContext | `--context` | `dotnet ef dbcontext info --context SchoolContext` |
| **Kompilering** | `dotnet ef dbcontext optimize`* | Forbereder prekompilering av modeller (avh. versjon) | `--context`, `--output-dir` | `dotnet ef dbcontext optimize --context SchoolContext` |

\* Tilgjengeligheten av enkelte kommandoer kan variere med EF‑versjon. Sjekk `dotnet ef --help` i ditt miljø.

---
<div style="page-break-after: always;"></div>

## 11) Design‑time factory (for CLI) 

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DesignTimeFactory : IDesignTimeDbContextFactory<SchoolContext>
{
    public SchoolContext CreateDbContext(string[] args)
    {
        var connStr = "Server=localhost;Port=3306;Database=school_db;User=root;Password=your_password;TreatTinyAsBoolean=true;";
        var options = new DbContextOptionsBuilder<SchoolContext>()
            .UseMySql(connStr, ServerVersion.AutoDetect(connStr))
            .Options;
        return new SchoolContext(options);
    }
}
```

---

## 12) Hurtigoppskrift (cheat sheet)

1. Installer pakker + `dotnet-ef`‑tool (Pomelo for MySQL).
2. Legg til modeller (Annotations) og/eller Fluent‑konfig i `OnModelCreating`.
3. Registrer `DbContext` med `UseMySql(...)` i `Program.cs`.
4. **Code‑first:** `dotnet ef migrations add InitialCreate` → `dotnet ef database update`.
5. **Database‑first:** `dotnet ef dbcontext scaffold "<conn>" Pomelo.EntityFrameworkCore.MySql ...`.
6. Projiser til DTO‑er, bruk `AsNoTracking()` for read, paginér.
7. Aktiver retry, logg spørringer, profiler og indekser.
8. Deploy med idempotente skript + sikre hemmeligheter.

---

## 13) Videre lesing

- Søk opp **Microsoft Learn: EF Core** for konsept- og API‑detaljer.
- **Pomelo EFCore MySql** GitHub for provider‑spesifikke tips.
- Artikkelserier om LINQ‑ytelse og SQL‑profilering i MySQL.

---

**Lykke til med undervisningen!**  
Denne filen kan brukes som handout, og du kan bygge forelesningene direkte på eksemplene.
