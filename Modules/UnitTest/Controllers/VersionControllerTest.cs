using Application.AppServices.VersionApplication.ViewModel;
using Application.Interfaces;
using AutoMapper;
using ConstruaApp.Api.Controllers;
using Domain.Entities;
using Domain.Interfaces.Services;
using FizzWare.NBuilder;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Controllers
{
    public class VersionControllerTest
    {
        private DomainNotificationHandler _notificationHandler;
        private Mock<IVersionDomainService> _versionDomainServiceMock;
        private Mock<ILogger<VersionController>> _loggerMock;
        private Mock<IMapper> _mapperMock;
        private VersionController _controller;

        public VersionControllerTest()
        {
            _versionDomainServiceMock = new Mock<IVersionDomainService>();
            _notificationHandler = new DomainNotificationHandler();
            _loggerMock = new Mock<ILogger<VersionController>>();
            _mapperMock = new Mock<IMapper>();
            _controller = new VersionController(_notificationHandler, _versionDomainServiceMock.Object, _mapperMock.Object,  _loggerMock.Object);
        }

        [Fact(DisplayName = "Should return lastest version getVersionAsync")]
        [Trait("[WebApi.Controllers]-VersionController", "Controllers-GetVersionAsync")]
        public async Task ShouldReturnLatestVersionInGetVersionAsync()
        {
            //arrange
            var versionViewModel = Builder<VersionViewModel>.CreateNew().Build();
            var versionEntity = Builder<Version>.CreateNew().Build();
            _mapperMock.Setup(x => x.Map<VersionViewModel>(versionEntity)).Returns(versionViewModel);
            _versionDomainServiceMock.Setup(x => x.GetVersionAsync("android", "latest")).ReturnsAsync(versionEntity);

            //act
            var result = await _controller.GetVersionAsync("android", "latest");

            //assert
            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<VersionViewModel>>(okObjectResult.Value);

            var resultVerify = (Result<VersionViewModel>)okObjectResult.Value;

            Assert.NotNull(result);
            Assert.Equal(versionViewModel, resultVerify.Data);
        }

        [Fact(DisplayName = "Should return null when doesnt find any version getVersionAsync")]
        [Trait("[WebApi.Controllers]-VersionController", "Controllers-GetVersionAsync")]
        public async Task ShouldReturnNullWhenDoesntFindAnyVersionGetVersionAsync()
            {
            //arrange
            var versionViewModel = Builder<VersionViewModel>.CreateNew().Build();
            Version versionEntity = Builder<Version>.CreateNew().Build();
            Version versionEntityNull = null;
            _mapperMock.Setup(x => x.Map<VersionViewModel>(versionEntity)).Returns(versionViewModel);
            _versionDomainServiceMock.Setup(x => x.GetVersionAsync("android", "latest")).ReturnsAsync(versionEntityNull);

            //act
            var result = await _controller.GetVersionAsync("android", "latest");

            //assert
            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<VersionViewModel>>(okObjectResult.Value);

            var resultVerify = (Result<VersionViewModel>)okObjectResult.Value;

            Assert.NotNull(result);
            Assert.Null(resultVerify.Data);
            }
        }
}
