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
}
