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

<h3>Batch/MeterBatch.cs</h3>

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using MeterBatchApp.Models;

public class MeterBatch
{
    private readonly List<ITestStep> _steps = new();

    public void AddStep(ITestStep step)
    {
        _steps.Add(step);
    }

    public async Task<List<TestStepResult>> RunAsync()
    {
        var results = new List<TestStepResult>();

        foreach (var step in _steps)
        {
            var result = await step.ExecuteAsync();
            results.Add(result);
        }

        return results;
    }
}
```

<h3>Models/TestStepResult.cs</h3>

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

<h3>Services/DbService.cs</h3>

```csharp
using Dapper;
using MeterBatchApp.Models;
using MySqlConnector;

namespace MeterBatchApp.Services
{
    public class DbService
    {
        private readonly string _connectionString;

        public DbService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task SaveResultsAsync(TestStepResult[] results)
        {
            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            foreach (var result in results)
            {
                await conn.ExecuteAsync(
                    "INSERT INTO test_results (step_name, status, timestamp, info) VALUES (@StepName, @Status, @Timestamp, @AdditionalInfo)",
                    result
                );
            }
        }
    }

```

<h3>Steps/FinalizeTestStep.cs</h3>

```csharp
using MeterBatchApp.Models;

public class FinalizeTestStep : ITestStep
{
    private readonly Func<TestStepResult[], Task> _saveAction;
    private readonly TestStepResult[] _results;

    public FinalizeTestStep(TestStepResult[] results, Func<TestStepResult[], Task> saveAction)
    {
        _results = results;
        _saveAction = saveAction;
    }

    public async Task<TestStepResult> ExecuteAsync()
    {
        await _saveAction(_results);
        return new TestStepResult
        {
            StepName = "Speichern",
            Status = "Fertig",
            AdditionalInfo = "DB-Speicherung erfolgreich"
        };
    }
}
```

<h3>Steps/InitTestStep.cs</h3>

```csharp
using System;
using System.Threading.Tasks;
using MeterBatchApp.Models;

public class InitTestStep : ITestStep
{
    public async Task<TestStepResult> ExecuteAsync()
    {
        await Task.Delay(1000);
        return new TestStepResult
        {
            StepName = "Initialisierung",
            Status = "Fertig",
            AdditionalInfo = "Init OK"
        };
    }
}
```

<h3>Steps/ITestStep.cs</h3>

```charp
using System.Threading.Tasks;
using MeterBatchApp.Models;

public interface ITestStep
{
    Task<TestStepResult> ExecuteAsync();
}
```

<h3>Steps/MeasureFlowTestStep.cs</h3>

```csharp
using MeterBatchApp.Models;

public class MeasureFlowTestStep : ITestStep
{
    public async Task<TestStepResult> ExecuteAsync()
    {
        await Task.Delay(1000);
        return new TestStepResult
        {
            StepName = "Durchfluss messen",
            Status = "Fertig",
            AdditionalInfo = "10 l/min"
        };
    }
}
```

<h3>Program.cs</h3>

```csharp
using System;
using System.Threading.Tasks;
using MeterBatchApp.Models;
using MeterBatchApp.Services;

class Program
{
    static async Task Main()
    {
        var connectionString = "server=127.0.0.1;user=root;password=root;database=meter_batch_db";

        var dbService = new DbService(connectionString);
        var batch = new MeterBatch();

        var results = new TestStepResult[2];  // Platz für die ersten beiden Schritte

        batch.AddStep(new InitTestStep());
        batch.AddStep(new MeasureFlowTestStep());

        // Schritte ausführen
        var stepResults = await batch.RunAsync();

        // Konvertieren zu Array für FinalizeTestStep
        results = stepResults.ToArray();

        // FinalizeStep einfügen und ausführen
        var finalizeStep = new FinalizeTestStep(results, async res => await dbService.SaveResultsAsync(res));
        var finalizeResult = await finalizeStep.ExecuteAsync();

        Console.WriteLine("Alle Schritte abgeschlossen.");
    }
}

```




