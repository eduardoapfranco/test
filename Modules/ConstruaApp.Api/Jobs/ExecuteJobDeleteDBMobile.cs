using Application.AppServices.DbMobileApplication.Input;
using Application.Interfaces;
using ConstruaApp.Api.Jobs.Interfaces;
using Infra.CrossCutting.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ConstruaApp.Api.Jobs
{
    public class ExecuteJobDeleteDBMobile : IExecuteJobDeleteDBMobile
    {
        private readonly IDbMobileApplication _dbMobileApplication;
        private IWebHostEnvironment _hostingEnvironment;
        private IConfiguration _configuration;
        private readonly ILogger<ExecuteJobDeleteDBMobile> _logger;

        public ExecuteJobDeleteDBMobile(
            IDbMobileApplication dbMobileApplication, 
            IWebHostEnvironment environment, 
            IConfiguration configuration, 
            ILogger<ExecuteJobDeleteDBMobile> logger)
        {
            _dbMobileApplication = dbMobileApplication;
            _hostingEnvironment = environment;
            _configuration = configuration;
            _logger = logger;
        }
        
        public async Task Execute()
        {
            var input = new DbMobileInput() { Secret = AuthSettings.SecretGenerateDB };
            _logger.LogInformation($"Init process delete db mobiles {nameof(Execute)}");
            var folder = _configuration.GetConnectionString("FolderDbMobile");
            _logger.LogInformation($"Delete db mobiles: Folder {folder}");
            await _dbMobileApplication.DeleteDBMobileAsync(folder, false);
            await _dbMobileApplication.DeleteDBMobileAsync(folder, true);
        }
    }
}
