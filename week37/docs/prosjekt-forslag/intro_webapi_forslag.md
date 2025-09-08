# Forslag til enkle Web API-prosjekter (1 controller, 1 modell)

Her finner du forslag til **små og enkle prosjekter** som kan brukes for å introdusere **Controllers**, **API-endepunkter** og **Entity Framework Core (EF Core)**.  
Hvert forslag har **kun én modell** og **én controller** for å gjøre det oversiktlig i starten.

---

## 1. Simple Notes API

En API hvor brukeren kan lagre og hente notater.

**Modell:**

- `Note`
  - Id (int)
  - Title (string)
  - Content (string)
  - CreatedAt (DateTime)

**Typiske endepunkter:**

| Endpoint | Method | Beskrivelse |
|-----------|--------|-------------|
| `/api/notes` | GET | Hent alle notater |
| `/api/notes/{id}` | GET | Hent ett notat |
| `/api/notes` | POST | Opprett nytt notat |
| `/api/notes/{id}` | PUT | Oppdater et notat |
| `/api/notes/{id}` | DELETE | Slett et notat |

**Læringsfokus:**

- CRUD-operasjoner.
- Grunnleggende EF Core integrasjon.
- Minimal relasjonell kompleksitet.

---

## 2. Simple Products API

En enkel API for å håndtere produkter, som et mini-lagersystem.

**Modell:**

- `Product`
  - Id (int)
  - Name (string)
  - Price (decimal)
  - Stock (int)

**Typiske endepunkter:**

| Endpoint | Method | Beskrivelse |
|-----------|--------|-------------|
| `/api/products` | GET | Hent alle produkter |
| `/api/products/{id}` | GET | Hent ett produkt |
| `/api/products` | POST | Opprett nytt produkt |
| `/api/products/{id}` | PUT | Oppdater et produkt |
| `/api/products/{id}` | DELETE | Slett et produkt |

**Læringsfokus:**

- EF Core `DbContext` med enkel tabell.
- Inputvalidering med `DataAnnotations`.
- API-design med standard CRUD.

---

## 3. Simple Contacts API

En enkel kontaktliste der brukeren kan lagre navn og telefonnummer.

**Modell:**

- `Contact`
  - Id (int)
  - Name (string)
  - Email (string)
  - Phone (string)

**Typiske endepunkter:**

| Endpoint | Method | Beskrivelse |
|-----------|--------|-------------|
| `/api/contacts` | GET | Hent alle kontakter |
| `/api/contacts/{id}` | GET | Hent én kontakt |
| `/api/contacts` | POST | Opprett ny kontakt |
| `/api/contacts/{id}` | PUT | Oppdater kontakt |
| `/api/contacts/{id}` | DELETE | Slett kontakt |

**Læringsfokus:**

- Viser hvordan man strukturerer input/output med DTO-er.
- Gir en praktisk case studentene kan relatere til.

---

## 4. Simple Tasks API

En API for å lagre oppgaver i en gjøremålsliste (to-do list).

**Modell:**

- `TaskItem`
  - Id (int)
  - Title (string)
  - IsDone (bool)
  - DueDate (DateTime)

**Typiske endepunkter:**

| Endpoint | Method | Beskrivelse |
|-----------|--------|-------------|
| `/api/tasks` | GET | Hent alle oppgaver |
| `/api/tasks/{id}` | GET | Hent én oppgave |
| `/api/tasks` | POST | Opprett ny oppgave |
| `/api/tasks/{id}` | PUT | Oppdater oppgave |
| `/api/tasks/{id}` | DELETE | Slett oppgave |

**Læringsfokus:**

- Enkelt prosjekt for å forklare statusfelt (`IsDone`).
- Filtrering med query parameters (f.eks. `/api/tasks?done=true`).

---

## 5. Simple Weather Reports API

En API for å lagre og hente enkle værmålinger.

**Modell:**

- `WeatherReport`
  - Id (int)
  - City (string)
  - Temperature (decimal)
  - ReportedAt (DateTime)

**Typiske endepunkter:**

| Endpoint | Method | Beskrivelse |
|-----------|--------|-------------|
| `/api/weather` | GET | Hent alle rapporter |
| `/api/weather/{id}` | GET | Hent én rapport |
| `/api/weather` | POST | Opprett ny rapport |
| `/api/weather/{id}` | PUT | Oppdater rapport |
| `/api/weather/{id}` | DELETE | Slett rapport |

**Læringsfokus:**

- Viser tidsfelt (`ReportedAt`).
- Perfekt for å introdusere filtrering etter dato.

---

## Anbefalt progresjon

1. **Simple Tasks API** – veldig enkel start, kun én boolsk status.
2. **Simple Contacts API** – introduksjon til mer strukturert data.
3. **Simple Products API** – viser håndtering av numeriske felter (pris, lagerbeholdning).
4. **Simple Notes API** – arbeid med tekstfelt og datetime.
5. **Simple Weather Reports API** – mer realistiske data og filtrering.

---

## Oppsummering

| Prosjekt | Kompleksitet | Fokuspunkter |
|----------|--------------|--------------|
| Simple Tasks API | Enkel | Grunnleggende CRUD |
| Simple Contacts API | Enkel | DTO-er og validering |
| Simple Products API | Enkel | Tallfelt, grunnleggende forretninglogikk |
| Simple Notes API | Enkel | Tekstfelter, datetime |
| Simple Weather Reports API | Middels | Filtrering og dato-håndtering |
