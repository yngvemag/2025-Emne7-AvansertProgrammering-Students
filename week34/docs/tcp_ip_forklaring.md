# TCP/IP – Grunnleggende forklaring

Når datamaskiner og enheter kommuniserer over internett eller et nettverk, skjer det ved hjelp av **TCP/IP-modellen**.  
Denne modellen består av flere lag som samarbeider for å sørge for at data kan sendes fra en enhet til en annen på en trygg og strukturert måte.

---

## Hva er IP?
**IP (Internet Protocol)**  
- Gjør det mulig å koble sammen forskjellige underliggende nett til et felles logisk nettverk.  
- De underliggende nettene kan være basert på ulike teknologier (f.eks. Wi-Fi, Ethernet, 4G/5G).  
- IP sørger for at alle enheter får en unik adresse (**IP-adresse**), slik at data vet hvor de skal sendes.  

👉 Kort sagt: IP fungerer som **adresse-systemet** på internett. Det tilsvarer adressen på huset ditt, slik at posten vet hvor den skal leveres.  

---

## Hva er TCP?
**TCP (Transmission Control Protocol)**  
- Sikrer pålitelig transport av datasignaler mellom brukerprogrammer som kommuniserer over nettet.  
- Deler opp meldinger i mindre biter kalt **pakker**, og sørger for at de sendes i riktig rekkefølge.  
- Håndterer gjenoppbygging av data på mottakersiden, og sjekker at ingen pakker mangler.  

👉 Kort sagt: TCP fungerer som **postbudet** som sørger for at brevene kommer frem, i riktig rekkefølge og uten feil.  

---

## Hvordan samarbeider TCP og IP?
- **IP** sørger for at pakkene finner veien til riktig mottaker (routing).  
- **TCP** sørger for at hele meldingen kommer frem riktig og i riktig rekkefølge.  

Eksempel: Når du sender en e-post eller besøker en nettside:  
1. TCP deler meldingen opp i små pakker.  
2. IP setter på avsender- og mottakeradresse.  
3. Pakkene sendes gjennom nettet og kan ta ulike ruter.  
4. På mottakersiden setter TCP pakkene sammen igjen i riktig rekkefølge.  

---
<div style="page-break-after: always;"></div>

## TCP/IP-modellen – Lagene
TCP/IP-modellen består av fire lag:

1. **Application Layer (Applikasjonslaget)**  
   - Her finner vi tjenester som **HTTP, FTP, SMTP** (nettleser, e-post osv.).  

2. **Transport Layer (Transportlaget)**  
   - Her finner vi **TCP og UDP**.  
   - Sørger for kommunikasjon mellom programmer, og pålitelig eller rask levering.  

3. **Internet Layer (Internettlaget)**  
   - Her finner vi **IP (Internet Protocol)**.  
   - Håndterer adressering og ruting av pakker.  

4. **Network Access Layer (Nettverkstilgangslaget)**  
   - Her finner vi fysiske nettverksprotokoller, f.eks. **Ethernet og Wi-Fi**.  
   - Bruker **MAC-adresser** for å sende data på det lokale nettverket.  

---

## Kort oppsummert
- **IP** = Adressene og rutingen som sørger for at data kommer frem til riktig sted.  
- **TCP** = Pålitelig transport som sørger for at data kommer frem i riktig rekkefølge uten feil.  
- Sammen gjør TCP/IP det mulig å kommunisere sømløst over hele verden.  

