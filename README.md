<h1>Meter Batch With SQL</h1>

<h2>Aufgabenstellung</h2>

Ich möchte eine Meter Batch programmieren, die einezelne Testschritte (z.B. bei der Produktion eines Wasserzählers)
durchführt und die Ergebnisse am Ende in eine SQL-Datenbank schreibt. Dabei sollen keine globalen Variablen ver-
wendet werden.

<h3>Prinzip</h3>

- Jeder Testschritt liefert ein **Ereignisobjekt** zurück (z.B. TestStepResult).
- Der Batch-Runner (z.B. MeterBatch) sammelt diese Ergebnisse in einer lokalen Liste
- Der finalie Schritt (oder dein DataService) schreibt diese Liste in die MySQL-Datenbank

<h2>Beispielstruktur</h2>

<h3>Ordnerstruktur</h3>

![grafik](https://github.com/user-attachments/assets/b306ba2a-e0ae-417e-adf9-de1781ca22f3)

<h3>TestStepResult.cs</h3>

```csharp
namespace MeterBatchApp.Models
{
    public class TestStepResult
    {
        public string StepName { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string AdditionalInfo { get; set; }
    }
}
```

<h3>ITestStep</h3>

```charp
using System.Threading.Tasks;

public interface ITestStep
{
    Task<TestStepResult> ExecuteAsync();
}
```

