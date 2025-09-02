# ADO.NET Oversikt

## Hva er ADO.NET?
- **Betydning**: ADO.NET står for "ActiveX Data Objects for .NET".
- **Funksjon**: Det er en del av .NET-rammeverket som gir et grensesnitt mellom programmer og databaser, og gjør det mulig å utføre CRUD-operasjoner (Create, Read, Update, Delete) mot ulike databaser.
- **Støttede databaser**: 
  - SQL Server
  - MySQL
  - Oracle
- **Bruk**: ADO.NET er en av de mest brukte databaseteknologiene i .NET-applikasjoner.

## Kjernekomponenter i ADO.NET
- **Connection**: Håndterer tilkoblingen til databasen.
- **Command**: Representerer SQL-spørringer eller lagrede prosedyrer som skal utføres i databasen.
- **DataReader**: Gir en rask, fremover-bare strøm av data fra en spørring.
- **DataAdapter**: Fungerer som en bro mellom DataSet og databasen for å hente og lagre data.
- **DataSet**: Holder data i minnet i en tabellform og kan inneholde flere tabeller og relasjoner mellom dem.

## Fordeler ved ADO.NET
- **Uavhengig av Databasetype**: Fungerer med forskjellige typer databaser med minimal endring i koden.
- **Sikkerhet**: Innebygd støtte for sikre tilkoblinger og parametriserte spørringer, noe som reduserer risikoen for SQL-injeksjon.
- **Skalerbarhet**: Kan håndtere en rekke forskjellige databaseløsninger og skalerer godt fra små til store applikasjoner.
- **Funksjonsrikt**: Tilbyr en rekke funksjoner for databehandling, inkludert støtte for transaksjoner, flere resultattsett, lagrede prosedyrer, osv.

## Ulemper ved ADO.NET
- **Læringskurve**: Kan være komplisert å forstå og implementere effektivt.
- **Manuell Håndtering**: Krever mer manuell kode for databehandling sammenlignet med ORM-løsninger som Entity Framework.

## Når skal man bruke ADO.NET?
- Når du trenger finjustert kontroll over databasetilkoblingen og spørringene.
- Når du arbeider med databaser som ikke har sterk ORM-støtte.
- Når ytelse er en kritisk faktor, og du vil unngå overhead som følger med ORMs (Object-Relational Mappers).
