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