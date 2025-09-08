# Forslag til enkle Web API-prosjekter for undervisning

Denne filen inneholder forslag til **enkle, men realistiske prosjekter** du kan bruke når du underviser **Web API med ASP.NET Core**.  
Prosjektene er designet for å kunne kodes **sammen med studentene** steg for steg, samtidig som du kan forklare prinsippene bak **lagdeling**, **Entity Framework Core**, og **REST API-design**.

---

## 1. Books & Authors API (klassikeren)

**Beskrivelse:**  
En API for å håndtere **bøker** og **forfattere**.  
Hver bok har én forfatter, men én forfatter kan ha mange bøker (1–mange-relasjon).

**Hvorfor den er bra:**

- Lett for studentene å forstå domenet.
- Viser **relasjoner i databasen**.
- Gir naturlig grunnlag for **to controllere** (`AuthorsController` og `BooksController`).

**Eksempler på endpoints:**

| Endpoint | HTTP Method | Beskrivelse |
|-----------|-------------|-------------|
| `/api/authors` | GET | Hent alle forfattere |
| `/api/authors/{id}` | GET | Hent en forfatter med alle bøkene |
| `/api/authors` | POST | Opprett ny forfatter |
| `/api/books` | GET | Hent alle bøker |
| `/api/books/{id}` | GET | Hent en bok med forfatterinfo |
| `/api/books` | POST | Opprett ny bok |
| `/api/books/{id}` | PUT | Oppdater bok |
| `/api/books/{id}` | DELETE | Slett bok |

**Datamodell:**

```csharp
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Country { get; set; } = default!;

    public ICollection<Book> Books { get; set; } = new List<Book>();
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public int Year { get; set; }

    public int AuthorId { get; set; }
    public Author Author { get; set; } = default!;
}
```

**Utvidelser senere:**

- Legg til **kategori** for bøker → mange-til-mange-relasjon.
- Legg til **søkefunksjon** (`GET /api/books?title=harry`).

---

## 2. Movie & Reviews API

**Beskrivelse:**  
En API for filmer der brukere kan legge inn anmeldelser.  
En film kan ha mange anmeldelser, men hver anmeldelse tilhører kun én film.

**Hvorfor den er bra:**

- Gir naturlig **to nivåer med lagdeling**: `MoviesController` og `ReviewsController`.
- Perfekt for å forklare hvordan `Include()` i EF Core fungerer.

**Eksempler på endpoints:**

| Endpoint | HTTP Method | Beskrivelse |
|-----------|-------------|-------------|
| `/api/movies` | GET | Hent alle filmer |
| `/api/movies/{id}` | GET | Hent film med anmeldelser |
| `/api/movies` | POST | Opprett ny film |
| `/api/reviews` | POST | Legg til anmeldelse for en film |
| `/api/reviews/{id}` | DELETE | Slett anmeldelse |

**Datamodell:**

```csharp
public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Genre { get; set; } = default!;
    public int Year { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

public class Review
{
    public int Id { get; set; }
    public string Reviewer { get; set; } = default!;
    public string Comment { get; set; } = default!;
    public int Rating { get; set; } // 1-5

    public int MovieId { get; set; }
    public Movie Movie { get; set; } = default!;
}
```

**Utvidelser senere:**

- Legg til **brukere** som logger inn for å poste anmeldelser.
- Legg til **gjennomsnittsrating** på filmer via en GET endpoint.

---

## 3. Todo & Categories API (superenkel start)

**Beskrivelse:**  
En enklere variant av Books & Authors – perfekt som første prosjekt.  
Brukerne kan lage oppgaver (`Todo`) og kategorisere dem (`Category`).

**Hvorfor den er bra:**

- Svært enkel datamodell.
- Perfekt som **første prosjekt** for å fokusere på lagdeling, EF Core og API-endepunkter.

**Datamodell:**

```csharp
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public ICollection<Todo> Todos { get; set; } = new List<Todo>();
}

public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public bool IsDone { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = default!;
}
```

**Eksempler på endpoints:**

| Endpoint | HTTP Method | Beskrivelse |
|-----------|-------------|-------------|
| `/api/categories` | GET | Hent alle kategorier |
| `/api/todos` | GET | Hent alle todos |
| `/api/todos` | POST | Opprett ny todo |
| `/api/todos/{id}` | PUT | Oppdater todo (fullført/ikke fullført) |
| `/api/todos/{id}` | DELETE | Slett todo |

---

## 4. Student & Courses API

**Beskrivelse:**  
En API for studenter og kurs med en **mange-til-mange-relasjon**.

**Hvorfor den er bra:**

- Introduserer **komplekse relasjoner**.
- Perfekt når studentene har begynt å mestre EF Core.

**Datamodell:**

```csharp
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
}

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;

    public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
}

public class StudentCourse
{
    public int StudentId { get; set; }
    public Student Student { get; set; } = default!;
    
    public int CourseId { get; set; }
    public Course Course { get; set; } = default!;
}
```

---

## Anbefalt rekkefølge for undervisning

1. **Todo & Categories API** – superenkel start, fokus på API, lagdeling og EF Core.
2. **Books & Authors API** – legg til relasjoner (1–mange), DTO-er og to controllere.
3. **Movie & Reviews API** – vis `Include()`, datafiltrering, og validering.
4. **Student & Courses API** – avanserte relasjoner (mange–mange).

---

## Lagdelingseksempel

Struktur for **Books & Authors API**:

```
MyLibraryApi/
│
├── API
│   └── Controllers
│       ├── AuthorsController.cs
│       └── BooksController.cs
│
├── Application
│   ├── Services
│   │   ├── AuthorService.cs
│   │   └── BookService.cs
│   └── DTOs
│       ├── AuthorDto.cs
│       └── BookDto.cs
│
├── Domain
│   └── Entities
│       ├── Author.cs
│       └── Book.cs
│
└── Infrastructure
    ├── Data
    │   └── LibraryContext.cs
    └── Repositories
        ├── AuthorRepository.cs
        └── BookRepository.cs
```

---

## Tips for undervisning

- **Kod sammen** med studentene i sanntid, forklar steg for steg.
- Start enkelt: én controller, én modell, deretter utvid med relasjoner.
- Bruk **Swagger** fra starten slik at de ser API-endepunktene visuelt.
- La studentene **bygge videre selv** som en oppgave etter hver forelesning.
- Vis **migrasjoner i EF Core** slik at de ser hvordan databasen bygges opp over tid.

---

## Oppsummering

| Prosjekt | Kompleksitet | Hovedfokus |
|----------|--------------|------------|
| Todo & Categories API | Enkel | Grunnleggende lagdeling, EF Core, API-endepunkter |
| Books & Authors API | Middels | Relasjoner (1–mange), to controllere |
| Movie & Reviews API | Middels | Eager loading, `Include()`, filtrering |
| Student & Courses API | Avansert | Mange–mange-relasjoner, avansert EF Core |
