using Application.AppServices.DbMobileApplication.Input;
using Application.AppServices.DbMobileApplication.ViewModel;
using Application.Interfaces;
using ConstruaApp.Api.Jobs;
using FizzWare.NBuilder;
using Infra.CrossCutting.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Jobs
{
    public class ExecutarJobCreateDBMobileTest
    {
        private Mock<IDbMobileApplication> _dbMobileApplicationMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private Mock<IConfiguration> _configurationMock;
        private ExecuteJobCreateDBMobile _job;
        private Mock<ILogger<ExecuteJobCreateDBMobile>> _loggerMock;

        public ExecutarJobCreateDBMobileTest( )
        {
            _dbMobileApplicationMock = new Mock<IDbMobileApplication>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<ExecuteJobCreateDBMobile>>();
            _job = new ExecuteJobCreateDBMobile(_dbMobileApplicationMock.Object, _webHostEnvironmentMock.Object, _configurationMock.Object, _loggerMock.Object);
        }

        [Fact(DisplayName = "Should execute job with success")]
        [Trait("[WebApi.Jobs]-ExecuteJobCreateDBMobile", "Jobs-Execute")]
        public async Task ShouldExecuteJobWithSuccess()
        {
            var dbViewModel = Builder<DbMobileViewModel>.CreateNew().Build();
            var input = new DbMobileInput() { Secret = AuthSettings.SecretGenerateDB };

            var path = Path.Combine(Environment.CurrentDirectory, "appsettings.json");

            _dbMobileApplicationMock.Setup(x => x.CreateDBMobileAsync(It.IsAny<DbMobileInput>(), path, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(dbViewModel);
            _webHostEnvironmentMock.Setup(x => x.ContentRootPath).Returns(path);

            Func<Task> task = () => _job.Execute();
            await task.Invoke();

            Assert.NotNull(task);
  
        }


    }
}
