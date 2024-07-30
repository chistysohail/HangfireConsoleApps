using System;
using Hangfire;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace JobProcessorLib
{
    public class JobProcessor
    {
        [AutomaticRetry(Attempts = 3)]
        public static void ProcessJob(int jobId, string workerName, string connectionString)
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
