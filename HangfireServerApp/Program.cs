using System;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;

namespace HangfireServerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
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
                    services.AddHangfireServer();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.Configure((context, app) =>
                    {
                        // Get the configuration from the DI container
                        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

                        // Get the connection string
                        var connectionString = configuration.GetConnectionString("HangfireConnection");

                        // Read Hangfire settings from configuration
                        var hangfireSettings = configuration.GetSection("HangfireSettings");
                        var username = hangfireSettings.GetValue<string>("Username");
                        var password = hangfireSettings.GetValue<string>("Password");
                        var requireSsl = hangfireSettings.GetValue<bool>("RequireSsl");
                        var sslRedirect = hangfireSettings.GetValue<bool>("SslRedirect");
                        var loginCaseSensitive = hangfireSettings.GetValue<bool>("LoginCaseSensitive");

                        // Configure Hangfire to use the connection string
                        GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);

                        // Configure Hangfire dashboard with Basic Authentication
                        var options = new DashboardOptions
                        {
                            Authorization = new[]
                            {
                                new BasicAuthAuthorizationFilter(
                                    username: username,
                                    password: password,
                                    requireSsl: requireSsl,
                                    sslRedirect: sslRedirect,
                                    loginCaseSensitive: loginCaseSensitive)
                            }
                        };

                        // Configure Hangfire dashboard with AllowAllAuthorizationFilter
                        //var options = new DashboardOptions
                        //{
                        //    Authorization = new[] { new AllowAllAuthorizationFilter() }
                        //};

                        app.UseHangfireDashboard("/hangfire", options);

                        // Enqueue a test job
                        BackgroundJob.Enqueue(() => Console.WriteLine("Hangfire is up and running!"));

                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapHangfireDashboard();
                        });
                    });
                });
    }
}


public class AllowAllAuthorizationFilter : Hangfire.Dashboard.IDashboardAuthorizationFilter
    {
        public bool Authorize(Hangfire.Dashboard.DashboardContext context)
        {
            return true; // Allow all requests
        }
    }


public class BasicAuthAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly string _username;
        private readonly string _password;
        private readonly bool _requireSsl;
        private readonly bool _sslRedirect;
        private readonly bool _loginCaseSensitive;

        public BasicAuthAuthorizationFilter(string username, string password, bool requireSsl = false, bool sslRedirect = false, bool loginCaseSensitive = true)
        {
            _username = username;
            _password = password;
            _requireSsl = requireSsl;
            _sslRedirect = sslRedirect;
            _loginCaseSensitive = loginCaseSensitive;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            if (_requireSsl && !httpContext.Request.IsHttps)
            {
                if (_sslRedirect)
                {
                    var withHttps = "https://" + httpContext.Request.Host + httpContext.Request.Path;
                    httpContext.Response.Redirect(withHttps);
                }

                return false;
            }

            var authHeader = httpContext.Request.Headers["Authorization"];

            if (authHeader.ToString().StartsWith("Basic "))
            {
                var encodedUsernamePassword = authHeader.ToString().Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];

                if ((_loginCaseSensitive ? username == _username : username.Equals(_username, StringComparison.OrdinalIgnoreCase)) && password == _password)
                {
                    return true;
                }
            }

            // Return 401 if the credentials are incorrect or not provided
            httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
            httpContext.Response.StatusCode = 401;
            return false;
        }
    }


