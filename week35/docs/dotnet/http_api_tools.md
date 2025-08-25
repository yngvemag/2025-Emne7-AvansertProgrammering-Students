
# Postman, Scalar og Swagger i ASP.NET Core – Oppsett og Bruk

## 1. Swagger (OpenAPI)

### Hva er Swagger?

- **Swagger** er en samling verktøy for å dokumentere og teste API-er.
- ASP.NET Core bruker ofte **Swashbuckle** for å generere Swagger UI og OpenAPI-dokumentasjon automatisk.
- Lar deg se alle API-endepunkter, sende test-requests og se respons direkte i nettleseren.

### Oppsett i ASP.NET Core

Installer NuGet-pakken:

```bash
dotnet add package Swashbuckle.AspNetCore
```

Legg til i `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
```

<div style="page-break-after: always;"></div>

Når applikasjonen kjører, gå til:  

```
https://localhost:5001/swagger
```

Her kan du teste alle API-endepunkter direkte.

### Tilpasning (valgfritt)

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "Eksempel API-dokumentasjon"
    });
});
```

---
<div style="page-break-after: always;"></div>

## 2. Scalar

### Hva er Scalar?

- **Scalar** er et alternativ til Swagger UI som gir en mer moderne og interaktiv OpenAPI-dokumentasjonsopplevelse.
- Bruker samme OpenAPI-spesifikasjon som Swagger, men med forbedret design og brukervennlighet.

### Oppsett i ASP.NET Core

Installer NuGet-pakken:

```bash
dotnet add package Scalar.AspNetCore
```

Legg til i `Program.cs`:

```csharp
builder.Services.AddOpenApi(); // Registrerer OpenAPI-spesifikasjonen

var app = builder.Build();

app.MapOpenApi();      // Genererer /openapi/v1.json
app.MapScalarApiReference(); // Genererer Scalar UI på /scalar
```

Kjør applikasjonen og åpne:  

```
https://localhost:5001/scalar
```

Her får du en moderne UI for API-dokumentasjon og testing.

---
<div style="page-break-after: always;"></div>

## 3. Postman

### Hva er Postman?

- **Postman** er et eksternt verktøy (desktop eller web) for å sende HTTP-requests (GET, POST, PUT, DELETE, osv.) til API-er.
- Nyttig for å teste API-er under utvikling og produksjon.
- Lar deg lagre **collections**, **environment variables** og **automatisere tester**.

### Hvordan bruke Postman med ASP.NET Core API?

1. **Installer Postman**: [https://www.postman.com/downloads/](https://www.postman.com/downloads/)
2. Start API-en lokalt (f.eks. `https://localhost:5001`).
3. Åpne Postman → Lag en ny request:
   - Velg metode (GET/POST/etc.)
   - Skriv inn URL: `https://localhost:5001/api/myendpoint`
   - Legg til body (JSON) hvis POST/PUT:

     ```json
     {
       "name": "Test",
       "age": 30
     }
     ```

4. Klikk **Send** for å se respons.

### Importere Swagger/OpenAPI i Postman

- Generer swagger JSON: `https://localhost:5001/swagger/v1/swagger.json`
- I Postman: `Import` → `Link` → lim inn URL til swagger.json → Postman lager automatisk alle endepunkter som en collection.

---

### Tips: Importer OpenAPI direkte til Postman

I stedet for å manuelt legge inn hvert endepunkt i Postman, kan du importere hele API-spesifikasjonen:

1. Kjør API-et lokalt og åpne OpenAPI-spesifikasjonen:  
   `https://localhost:5001/openapi/v1.json` (eller `swagger/v1/swagger.json` hvis du bruker Swagger).
2. Kopier URL-en.
3. I Postman: Klikk **Import** → **Link** → Lim inn URL-en til `openapi/v1.json`.
4. Postman oppretter automatisk en **collection** med alle endepunktene.

Dette sparer tid og sikrer at Postman-collection alltid er oppdatert med API-spesifikasjonen.

## 4. Oppsummering

- **Swagger**: Innebygd i ASP.NET Core, kjører i nettleseren for dokumentasjon/testing.
- **Scalar**: Moderne UI for OpenAPI, kjører på `/scalar`.
- **Postman**: Eksternt verktøy, mer avansert testing, collections og automatisering.

Bruk gjerne alle tre sammen:  

- Swagger/Scalar for rask testing og dokumentasjon under utvikling.  
- Postman for dypere testing, integrasjoner og automatisering.
