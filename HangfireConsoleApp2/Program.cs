using System;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HangfireConsoleApp2
{
    class Program
    {
        private static readonly string connectionString = "Server=host.docker.internal,1434;Database=HangfireApps;User Id=sa;Password=YourNewStrong(!)Password;MultipleActiveResultSets=true;TrustServerCertificate=True;Connection Timeout=30;";

        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            EnqueueJob();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHangfire(configuration =>
                        configuration.UseSqlServerStorage(connectionString));
                });

        public static void EnqueueJob()
        {
            BackgroundJob.Enqueue(() => JobProcessorLib.JobProcessor.ProcessJob(2, Environment.MachineName));
        }
    }
}
