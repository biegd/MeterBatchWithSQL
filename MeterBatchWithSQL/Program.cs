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
