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