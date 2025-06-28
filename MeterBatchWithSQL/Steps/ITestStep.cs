using System.Threading.Tasks;
using MeterBatchApp.Models;

public interface ITestStep
{
    Task<TestStepResult> ExecuteAsync();
}
