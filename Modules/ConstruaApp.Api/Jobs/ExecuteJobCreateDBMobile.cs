using Application.AppServices.DbMobileApplication.Input;
using Application.Interfaces;
using ConstruaApp.Api.Jobs.Interfaces;
using Infra.CrossCutting.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace ConstruaApp.Api.Jobs
{
    public class ExecuteJobCreateDBMobile: IExecuteJobCreateDBMobile
    {
        private readonly IDbMobileApplication _dbMobileApplication;
        private IWebHostEnvironment _hostingEnvironment;
        private IConfiguration _configuration;
        private readonly ILogger<ExecuteJobCreateDBMobile> _logger;

        public ExecuteJobCreateDBMobile(
            IDbMobileApplication dbMobileApplication, 
            IWebHostEnvironment environment, 
            IConfiguration configuration, 
            ILogger<ExecuteJobCreateDBMobile> logger)
        {
            _dbMobileApplication = dbMobileApplication;
            _hostingEnvironment = environment;
            _configuration = configuration;
            _logger = logger;
        }
        
        public async Task Execute()
        {
            var input = new DbMobileInput() { Secret = AuthSettings.SecretGenerateDB };
            _logger.LogInformation($"Init process {nameof(Execute)}");
            var path = Path.Combine(_hostingEnvironment.ContentRootPath, "construa.bd");
            _logger.LogInformation($"Folder process {nameof(Execute)} with path: {path}");
            await _dbMobileApplication.CreateDBMobileAsync(input, path, _configuration.GetConnectionString("FolderDbMobile"), _hostingEnvironment.ContentRootPath);
        }
    }
}
