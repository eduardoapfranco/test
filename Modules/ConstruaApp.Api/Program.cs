using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Serilog.Core;
using Serilog.Events;

namespace ConstruaApp.Api
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        private static string _env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        public static void Main(string[] args)
        {
            if (String.IsNullOrWhiteSpace(_env))
                throw new Exception("Empty or unset ASPNETCORE_ENVIRONMENT environment variable");

            ConfigureLogging();
            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            try
            {
                return Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingCOntext, config) =>
                {
                    config.AddJsonFile("appsettings.json");
                    if (_env != null && _env != "")
                    {
                        config.AddJsonFile($"appsettings.{_env}.json");
                    }
                })
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               }).UseSerilog();
            }
            catch (Exception ex)
            {
                Log.Fatal($"Failed to start {Assembly.GetExecutingAssembly().GetName().Name}", ex);
                throw;
            }         
        }

        private static void ConfigureLogging()
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            if (_env != null && _env != "")
            {
                configuration.AddJsonFile($"appsettings.{_env}.json");
            }

            var appLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Debug);
            var netLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Error);
            var systemLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Error);


            var resultConfiguration = configuration.Build();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(ConfigureElasticSink(resultConfiguration, _env))
                .Enrich.WithProperty("Environment", _env)
                .ReadFrom.Configuration(resultConfiguration)
                .MinimumLevel.ControlledBy(appLevelSwitch)
                .MinimumLevel.Override("Microsoft", netLevelSwitch)
                .MinimumLevel.Override("System", systemLevelSwitch)
                .CreateLogger();
        }

        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"kibana_sample_data_logs{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                ModifyConnectionSettings = x => x.BasicAuthentication("construa9876", "wM2jQ8vR4qH5zM9b")
            };
        }

    }
}
