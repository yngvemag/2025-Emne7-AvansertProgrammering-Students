# Kommunikasjon mellom klient og server

Når vi bruker internett, skjer kommunikasjonen mellom **klient** (for eksempel en nettleser) og en **server** (der nettsiden eller tjenesten ligger).  
- **Klienten** sender en **request (forespørsel)**.  
- **Serveren** svarer med en **response (respons)**.  

Denne kommunikasjonen går som oftest via **HTTP (Hypertext Transfer Protocol)**, som er et sett med regler for hvordan data sendes og mottas på nettet.

---

## HTTP – ulike typer kommunikasjon
Det finnes forskjellige måter å kommunisere på med HTTP:

1. **SOAP (Simple Object Access Protocol)**  
   - En eldre webservice-protokoll.  
   - Bruker **XML** som format for å sende data.  
   - Var mye brukt tidligere, spesielt i større bedrifter og systemer.  

2. **REST API (Representational State Transfer)**  
   - Den mest brukte metoden i dag.  
   - Basert på JSON (JavaScript Object Notation), som er lett å lese både for mennesker og maskiner.  
   - Kan også bruke XML, men JSON er mest vanlig.  
   - Eksempel: Når en app på mobilen henter værdata fra en server.  

---

## HTML, CSS og JavaScript – selve nettsiden
Når en server sender en nettside tilbake til klienten, består den av flere byggesteiner:

- **HTML (HyperText Markup Language)**  
  - Beskriver strukturen på en nettside (overskrifter, avsnitt, lenker, bilder osv.).  
  - Kan sammenlignes med "skjelettet" på en nettside.  

- **CSS (Cascading Style Sheets)**  
  - Bestemmer hvordan nettsiden ser ut (farger, skrifttyper, layout).  
  - Gir design og utseende.  

- **JavaScript**  
  - Gir interaktivitet (knapper som reagerer, menyer som åpner seg, dynamisk innhold).  
  - Gjør nettsider "levende".  

---

# Historie: Hva skjer når du åpner en nettside?

## 1. Brukeren starter
Du sitter med en PC eller mobil og åpner nettleseren (f.eks. Chrome). Du skriver inn:  
```
www.turnonvpn.org
```
👉 **Nettleseren din er klienten.**

---

## 2. Nettleseren spør etter adressen
Nettleseren forstår ikke "www.turnonvpn.org" direkte – den må finne ut hvilken **IP-adresse** dette domenet peker til.  
Den sender derfor en forespørsel til en **DNS-server** (Internettets "telefonkatalog").  

👉 DNS-serveren svarer:  
```
www.turnonvpn.org = 198.61.190.243
```

---

## 3. Nettleseren sender en forespørsel
Nå vet nettleseren IP-adressen. Den sender en **HTTP-request** til serveren på denne adressen:  
```
GET / HTTP/1.1
Host: www.turnonvpn.org
```

👉 Dette er en beskjed som betyr:  
"Hei server! Jeg (klienten) ønsker å hente forsiden til nettsiden."

---

## 4. Serveren svarer tilbake
Serveren mottar forespørselen og sender en **HTTP-response** tilbake. Den inneholder:  

- **HTML** (struktur og innhold)  
- **CSS** (farger, layout, stil)  
- **JavaScript** (interaktivitet og dynamiske funksjoner)  

---
<div style="page-break-after: always;"></div>

## 5. Nettleseren setter sammen alt
Nettleseren din tar imot HTML, CSS og JavaScript, setter alt sammen – og viser deg den ferdige nettsiden på skjermen.  

👉 Du som bruker ser bare sluttresultatet, men bak kulissene har det skjedd masse kommunikasjon.  

---

## 6. Hva med API-er?
Hvis nettsiden også skal hente data (f.eks. værmelding, kart eller nyheter), så snakker den ofte med en server via **REST API**.  
- Her sendes data frem og tilbake i **JSON-format** (eller noen ganger XML).  
- Eksempel:  
  ```json
  {
    "temp": 12,
    "weather": "sunny"
  }
  ```

👉 Dette brukes gjerne av apper eller andre systemer – ikke direkte av mennesker.  

---

# Kort oppsummert som fortelling:
- Klienten spør DNS om veien.  
- DNS viser til riktig server (IP-adresse).  
- Klienten sender en HTTP-forespørsel.  
- Serveren svarer med HTML, CSS og JavaScript.  
- Nettleseren setter det sammen og viser nettsiden.  
- API-er brukes når systemer skal utveksle data i JSON eller XML.  
