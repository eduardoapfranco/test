using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.UoW;
using Domain.Services;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using Version = Domain.Entities.Version;

namespace UnitTest.Domain
{
    public class VersionDomainServiceTest
        {
        private Mock<IVersionRepository> _versionRepositoryMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private VersionDomainService _versionDomainService;
        private Mock<ILogger<VersionDomainService>> _loggerMock;


        public VersionDomainServiceTest()
            {
            _versionRepositoryMock = new Mock<IVersionRepository>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<VersionDomainService>>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _versionDomainService = new VersionDomainService(_versionRepositoryMock.Object, _smartNotificationMock.Object, _unitOfWorkMock.Object, new DomainNotificationHandler(), _loggerMock.Object);
            }

        [Fact(DisplayName = "Shoud return last android version")]
        [Trait("[Domain.Services]-VersionDomainService", "Version-GetVersionAsync")]
        public async Task ShouldReturnLastAndroidVersion()
            {
            // arrange
            var version = new Version() {
                Platform = "android",
                _Version = "1.0.0"
                };

            List<Version> listaDeVersoes = new List<Version>()
                {
                version
                };

            _versionRepositoryMock.Setup(x => x.GetLast("android")).ReturnsAsync(version);
            _versionRepositoryMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Version, bool>>>())).ReturnsAsync(listaDeVersoes);

            // act
            var result = await _versionDomainService.GetVersionAsync("android", "latest");

            // assert
            Assert.NotNull(result);
            }
        [Fact(DisplayName = "Shoud return specified ios version")]
        [Trait("[Domain.Services]-VersionDomainService", "Version-GetVersionAsync")]
        public async Task ShouldReturnSpecifiedIOSVersion()
            {
            // arrange
            var version = new Version()
                {
                Platform = "ios",
                _Version = "1.0.0"
                };

            List<Version> listaDeVersoes = new List<Version>()
                {
                version
                };

            _versionRepositoryMock.Setup(x => x.GetLast("android")).ReturnsAsync(version);
            _versionRepositoryMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Version, bool>>>())).ReturnsAsync(listaDeVersoes);

            // act
            var result = await _versionDomainService.GetVersionAsync("ios", "1.0.0");

            // assert
            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Shoud return version no not found message")]
        [Trait("[Domain.Services]-VersionDomainService", "Version-GetVersionAsync")]
        public async Task ShouldReturnVersionNotFoundMessage()
            {
            // arrange
            var version = new Version()
                {
                Platform = "ios",
                _Version = "1.0.0"
                };

            List<Version> listaDeVersoes = new List<Version>();

            _versionRepositoryMock.Setup(x => x.GetLast("android")).ReturnsAsync(version);
            _versionRepositoryMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Version, bool>>>())).ReturnsAsync(listaDeVersoes);

            // act
            var result = await _versionDomainService.GetVersionAsync("android", "1.0.0");

            // assert
            Assert.Null(result);
            }
        }

    }
