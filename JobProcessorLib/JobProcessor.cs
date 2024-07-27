using System;
using Hangfire;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace JobProcessorLib
{
    public class JobProcessor
    {
        private static readonly string connectionString = "Server=host.docker.internal,1434;Database=HangfireApps;User Id=sa;Password=YourNewStrong(!)Password;MultipleActiveResultSets=true;TrustServerCertificate=True;Connection Timeout=30;";
        //private static readonly string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=HangfireApps;Integrated Security=True;";
        [AutomaticRetry(Attempts = 3)]
        public static void ProcessJob(int jobId, string workerName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("UPDATE Jobs SET Status = @Status, ProcessedBy = @ProcessedBy WHERE JobId = @JobId", connection))
                {
                    command.Parameters.Add(new SqlParameter("@Status", "Processing"));
                    command.Parameters.Add(new SqlParameter("@ProcessedBy", workerName));
                    command.Parameters.Add(new SqlParameter("@JobId", jobId));
                    command.ExecuteNonQuery();
                }

                Console.WriteLine($"Processing job {jobId} by worker {workerName}");

                // Simulate job processing
                Task.Delay(10000).Wait();

                using (SqlCommand command = new SqlCommand("UPDATE Jobs SET Status = @Status WHERE JobId = @JobId", connection))
                {
                    command.Parameters.Add(new SqlParameter("@Status", "Completed"));
                    command.Parameters.Add(new SqlParameter("@JobId", jobId));
                    command.ExecuteNonQuery();
                }

                Console.WriteLine($"Completed job {jobId} by worker {workerName}");
            }
        }
    }
}
