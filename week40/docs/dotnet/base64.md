# Base64 Encoding

## Hva er Base64?

**Base64** er en metode for å representere binære data i ASCII-tekstformat. Den tar en binær datastrøm (som bilder, filer eller binære representasjoner) og konverterer den til en streng av 64 forskjellige ASCII-tegn. Disse tegnene består av:

- Store bokstaver (A-Z)
- Små bokstaver (a-z)
- Tall (0-9)
- To spesialtegn (+, /)
- Padding-tegn (`=`), som brukes for å sikre at den endelige strengen er et multiplum av 4 tegn.

### Karaktersettet for Base64

| Verdiområde (Binært) | ASCII-karakter |
|----------------------|----------------|
| 000000 – 00111111    | A–Z            |
| 010000 – 01111111    | a–z            |
| 100000 – 10111111    | 0–9            |
| 110000 – 11101111    | +, /           |
| Padding              | =              |

## Hvordan fungerer Base64?

1. **Input Data**: Dataen som skal kodes, er vanligvis i binært format. Dette kan være alt fra en tekststreng, bilde, eller annen filtype.

2. **Splitting**: Den binære datastrømmen deles opp i 6-bits segmenter (hvert segment kan representere en verdi fra 0 til 63).

3. **Mapping**: Hvert 6-bits segment konverteres til et av de 64 tegnene i Base64-karaktersettet. Hvis datastrømmen ikke er et multiplum av 3 bytes, vil padding-tegnet (`=`) bli brukt på slutten av strengen for å oppnå riktig lengde.

4. **Output**: Den resulterende Base64-strengen inneholder kun ASCII-tegn, som kan sendes i tekstbaserte systemer som ikke støtter binær data.

<div style="page-break-after: always;"></div>

### Eksempel på Base64 Encoding

Ta strengen `"OpenAI"` som eksempel.

1. Først blir strengen konvertert til ASCII-byter:

> O: 01001111 p: 01110000 e: 01100101 n: 01101110 A: 01000001 I: 01001001

2. Denne binære representasjonen deles opp i 6-biters grupper:

> 010011 | 110111 | 000001 | 101010 | 010011 | 100101 | 010000 | 001001

3. Hver gruppe konverteres deretter til tilsvarende Base64-tegn:

> 010011 → T 110111 → 3 000001 → B 101010 → q 010011 → T 100101 → l 010000 → Q 001001 → J

Base64-encoded streng: `T3BqTlFJ`

## Hvorfor brukes Base64?

Base64 brukes fordi det tillater at binær data kan overføres eller lagres i tekstbaserte systemer som kanskje ikke støtter rå binære formater. Dette er spesielt nyttig i protokoller som e-post (SMTP), HTTP, og JSON, der dataene som sendes må være i et tekstbasert format.

### Bruksområder

1. **Dataoverføring i nettprotokoller**:

- Brukes i HTTP-headers for autentisering (som Basic Authentication), der brukernavn og passord kodes som Base64.

2. **E-post**:

- Brukes i MIME for å sende binære filer som vedlegg.

3. **Lagring av binære data i tekstfiler**:

- Base64 tillater binære data å bli lagret i format som er lesbart av tekstbaserte systemer (som JSON eller XML).

4. **Embedde binære filer i HTML/CSS**:

- Bilder eller andre binære filer kan kodes som Base64 og embeddet direkte i HTML-dokumenter (data URLs) for å unngå eksterne HTTP-forespørsler.

<div style="page-break-after: always;"></div>

## Fordeler og Ulemper

### Fordeler

- **Kompatibilitet**: Tillater binære data å bli representert i et tekstformat, noe som gjør det enklere å overføre i nettverk eller lagre i tekstbaserte systemer.
- **Enkel Implementasjon**: Lett å implementere og forstå med mange biblioteker tilgjengelig for ulike programmeringsspråk.
- **Sikkerhet i visse kontekster**: Brukt i Basic Authentication og visse sikkerhetsapplikasjoner.

### Ulemper

- **Plass**: Base64 øker størrelsen på dataen med omtrent 33%. Dette kan være ineffektivt for store mengder data.
- **Ingen kryptering**: Base64 er ikke en krypteringsmetode. Data kodet i Base64 er enkelt å dekode og gir ingen sikkerhetsfordel alene.
- **Kun ASCII-støtte**: Base64 er begrenset til å representere data i ASCII-karakterer og er ikke egnet for direkte binær lagring.

---

<div style="page-break-after: always;"></div>

## Hvordan dekode Base64?

Når Base64-strengen dekodes, gjøres omvendt av prosessen ovenfor:

1. Strengen konverteres fra Base64-tegn til binær.
2. Binærstrømmen deles opp i 8-biters grupper (hver 8-bits gruppe representerer en byte).
3. Byte-verdiene konverteres tilbake til sine originale data (tekst, bilde, etc.).

### Eksempel

Strengen `T3BqTlFJ` kan dekodes tilbake til `"OpenAI"` ved å følge de omvendte stegene.

---

## Base64 i C #

```csharp
using System;

class Program
{
    static void Main()
    {
        string message = "OpenAI";
        byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message);
        string base64Message = Convert.ToBase64String(messageBytes);
        
        Console.WriteLine(base64Message);  // Output: T3BqTlFJ
        
        // Decode
        byte[] base64Bytes = Convert.FromBase64String(base64Message);
        string originalMessage = System.Text.Encoding.ASCII.GetString(base64Bytes);
        
        Console.WriteLine(originalMessage);  // Output: OpenAI
    }
}

```

## Oppsummering

Base64 Encoding er en effektiv måte å representere binære data som en tekstlig representasjon som kan overføres eller lagres i tekstbaserte systemer. Selv om det øker datastørrelsen, er det et populært verktøy for dataoverføring i nettverksprotokoller og lagring av binære data i tekstformater
