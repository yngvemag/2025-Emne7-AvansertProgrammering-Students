# Kodeorganisering i ASP.NET: Lagdelt vs. By Feature (Vertical Slice)

<small>

## 1. Lagdelt Arkitektur

### Beskrivelse

I lagdelt arkitektur er koden delt opp etter ansvarsområder eller tekniske lag, som for eksempel presentasjon (Controllers), forretningslogikk (Services), dataadgang (Repositories), og domenemodeller (Models). Hver lag er ansvarlig for en spesifikk del av applikasjonens funksjonalitet og kommuniserer med de andre lagene.

### Typiske lag

- **Controllers**: Håndterer HTTP-forespørsler og responslogikk.
- **Services**: Håndterer forretningslogikken.
- **Repositories**: Håndterer dataadgang og CRUD-operasjoner.
- **Models**: Definerer domenemodeller (objektene som brukes i applikasjonen).

### Fordeler

- Velkjent og vanlig arkitektur som er enkel å forstå.
- Separate ansvar gjennom de ulike lagene.
- God for gjenbruk av logikk på tvers av funksjoner.

### Ulemper

- Kan bli vanskelig å navigere når prosjektet vokser.
- Vanskelig å koble spesifikke funksjoner eller user stories til en del av koden, da logikken er spredt utover flere mapper/lag.

<div style="page-break-after: always;"></div>

### Eksempel på lagdelt struktur

```plaintext
ProjectName/
│
├── Controllers/
│   ├── UsersController.cs
│   ├── PostsController.cs
│   └── CommentsController.cs
│
├── Services/
│   ├── UserService.cs
│   ├── PostService.cs
│   └── CommentService.cs
│
├── Repositories/
│   ├── UserRepository.cs
│   ├── PostRepository.cs
│   └── CommentRepository.cs
│
├── Models/
│   ├── User.cs
│   ├── Post.cs
│   └── Comment.cs
│
└── Program.cs
```

## 2. By Feature (Vertical Slice) Arkitektur

### Beskrivelse

By feature, eller vertical slice-arkitektur, organiserer koden etter funksjoner eller brukerhistorier i stedet for tekniske lag. Hver funksjon får sin egen mappe, og alle komponentene (controller, service, repository, etc.) som trengs for å implementere denne funksjonen ligger i samme mappe. Dette gir en mer modulær og funksjonsorientert struktur.

### Fordeler

Gjør det enkelt å knytte koden til spesifikke funksjoner eller user stories.
Forenkler vedlikehold av spesifikke funksjoner, da all relatert logikk er samlet på ett sted.
Kan gjøre det lettere å skalere, siden nye funksjoner kan legges til uten å påvirke andre deler av koden.

<div style="page-break-after: always;"></div>

### Ulemper

Kan føre til duplisering av kode hvis ikke abstrahering brukes riktig.
Mindre vanlig og kan kreve mer opplæring for team som er vant til lagdelt arkitektur.
Eksempel på by feature/vertical slice struktur:

```plaintext
ProjectName/
│
├── Features/
│   ├── Users/
│   │   ├── UserController.cs
│   │   ├── UserService.cs
│   │   ├── UserRepository.cs
│   │   └── User.cs
│   ├── Posts/
│   │   ├── PostController.cs
│   │   ├── PostService.cs
│   │   ├── PostRepository.cs
│   │   └── Post.cs
│   └── Comments/
│       ├── CommentController.cs
│       ├── CommentService.cs
│       ├── CommentRepository.cs
│       └── Comment.cs
│
├── Middleware/
│   ├── AuthMiddleware.cs
│   └── ExceptionMiddleware.cs
│
├── Exceptions/
│   └── CustomExceptions.cs
│
└── Program.cs

```
<div style="page-break-after: always;"></div>

## Sammenligning av de to tilnærmingene

| Egenskap                      | Lagdelt Arkitektur                                        | By Feature (Vertical Slice)                                  |
|-------------------------------|----------------------------------------------------------|--------------------------------------------------------------|
| **Organisering**               | Basert på tekniske lag som Controller, Service, Repository | Basert på funksjoner som User, Post, Comment                  |
| **Modularitet**                | Logikk er delt mellom lag                                 | Hver funksjon er modulær og isolert                           |
| **Navigering**                 | Kan bli mer kompleks etterhvert som prosjektet vokser     | Enklere å finne koden for en spesifikk funksjon               |
| **Gjenbruk**                   | Gjenbruk av tjenester og repositories på tvers av lag     | Mindre gjenbruk, men lettere å isolere og teste               |
| **Egnet for små prosjekter**   | Ja                                                        | Ja                                                            |
| **Egnet for store prosjekter** | Kan bli vanskelig å skalere                               | Bedre skalerbarhet med moduler for hver funksjon              |

### Oppsummering

- **Lagdelt Arkitektur** passer godt hvis du har et mindre prosjekt eller hvis du har mange felles tjenester som brukes på tvers av funksjoner.
- **By Feature (Vertical Slice)** passer godt for større prosjekter med mange uavhengige funksjoner, og gir en mer modulær tilnærming hvor hver funksjon kan utvides uten å påvirke andre deler av systemet.

</small>
