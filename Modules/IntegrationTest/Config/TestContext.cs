using ConstruaApp.Api;
using Infra.CrossCutting.BlobStorage;
using Infra.CrossCutting.BlobStorage.Interfaces;
using Infra.Data.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net.Http;
using Serilog;

namespace IntegrationTest.Config
{
    public class TestContext
    {
        public HttpClient Client { get; set; }
        private TestServer _server;
        public TestContext()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<IntegrationTestStartup>()
                .UseSerilog()
                .UseConfiguration(ConnectionString.GetConnection());
            SetupClient(webHostBuilder);
        }
        private void SetupClient(IWebHostBuilder builder)
        {

            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkMySql()
                    .BuildServiceProvider();

                // Add a database context (AppDbContext) using an in-memory database for testing.
                var configuration = ConnectionString.GetConnection();

                services.AddDbContext<MySQLCoreContext>(options => options.UseMySql(configuration["ConnectionStrings:MySqlCore"]));      
                
                services.AddDbContext<SQLiteCoreContext>(options => options.UseSqlite(ConnectionString.GetConnectionSqlite()));

                var sp = services.BuildServiceProvider();               
            });           

            _server = new TestServer(builder);
            Client = _server.CreateClient();
        }
    }    
}

