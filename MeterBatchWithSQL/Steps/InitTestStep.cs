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