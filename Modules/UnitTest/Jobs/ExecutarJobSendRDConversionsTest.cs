using Application.Interfaces;
using ConstruaApp.Api.Jobs;
using Domain.Input.RDStation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Jobs
    {
    public class ExecutarJobSendRDConversionsTest
    {
        private Mock<IRDApplication> _rdApplicationMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private Mock<IConfiguration> _configurationMock;
        private ExecuteJobSendRDConversions _job;
        private Mock<ILogger<ExecuteJobSendRDConversions>> _loggerMock;

        public ExecutarJobSendRDConversionsTest( )
        {
            _rdApplicationMock = new Mock<IRDApplication>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<ExecuteJobSendRDConversions>>();
            _job = new ExecuteJobSendRDConversions(_rdApplicationMock.Object, _webHostEnvironmentMock.Object, _configurationMock.Object, _loggerMock.Object);
        }

        [Fact(DisplayName = "Should execute job with success")]
        [Trait("[WebApi.Jobs]-ExecuteJobSendRDConversions", "Jobs-Execute")]
        public async Task ShouldExecuteJobWithSuccess()
        {
            //// arrange
            //var rdInput = new RDStationInput()
            //    {
            //    UrlBase = "https://urlbase.construa.app",
            //    ApiSecret = "apisecret-construa"
            //    };

            //var listOfEventsUuids = new List<string>();
            //listOfEventsUuids.Add("event-uuid");

            //_rdApplicationMock.Setup(x => x.CreateRDStationConversion(It.IsAny<RDStationInput>())).ReturnsAsync(listOfEventsUuids);
            //var path = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
            //_webHostEnvironmentMock.Setup(x => x.ContentRootPath).Returns(path);

            //Func<Task> task = () => _job.Execute();

            ////act
            //await task.Invoke();

            ////assert
            //Assert.NotNull(task);

            }


    }
}
