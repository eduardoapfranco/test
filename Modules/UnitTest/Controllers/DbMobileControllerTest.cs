using Application.AppServices.DbMobileApplication.Input;
using Application.AppServices.DbMobileApplication.ViewModel;
using Application.Interfaces;
using ConstruaApp.Api.Controllers;
using FizzWare.NBuilder;
using Infra.CrossCutting.Auth;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Handler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Controllers
    {
    public class DbMobileControllerTest
        {
        private DomainNotificationHandler _notificationHandler;
        private Mock<IDbMobileApplication> _dbMobileApplicationMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private Mock<IConfiguration> _configurationMock;
        private DbMobileController _controller;

        public DbMobileControllerTest()
            {
            _dbMobileApplicationMock = new Mock<IDbMobileApplication>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _configurationMock = new Mock<IConfiguration>();
            _notificationHandler = new DomainNotificationHandler();
            _controller = new DbMobileController(_notificationHandler, _dbMobileApplicationMock.Object, _webHostEnvironmentMock.Object, _configurationMock.Object);
            }

        [Fact(DisplayName = "Should return create db with success post async")]
        [Trait("[WebApi.Controllers]-DbMobileController", "Controllers-PostAsync")]
        public async Task ShouldReturnCreateDbWithSuccessPostAsync()
            {
            var dbViewModel = Builder<DbMobileViewModel>.CreateNew().Build();
            var input = new DbMobileInput() { Secret = AuthSettings.SecretGenerateDB };

            var path = Path.Combine(Environment.CurrentDirectory, "appsettings.json");

            _dbMobileApplicationMock.Setup(x => x.CreateDBMobileAsync(input, path, "folder-test", Environment.CurrentDirectory)).ReturnsAsync(dbViewModel);
            _webHostEnvironmentMock.Setup(x => x.ContentRootPath).Returns(path);

            var result = await _controller.PostAsync(input);

            Assert.NotNull(input);
            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<DbMobileViewModel>>(okObjectResult.Value);

            var resultVerify = (Result<DbMobileViewModel>)okObjectResult.Value;

            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Shoud return url download db mobile with success")]
        [Trait("[WebApi.Controllers]-DbMobileController", "Controllers-GetDownloadUrlAsync")]
        public async Task ShouldReturnUrlDownloadDbMobileWithSuccessGetDownloadUrlAsync()
            {
            var dbViewModel = new DbMobileViewModel() { DownloadUrl = "http://google.com" };

            var configurationSection = new Mock<IConfigurationSection>();

            configurationSection.Setup(x => x.Value).Returns("folder-test");

            _configurationMock.Setup(x => x.GetSection("ConnectionStrings:FolderDbMobile")).Returns(configurationSection.Object);

            _dbMobileApplicationMock.Setup(x => x.GetLastDBMobileGeneratedAsync(It.IsAny<string>())).ReturnsAsync(dbViewModel);

            var result = await _controller.GetDownloadUrlAsync();

            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<DbMobileViewModel>>(okObjectResult.Value);

            var resultVerify = (Result<DbMobileViewModel>)okObjectResult.Value;

            Assert.NotNull(result);
            Assert.Contains(dbViewModel.DownloadUrl, resultVerify.Data.DownloadUrl);
            }

        [Fact(DisplayName = "Shoud return last updated dates async")]
        [Trait("[WebApi.Controllers]-DbMobileController", "Controllers-GetLastUpdatedDatesAsync")]
        public async Task ShouldReturnLastUpdatedDatesAsync()
            {
            var lastUpdatedDatesViewModel = Builder<LastUpdatedDatesViewModel>.CreateNew().Build();
            _dbMobileApplicationMock.Setup(x => x.GetLastUpdatedDatesAsync()).ReturnsAsync(lastUpdatedDatesViewModel);
            var result = await _controller.GetLastUpdatedDatesAsync();

            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<LastUpdatedDatesViewModel>>(okObjectResult.Value);

            var resultVerify = (Result<LastUpdatedDatesViewModel>)okObjectResult.Value;

            Assert.NotNull(result);
            Assert.Equal(lastUpdatedDatesViewModel, resultVerify.Data);
            }
    }
}
