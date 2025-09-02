# User Secrets

User Secrets er en mekanisme i ASP.NET Core for å holde sensitive data utenfor konfigurasjonsfilene (som appsettings.json) når du jobber lokalt. Dataene lagres i et separat, kryptert filsystem som er knyttet til prosjektet ditt og brukerprofilen din på maskinen din. Dette gjør det mulig å holde hemmelig informasjon privat under utvikling, uten risiko for å eksponere dem for andre utviklere eller publisere dem ved et uhell.

User Secrets lagres på et sted utenfor prosjektmappen, noe som gjør dem utilgjengelige for versjonskontrollsystemer.

---

## Hvorfor bruke User Secrets?

* Sikkerhet: Hindrer utilsiktet deling av sensitive data, som API-nøkler, tokens, eller databaseforbindelser, gjennom kildekoden.
* Unngå hardkoding: Lagring av sensitive opplysninger i kildekoden eller appsettings.json er en dårlig praksis. User Secrets gir en bedre løsning.
* Separasjon av miljøer: For lokal utvikling kan du ha utviklingsspesifikke hemmeligheter i User Secrets, mens du for produksjon kan bruke sikre miljøvariabler eller andre sikrede konfigurasjonsløsninger (som Azure Key Vault).

---

## Hvordan bruke User Secrets i en ASP.NET Core-applikasjon

### Steg 1: Legg til User Secrets i prosjektet ditt

Først må du sørge for at prosjektet ditt er konfigurert til å bruke User Secrets. Dette gjøres ved å kjøre følgende kommando i terminalen fra rotmappen til prosjektet ditt:

```bash
dotnet user-secrets init
```

Dette vil legge til et UserSecretsId-felt i .csproj-filen din, som ser slik ut:

```bash
<PropertyGroup>
  <UserSecretsId>din-unike-identifikator</UserSecretsId>
</PropertyGroup>
```

### Steg 2: Legg til hemmeligheter

Nå som prosjektet ditt er konfigurert for User Secrets, kan du legge til hemmelig informasjon. Dette gjøres med kommandoen dotnet user-secrets set.

Eksempel på å sette en API-nøkkel i User Secrets:

```bash
dotnet user-secrets set "ApiKey" "DinHemmeligAPIKey"

```

Dette lagrer API-nøkkelen i en hemmelig konfigurasjonsfil på maskinen din, utenfor prosjektmappen.

### Steg 3: Hent hemmelighetene i applikasjonen din

For å hente verdiene du har satt i User Secrets, bruker du ASP.NET Core sin konfigurasjons-API. Verdiene blir automatisk tilgjengelig i IConfiguration-objektet, som allerede er en del av ASP.NET Core applikasjonsoppsett.

Eksempel på hvordan du kan bruke API-nøkkelen i Startup.cs eller Program.cs:

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Legg til User Secrets for utviklingsmiljøer
        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets<Program>();
        }

        var app = builder.Build();

        var apiKey = builder.Configuration["ApiKey"];  // Hent API-nøkkelen fra User Secrets

        app.MapGet("/", () => $"API Key: {apiKey}");

        app.Run();
    }
}

```

I dette eksempelet hentes API-nøkkelen fra User Secrets ved å bruke builder.Configuration["ApiKey"]. Dette er akkurat som å hente verdier fra appsettings.json, men i dette tilfellet kommer de fra User Secrets.

## Hvor lagres User Secrets?

På Windows lagres User Secrets under:

```bash
%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\

```

På MAC lagres User Secrets under:

```bash
~/.microsoft/usersecrets/<user_secrets_id>/

```

Hver fil er knyttet til et prosjekt via en unik UserSecretsId som finnes i .csproj-filen.

## Når bør du bruke User Secrets?

* Under utvikling: For lokalt å lagre sensitive konfigurasjonsinnstillinger som API-nøkler, tokens, eller databasenøkler.
* Unngå hardcoding: I stedet for å lagre sensitive data i appsettings.json, kan du bruke User Secrets for lokalt utviklingsarbeid.
* Skille utviklings- og produksjonsmiljø: Når du bruker User Secrets under utvikling, kan du senere bruke miljøvariabler eller andre sikringsmekanismer i produksjon (f.eks. Azure Key Vault).

## Viktige begrensninger

* Kun for lokal utvikling: User Secrets er ikke designet for produksjon. Når applikasjonen distribueres til produksjon, bør du bruke sikre lagringsmekanismer som miljøvariabler, Azure Key Vault, AWS Secrets Manager, etc.
* Ikke synkronisert: Hemmelighetene er lagret lokalt på utviklerens maskin, og blir ikke synkronisert til andre maskiner eller kilder.

## Oppsummering

User Secrets er en praktisk måte å håndtere sensitive opplysninger på under lokal utvikling i ASP.NET Core. Ved å bruke User Secrets unngår du å lagre sensitive data i kildekoden og sikrer at sensitive opplysninger holdes utenfor kildekontrollsystemer. Det er en anbefalt løsning for utvikling, men for produksjon anbefales det å bruke mer avanserte løsninger som miljøvariabler eller hemmelighetshåndteringsverktøy som Azure Key Vault.
