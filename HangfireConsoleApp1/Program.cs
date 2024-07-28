using System;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HangfireConsoleApp1
{
    class Program
    {
        private static readonly string connectionString = "Server=host.docker.internal,1434;Database=HangfireApps;User Id=sa;Password=YourNewStrong(!)Password;MultipleActiveResultSets=true;TrustServerCertificate=True;Connection Timeout=30;";

        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Set JobStorage.Current
            var jobStorage = new SqlServerStorage(connectionString);
            JobStorage.Current = jobStorage;

            // Enqueue the initial job
            EnqueueJob();

            // Schedule recurring job
            RecurringJob.AddOrUpdate("recurring-job", () => JobProcessorLib.JobProcessor.ProcessJob(1, Environment.MachineName), "*/5 * * * *");

            // Run the host
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHangfire(configuration =>
                        configuration.UseSqlServerStorage(connectionString));
                });

        public static void EnqueueJob()
        {
            BackgroundJob.Enqueue(() => JobProcessorLib.JobProcessor.ProcessJob(1, Environment.MachineName));
        }
    }
}
