using Application.Interfaces;
using ConstruaApp.Api.Jobs.Interfaces;
using Domain.Input.RDStation;
using Infra.CrossCutting.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace ConstruaApp.Api.Jobs
{
    public class ExecuteJobSendRDConversions: IExecuteJobSendRDConversions
        {
        private readonly IRDApplication _rdApplication;
        private IWebHostEnvironment _hostingEnvironment;
        private IConfiguration _configuration;
        private readonly ILogger<ExecuteJobSendRDConversions> _logger;

        public ExecuteJobSendRDConversions(
            IRDApplication rdApplication, 
            IWebHostEnvironment environment, 
            IConfiguration configuration, 
            ILogger<ExecuteJobSendRDConversions> logger)
        {
            _rdApplication = rdApplication;
            _hostingEnvironment = environment;
            _configuration = configuration;
            _logger = logger;
        }
        
        public async Task Execute()
        {
            var input = new RDStationInput() {
                UrlBase = _configuration.GetSection("RDStation:UrlBase").Value,
                ApiSecret = _configuration.GetSection("RDStation:ApiSecret").Value
                };
            _logger.LogInformation($"Init process {nameof(Execute)}");
            await _rdApplication.CreateRDStationConversion(input);
        }
    }
}
