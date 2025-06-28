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
