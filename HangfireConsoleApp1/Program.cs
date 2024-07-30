using System;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HangfireConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Get the connection string from the DI container
            var connectionString = host.Services.GetRequiredService<IConfiguration>().GetConnectionString("HangfireConnection");

            // Set JobStorage.Current
            var jobStorage = new SqlServerStorage(connectionString);
            JobStorage.Current = jobStorage;

            // Enqueue the initial job
            EnqueueJob(connectionString);

            // Schedule recurring job
            RecurringJob.AddOrUpdate("recurring-job1", () => JobProcessorLib.JobProcessor.ProcessJob(1, Environment.MachineName, connectionString), "*/5 * * * *");

            // Run the host
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var env = hostContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                          .AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Register the configuration
                    services.AddSingleton(hostContext.Configuration);

                    // Register Hangfire with the connection string
                    var connectionString = hostContext.Configuration.GetConnectionString("HangfireConnection");
                    services.AddHangfire(configuration =>
                        configuration.UseSqlServerStorage(connectionString));
                });

        public static void EnqueueJob(string connectionString)
        {
            BackgroundJob.Enqueue(() => JobProcessorLib.JobProcessor.ProcessJob(1, Environment.MachineName, connectionString));
        }
    }
}
