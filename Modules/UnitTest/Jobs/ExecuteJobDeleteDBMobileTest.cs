using Application.Interfaces;
using ConstruaApp.Api.Jobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Jobs
{
    public class ExecuteJobDeleteDBMobileTest
    {
        private Mock<IDbMobileApplication> _dbMobileApplicationMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private Mock<IConfiguration> _configurationMock;
        private ExecuteJobDeleteDBMobile _job;
        private Mock<ILogger<ExecuteJobDeleteDBMobile>> _loggerMock;

        public ExecuteJobDeleteDBMobileTest( )
        {
            _dbMobileApplicationMock = new Mock<IDbMobileApplication>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<ExecuteJobDeleteDBMobile>>();
            _job = new ExecuteJobDeleteDBMobile(_dbMobileApplicationMock.Object, _webHostEnvironmentMock.Object, _configurationMock.Object, _loggerMock.Object);
        }

        [Fact(DisplayName = "Should execute job with success")]
        [Trait("[WebApi.Jobs]-ExecuteJobDeleteDBMobile", "Jobs-Execute")]
        public async Task ShouldExecuteJobWithSuccess()
        {

            _dbMobileApplicationMock.Setup(x => x.DeleteDBMobileAsync(It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

            Func<Task> task = () => _job.Execute();
            await task.Invoke();

            Assert.NotNull(task);
        }
    }
}
