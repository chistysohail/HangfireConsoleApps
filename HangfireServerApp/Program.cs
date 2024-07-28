using System;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;

namespace HangfireServerApp
{
    public class Program
    {
        private static readonly string connectionString = "Server=host.docker.internal,1434;Database=HangfireApps;User Id=sa;Password=YourNewStrong(!)Password;MultipleActiveResultSets=true;TrustServerCertificate=True;Connection Timeout=30;";

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddHangfire(configuration =>
                            configuration.UseSqlServerStorage(connectionString));
                        services.AddHangfireServer();
                    });

                    webBuilder.Configure(app =>
                    {
                        var options = new DashboardOptions
                        {
                            Authorization = new[]
                            {
                                new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                                {
                                    RequireSsl = false,
                                    SslRedirect = false,
                                    LoginCaseSensitive = true,
                                    Users = new[]
                                    {
                                        new BasicAuthAuthorizationUser
                                        {
                                            Login = "admin",
                                            PasswordClear = "password"
                                        }
                                    }
                                })
                            }
                        };

                        app.UseHangfireDashboard("/hangfire", options);
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapHangfireDashboard();
                        });
                    });
                });
    }
}
