﻿using System;
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

            // Enqueue job after Hangfire has been initialized
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var jobStorage = services.GetRequiredService<JobStorage>();
                JobStorage.Current = jobStorage;

                using (var server = new BackgroundJobServer(new BackgroundJobServerOptions(), jobStorage))
                {
                    EnqueueJob();
                    host.Run();
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHangfire(configuration =>
                        configuration.UseSqlServerStorage(connectionString));
                    services.AddHangfireServer();
                });

        public static void EnqueueJob()
        {
            BackgroundJob.Enqueue(() => JobProcessorLib.JobProcessor.ProcessJob(2, Environment.MachineName));
        }
    }
}
