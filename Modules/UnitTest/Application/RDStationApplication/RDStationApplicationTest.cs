using Application.AppServices.RDApplication;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Domain.Entities;
using Domain.Input.RDStation;
using Domain.Interfaces.Services;
using FizzWare.NBuilder;
using Infra.CrossCutting.Notification.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnitTest.Application.UserApplication.Faker;
using Xunit;
using App = Application.AppServices;

namespace UnitTest.Application.RDApplicationTest
    {
    public class RDStationApplicationTest
        {
        private Mock<IRDStationDomainService> _rdStationDomainServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private App.RDApplication.RDStationApplication _rdApplication;
        private readonly Fixture _fixture;
        private Mock<ILogger<RDStationApplication>> _loggerMock;

        public RDStationApplicationTest()
            {
            // configure
            _rdStationDomainServiceMock = new Mock<IRDStationDomainService>();
            _mapperMock = new Mock<IMapper>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _loggerMock = new Mock<ILogger<RDStationApplication>>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _rdApplication = new App.RDApplication.RDStationApplication(_rdStationDomainServiceMock.Object, _mapperMock.Object, _smartNotificationMock.Object, _loggerMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
            }

        [Fact(DisplayName = "Shoud return sucess create RDStation Conversions async")]
        [Trait("[Application.AppServices]-RDApplication", "Application-CreateRDStationConversions")]
        public async Task ShouldReturnSuccessCreateRDStationConversionsAsync()
            {
            // arrange
            var rdInput = new RDStationInput() { 
                UrlBase = "https://urlbase.construa.app",
                ApiSecret = "apisecret-construa"
                 };

            var userLoginInput = UserFaker.CreateUserLoginInput();
            var userViewModel = UserFaker.UserViewModel;
            var userList = UserFaker.CreateListUserLogin();
            var conversion = Builder<Conversion>.CreateNew().Build();
            _rdStationDomainServiceMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(userList);
            _rdStationDomainServiceMock.Setup(x => x.PostConversionAsync(It.IsAny<User>(), It.IsAny<RDStationInput>())).ReturnsAsync(conversion);


            // act
            var result = await _rdApplication.CreateRDStationConversion(rdInput);

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            _rdStationDomainServiceMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.AtLeastOnce());
            }
        [Fact(DisplayName = "Shoud return empty list when does'nt found any user to create RDStation conversions async")]
        [Trait("[Application.AppServices]-RDApplication", "Application-CreateRDStationConversions")]
        public async Task ShouldReturnEmptyListWhenDoesntFoundAnyUserToCreateRDStationConversionsAsync()
            {
            // arrange
            var rdInput = new RDStationInput()
                {
                UrlBase = "https://urlbase.construa.app",
                ApiSecret = "apisecret-construa"
                };

            var userLoginInput = UserFaker.CreateUserLoginInput();
            var userViewModel = UserFaker.UserViewModel;
            var userList = new List<User>();
            var conversion = Builder<Conversion>.CreateNew().Build();
            //_mapperMock.Setup(x => x.Map<UserViewModel>(userList.FirstOrDefault())).Returns(userViewModel);
            _rdStationDomainServiceMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(userList);
            _rdStationDomainServiceMock.Setup(x => x.PostConversionAsync(It.IsAny<User>(), It.IsAny<RDStationInput>())).ReturnsAsync(conversion);


            // act
            var result = await _rdApplication.CreateRDStationConversion(rdInput);

            // assert
            Assert.Null(result);
            _rdStationDomainServiceMock.Verify(x => x.SelectFilterAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once());
            _rdStationDomainServiceMock.Verify(x => x.PostConversionAsync(It.IsAny<User>(), It.IsAny<RDStationInput>()), Times.Exactly(0));
            }
        }
    }
