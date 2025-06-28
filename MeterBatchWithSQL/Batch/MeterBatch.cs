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
