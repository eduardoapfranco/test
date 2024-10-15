using System;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Extensions.Hosting.Internal;
using System.Collections.Generic;

namespace IntegrationTest.Config
{
    [ExcludeFromCodeCoverage]
    public static class ConnectionString
    {
        public static IConfiguration GetConnection()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT_CONSTRUA_TEST");
            var appsettings = (environment != null && environment != "") ? $"appsettings.{environment}.json" : $"appsettings.test_local.json" ;

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(appsettings)
                .AddEnvironmentVariables()
                .Build();

            return config;
        }

        public static string GetConnectionSqlite()
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            var connection = codeBase.Replace("bin/Debug/netcoreapp3.1/IntegrationTest.dll", "Db/construa.bd");
            connection = "Data Source=" + connection;
            return connection;
        }
    }
}
