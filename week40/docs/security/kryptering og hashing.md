# Kryptering

Kryptering er en prosess som konverterer lesbar tekst eller data til en kodet versjon ved hjelp av en algoritme og en nøkkel. Formålet er å sikre at dataene forblir private og beskyttede, og bare de med riktig nøkkel kan dekryptere og få tilgang til den opprinnelige informasjonen. Det er to hovedtyper av kryptering: symmetrisk og asymmetrisk kryptering.

## Symmetrisk kryptering

I symmetrisk kryptering brukes den samme nøkkelen til både kryptering og dekryptering av data.
Denne metoden er rask og effektiv for store datamengder.
Hovedutfordringen er sikker nøkkelutveksling og distribusjon. Begge partene som er involvert i kommunikasjonen, må ha tilgang til nøkkelen på forhånd, og nøkkelen må holdes strengt hemmelig for å sikre datasikkerheten.

Eksempler på symmetriske krypteringsalgoritmer inkluderer:
 
- Advanced Encryption Standard (AES)
- Data Encryption Standard (DES)

## Asymmetrisk kryptering (eller offentlig nøkkelkryptering)

Asymmetrisk kryptering bruker to nøkler: en offentlig nøkkel for kryptering og en privat nøkkel for dekryptering.
Den offentlige nøkkelen kan deles fritt, mens den private nøkkelen holdes hemmelig. Data som er kryptert med den offentlige nøkkelen, kan bare dekrypteres med den tilsvarende private nøkkelen, og omvendt.
Asymmetrisk kryptering er generelt tregere enn symmetrisk kryptering, men den løser problemet med nøkkelutveksling ved å tillate sikker kommunikasjon mellom parter som ikke har utvekslet noen hemmelige nøkler på forhånd.

Eksempler på asymmetriske krypteringsalgoritmer inkluderer:

- RSA
- DSA
- Elliptic Curve Cryptography (ECC)

Begge typene kryptering har sine styrker og brukes ofte sammen i mange sikkerhetssystemer. For eksempel kan asymmetrisk kryptering brukes til å utveksle en symmetrisk nøkkel sikkert, som deretter brukes for å kryptere den faktiske datakommunikasjonen. Dette gir en god balanse mellom ytelse og sikkerhet.

<div style="page-break-after:always;"></div>

# Hashing

Hashing er en prosess som konverterer en inndata (eller 'melding') til en fast størrelse streng av bytes. Strengen er typisk en sekvens av tegn som representerer en numerisk verdi generert fra inndata. Her er noen nøkkelpunkter om hvordan hashing fungerer og hva det brukes til:

### Fast utdatastørrelse

- Uansett størrelsen på inndata, vil hashfunksjonen produsere en hashverdi av fast størrelse. For eksempel vil SHA-256 alltid produsere en 256-bit lang hash.

### Deterministisk

- En gitt inndata vil alltid produsere samme hashverdi. Det er ingen tilfeldighet involvert, så den samme inndata vil alltid gi samme utdata når den sendes gjennom en spesifikk hashfunksjon.

### Rask

- Hashfunksjoner er designet for å være raske, slik at de kan behandle data og generere hashverdier effektivt.

### Preimage-resistens

- Det skal være computationally umulig å reversere en hashfunksjon for å finne den opprinnelige inndataen fra hashverdien. Dette er kjent som preimage-resistens.

### Kollisjonsresistens

- Det skal være computationally umulig å finne to forskjellige inndata som produserer samme hashverdi. Dette er kjent som kollisjonsresistens.

### Avalanche-effekt

- En liten endring i inndata skal produsere en dramatisk forskjellig hashverdi. Selv en endring på én bit i inndata skal resultere i en fullstendig forskjellig hash.

## Bruksområder for Hashing

- **Dataintegritetskontroller**:
  - Hashing brukes ofte til å verifisere integriteten til data. Ved å sammenligne hashverdien av originaldata med hashverdien av mottatte eller lagrede data, kan systemer oppdage om dataene har blitt endret.
- **Passordlagring**:
  - Passord hashes før de lagres i databaser. Når en bruker prøver å logge inn, hashes passordet de oppgir, og sammenlignes med den lagrede hashverdien.
- **Digitale signaturer**:
  - Hashing brukes i generering av digitale signaturer. En hash av en melding signeres i stedet for meldingen selv, noe som gir bevis på opprinnelse og integritet av meldingen.
- **Kryptografiske applikasjoner**:
  - Hashfunksjoner brukes i ulike kryptografiske applikasjoner og protokoller, inkludert SSL/TLS, Bitcoin og andre kryptokurrency protokoller.
- **Dataindeksering**:
  - Hashing brukes også til dataindeksering, hvor hashverdier brukes som indekser for rask dataoppslag.

Hashing er en fundamental teknologi i moderne sikkerhets- og datasystemer, og forståelse av hvordan hashing fungerer, er kritisk for mange domener innen informatikk og informasjonssikkerhet.

## Salt

"Salt" i sammenheng med hashing refererer til en tilfeldig verdi som genereres og legges til inndata før hashingprosessen. Dette gjør at selv identiske inndata vil produsere forskjellige hashverdier, noe som bidrar til å forbedre sikkerheten. Her er hvordan salting fungerer og hvorfor det er viktig:

### Tilfeldig Verdi

- Saltet er en tilfeldig verdi som genereres unikt for hver inndata. Det er viktig at saltet er tilfeldig og unikt for å sikre effektiviteten av salting.

### Kombinering med Inndata

- Saltet blir kombinert med inndata, ofte ved å legge det til i begynnelsen eller slutten av inndata, før hashfunksjonen blir brukt.

### Unik Hash

- Ved å legge til et unikt salt til inndata, sikres det at selv identiske inndata produserer unike hashverdier. Dette forhindrer at angripere kan bruke forhåndsberegnede tabeller, kjent som rainbow tables, for å reversere hashverdier til deres opprinnelige inndata.

### Lagring av Salt

- Saltet må lagres sammen med hashverdien for å kunne verifisere inndata senere. Når inndata skal verifiseres, kombineres det med saltet, hashes, og sammenlignes med den lagrede hashverdien.

### Beskyttelse mot Angrep

- Salting forhindrer effektivt angrep som bruker forhåndsberegnede tabeller for å reversere hashverdier. Det beskytter også mot såkalte kollisjonsangrep ved å sørge for at identiske inndata har unike hashverdier.

### Forbedrer Passordsikkerheten

- I sammenheng med passordlagring gjør salting det mye vanskeligere for angripere å bruke teknikker som brute force eller dictionary attacks for å gjette passord.

Salting er en viktig praksis som betydelig forbedrer sikkerheten og robustheten til systemer som bruker hashing for å beskytte eller verifisere data.

## BCrypt

BCrypt.Net-biblioteket, som er et populært bibliotek for hashing og salting av passord i .NET-applikasjoner. BCrypt er spesielt designet for å beskytte passord ved å bruke en adaptiv hashing-algoritme som inkluderer innebygd salting og en kostnadsfaktor for å gjøre bruteforce-angrep vanskeligere.

BCrypt.Net Oversikt
BCrypt.Net er et .NET-bibliotek som implementerer BCrypt-hashingen, som er en kryptografisk hashing-algoritme designet for å være både sikker og relativt langsom for å motvirke bruteforce-angrep. BCrypt.Net tilbyr funksjoner som automatisk salting og styring av kompleksiteten til hashing-operasjoner.

### Funksjoner

- Automatisk Salting:
  - Når du hasher et passord med BCrypt, genereres salt automatisk og lagres sammen med hashverdien.
- Innebygd Kostnadsfaktor:
  - BCrypt bruker en "arbeidsfaktor" eller kostnadsfaktor (også kjent som en logrounds), som bestemmer hvor mange ganger algoritmen kjøres. Jo høyere denne verdien er, jo langsommere vil hashingprosessen være, noe som gjør det vanskeligere å bruteforce passordet.
- Passordverifisering:
  - BCrypt tilbyr innebygde funksjoner for å sammenligne et passord med en lagret hash, inkludert salt.

### Installasjon

For å installere BCrypt.Net i ditt .NET-prosjekt, kan du bruke NuGet-pakken ved hjelp av følgende kommando i Package
Manager Console

```bash
Install-Package BCrypt.Net-Next
```

.Net CLI

```bash
dotnet add package BCrypt.Net-Next
```
<div style="page-break-after:always;"></div>

## Grunnleggende Bruk

**1. Hashing av et passord**
For å hashe et passord med BCrypt og lagre det i databasen:

```csharp 
using BCrypt.Net;

// Hash passordet med en arbeidsfaktor (default er 10)
string hashedPassword = BCrypt.HashPassword("MinHemmeligePassord");

// Lagre `hashedPassword` i databasen

```

Her genererer HashPassword automatisk et salt og bruker det sammen med passordet for å generere en sikker hash.

**2. Verifisering av passord**
For å verifisere et brukers passord under innlogging, sammenlign det oppgitte passordet med den lagrede hashen:

```csharp 
using BCrypt.Net;

// Sammenlign brukers inndata med den lagrede hashen
bool isCorrect = BCrypt.Verify("MinHemmeligePassord", hashedPassword);

if (isCorrect)
{
    Console.WriteLine("Passordet er riktig");
}
else
{
    Console.WriteLine("Feil passord");
}

```

**3. Tilpasse kostnadsfaktor**
BCrypt tillater deg å justere kostnadsfaktoren (arbeidsfaktor) for å gjøre hashingprosessen mer krevende. Dette gjør algoritmen langsommere, noe som øker sikkerheten:

```csharp
int workFactor = 12; // Høyere arbeidsfaktor gjør hashingprosessen tregere og sikrere
string hashedPassword = BCrypt.HashPassword("MinHemmeligePassord", workFactor);

```
<div style="page-break-after:always;"></div>

## Anbefalinger for Bruk

1. ### Velg en Fornuftig Arbeidsfaktor

- En typisk arbeidsfaktor for BCrypt er 10, men dette kan økes etter hvert som maskinvaren blir raskere. Du bør finne en balanse mellom sikkerhet og ytelse.
Oppbevar Ikke Rå Passord:

2. ### Oppbevar aldri rå passord i databasen

- Bruk alltid BCrypt til å hashe og salte passordene før de lagres.

3. ### Oppdater Arbeidsfaktor

Etterhvert som maskinvare forbedres, bør du periodisk øke arbeidsfaktoren for å sikre at hashene forblir vanskelige å bruteforce. Dette krever at du rehasher passordene når brukerne logger inn.

4. ### Verifiser Ikke Manuelt

- Bruk den innebygde BCrypt.Verify-funksjonen i stedet for å skrive egendefinerte verifiseringsalgoritmer. Dette sikrer at verifiseringen alltid fungerer korrekt med det innebygde saltet og kostnadsfaktoren.

```csharp
using BCrypt.Net;

// Hash et nytt passord
string originalPassword = "MinHemmeligePassord";
string hashedPassword = BCrypt.HashPassword(originalPassword);

Console.WriteLine($"Hashed passord: {hashedPassword}");

// Simuler innlogging: verifiser passordet
string inputPassword = "MinHemmeligePassord"; // Dette ville normalt komme fra brukeren
bool isCorrect = BCrypt.Verify(inputPassword, hashedPassword);

if (isCorrect)
{
    Console.WriteLine("Passordet er riktig");
}
else
{
    Console.WriteLine("Feil passord");
}


```
