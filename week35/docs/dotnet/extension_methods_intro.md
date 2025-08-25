
# Introduksjon til Extension Methods i C#

## Hva er Extension Methods?
**Extension methods** i C# gjør det mulig å "utvide" eksisterende klasser eller grensesnitt med nye metoder **uten** å endre den opprinnelige koden eller lage en ny avledet klasse.  
Dette er spesielt nyttig når du vil legge til funksjonalitet i tredjepartsbiblioteker eller .NET-klasser.

**Nøkkelpunkter:**
- Extension methods er **statiske metoder** i en **statisk klasse**.
- Den første parameteren bruker nøkkelordet `this` foran typen du vil utvide.
- De kalles som om de var vanlige instansmetoder på objektet.

---

## Hvordan lage en Extension Method?

### 1. Opprett en statisk klasse
```csharp
public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }
}
```

### 2. Bruk metoden som en del av klassen
```csharp
class Program
{
    static void Main()
    {
        string text = null;
        bool result = text.IsNullOrEmpty(); // Kaller extension method som om den var en instansmetode
        Console.WriteLine(result); // True
    }
}
```

Her har vi lagt til `IsNullOrEmpty()`-metoden på `string`-typen uten å endre `System.String`.

---

## Viktige regler
- Extension methods **må være i en statisk klasse**.
- De **må være statiske metoder**.
- Den første parameteren **må bruke `this`** foran typen som utvides.
- Husk å importere navnerommet (`using`-direktiv) der extension-klassen ligger.

---

## Eksempel: Utvid `int`-typen
```csharp
public static class IntExtensions
{
    public static bool IsEven(this int number)
    {
        return number % 2 == 0;
    }
}

// Bruk
int x = 4;
Console.WriteLine(x.IsEven()); // True
```

---

## Bruksområder
- Legge til hjelpefunksjoner på innebygde typer (`string`, `int`, etc.).
- Legge til funksjonalitet i tredjepartsbiblioteker uten å endre koden.
- Gjøre koden mer lesbar og uttrykksfull.

---

## Eksempel: Kombinere med LINQ
Mange av LINQ-metodene (`Where`, `Select`, `OrderBy`) er extension methods for `IEnumerable<T>`.
```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5 };
var evenNumbers = numbers.Where(n => n.IsEven());
```

Her bruker vi vår egen `IsEven()` sammen med LINQ.

---

## Oppsummering
- Extension methods lar deg legge til funksjonalitet uten å endre eksisterende klasser.
- De gjør koden renere og mer gjenbrukbar.
- Husk `static class`, `static method` og `this` foran parameteren.

Extension methods er et kraftig verktøy for å gjøre koden din mer fleksibel og uttrykksfull!
