using System;
using AutoMapper;
using Domain.Core;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Moq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using UnitTest.Application.CategoryApplication.Faker;
using Xunit;
using App = Application.AppServices;
using System.Linq.Expressions;
using Domain.Entities;
using Application.AppServices.ConstructionApplication.ViewModel;
using System.Collections.Generic;
using NuGet.Frameworks;
using Microsoft.Extensions.Logging;
using UnitTest.Application.ConstructionApplication.Faker;

namespace UnitTest.Application.ConstructionApplication
    {
    public class ConstructionApplicationTest
        {
        private Mock<IConstructionDomainService> _constructionDomainServiceMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<ILogger<App.ConstructionApplication.ConstructionApplication>> _loggerMock;
        private App.ConstructionApplication.ConstructionApplication _constructionApplication;
        private readonly Fixture _fixture;
        private Mock<IMapper> _mapperMock;

        private int userId = 1;
        private string guid1 = Guid.NewGuid().ToString();
        private string guid2 = Guid.NewGuid().ToString();

        public ConstructionApplicationTest()
            {
            // configure
            _constructionDomainServiceMock = new Mock<IConstructionDomainService>();
            _mapperMock = new Mock<IMapper>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _loggerMock = new Mock<ILogger<App.ConstructionApplication.ConstructionApplication>>();
            _constructionApplication = 
                new App.ConstructionApplication.ConstructionApplication(
                    _constructionDomainServiceMock.Object, 
                    _smartNotificationMock.Object, 
                    _mapperMock.Object,
                    _loggerMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
            }

        [Fact(DisplayName = "Shoud sync a list of constructions async")]
        [Trait("[Application.AppServices]-ConstructionApplication", "Application-SyncAsync")]
        public async Task ShouldSyncAListOfConstructions()
            {
            // arrange
            List<ConstructionViewModel> request = (List<ConstructionViewModel>)ConstructionFaker.CreateListConstructionViewModel();

            _constructionDomainServiceMock.Setup(x => x.Sync(It.IsAny<IEnumerable<Construction>>(), It.IsAny<IEnumerable<Construction>>(), It.IsAny<IEnumerable<Construction>>())).ReturnsAsync(true);

            // act
            var result = await _constructionApplication.SyncAsync(userId, request);

            // assert
            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Shoud get a list of constructions async")]
        [Trait("[Application.AppServices]-ConstructionApplication", "Application-ListAsync")]
        public async Task ShouldGetAListConstructionAsync()
            {
            // arrange
            List<Construction> list = (List<Construction>)ConstructionFaker.CreateListConstruction();
            List<ConstructionViewModel> constructionListViewModel = (List<ConstructionViewModel>)ConstructionFaker.CreateListConstructionViewModel();
            _mapperMock.Setup(x => x.Map<IEnumerable<ConstructionViewModel>>(list)).Returns(constructionListViewModel);
            _constructionDomainServiceMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Construction, bool>>>())).ReturnsAsync(list);

            // act
            var result = await _constructionApplication.ListAsync(userId);

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            }

        [Fact(DisplayName = "Shoud insert a construction async")]
        [Trait("[Application.AppServices]-ConstructionApplication", "Application-InsertAsync")]
        public async Task ShouldInsertAConstuctionAsync()
            {
            //arrange
            var constructionInput = ConstructionFaker.CreateConstructionInput;
            constructionInput.UserId = this.userId;
            var constructionResult = ConstructionFaker.CreateConstruction;
            var constructionResultViewModel = ConstructionFaker.CreateConstructionViewModel;
            _mapperMock.Setup(x => x.Map<ConstructionViewModel>(constructionResult)).Returns(constructionResultViewModel);

            _constructionDomainServiceMock.Setup(x => x.InsertAsync(It.IsAny<Construction>())).ReturnsAsync(constructionResult);
            
            //act
            var result = await _constructionApplication.InsertAsync(constructionInput);
            
            //assert
            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Shoud update a construction async")]
        [Trait("[Application.AppServices]-ConstructionApplication", "Application-UpdateAsync")]
        public async Task ShouldUpdateAConstructionAsync()
            {
            //assert
            var constructionInput = ConstructionFaker.CreateConstructionInput;
            constructionInput.UserId = this.userId;
            var constructionResult = ConstructionFaker.CreateConstruction;
            var listResult = new List<Construction>() { constructionResult };

            List<ConstructionViewModel> constructionListViewModel = (List<ConstructionViewModel>)ConstructionFaker.CreateListConstructionViewModel();
            _mapperMock.Setup(x => x.Map<IEnumerable<ConstructionViewModel>>(listResult)).Returns(constructionListViewModel);

            var constructionResultViewModel = ConstructionFaker.CreateConstructionViewModel;
            _mapperMock.Setup(x => x.Map<ConstructionViewModel>(constructionResult)).Returns(constructionResultViewModel);

            _constructionDomainServiceMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Construction, bool>>>())).ReturnsAsync(listResult);

            _constructionDomainServiceMock.Setup(x => x.UpdateAsync(It.IsAny<Construction>())).ReturnsAsync(constructionResult);

            //act
            var result = await _constructionApplication.UpdateAsync(constructionInput);

            //assert
            Assert.NotNull(result);
            }
        }
    }
