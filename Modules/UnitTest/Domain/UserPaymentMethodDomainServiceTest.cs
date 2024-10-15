using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.UoW;
using Domain.Services;
using FluentAssertions;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using Moq;
using UnitTest.Application.UserApplication.Faker;
using UnitTest.Domain.Faker;
using Xunit;

namespace UnitTest.Domain
{
    public class UserPaymentMethodDomainServiceTest
    {
        private Mock<IUserPaymentMethodRepository> _repositoryMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private UserPaymentMethodDomainService _domainService;
        private Mock<ILogger<UserPaymentMethodDomainService>> _loggerMock;
        private readonly Fixture _fixture;



        public UserPaymentMethodDomainServiceTest()
        {
            _repositoryMock = new Mock<IUserPaymentMethodRepository>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UserPaymentMethodDomainService>>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _domainService = new UserPaymentMethodDomainService(_repositoryMock.Object, _smartNotificationMock.Object, _unitOfWorkMock.Object, new DomainNotificationHandler(), _loggerMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }

        [Fact(DisplayName = "Should inactive all payments and insert new payment method")]
        [Trait("[Domain.Services]-UserPaymentMethodDomainService", "Domain-InsertAndInactiveAsync")]
        public async Task ShouldInactiveAllPaymentsUserAndInsertNewPaymentMethod()
        {
            // arrange
            var userPaymentMethod = _fixture.Create<UserPaymentMethod>();
            var userPaymentsMethods = _fixture.CreateMany<UserPaymentMethod>();
            
            _repositoryMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<UserPaymentMethod, bool>>>()))
                .ReturnsAsync(userPaymentsMethods);
            _unitOfWorkMock.Setup(x => x.UserPaymentMethod.UpdateAsync(userPaymentMethod)).ReturnsAsync(userPaymentMethod);
            _unitOfWorkMock.Setup(x => x.UserPaymentMethod.InsertAsync(userPaymentMethod)).ReturnsAsync(userPaymentMethod);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(new CommandResponse(true));


            // act
            var result = await _domainService.InsertAndInactiveAsync(userPaymentMethod);

            // assert
            _repositoryMock.Verify(x => x.SelectFilterAsync(It.IsAny<Expression<Func<UserPaymentMethod, bool>>>()), Times.Once);
            Assert.NotNull(result);
            result.UserId.Should().Be(result.UserId);
            result.Active.Should().Be(result.Active);
            result.Flag.Should().Be(result.Flag);
            result.LastFourDigits.Should().Be(result.LastFourDigits);
            result.Type.Should().Be(result.Type);
            result.Token.Should().Be(result.Token);
            result.CreatedAt.Should().Be(result.CreatedAt);
            result.UpdatedAt.Should().Be(result.UpdatedAt);
        }


        [Fact(DisplayName = "Should insert new payment method when not exists other payments methods")]
        [Trait("[Domain.Services]-UserPaymentMethodDomainService", "Domain-InsertAndInactiveAsync")]
        public async Task ShouldInsertNewPaymentMethodWhenNotExistsOtherPaymentsMethods()
        {

            // arrange
            var userPaymentMethod = _fixture.Create<UserPaymentMethod>();
            var userPaymentsMethods = new List<UserPaymentMethod>();

            _repositoryMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<UserPaymentMethod, bool>>>()))
                .ReturnsAsync(userPaymentsMethods);
            _unitOfWorkMock.Setup(x => x.UserPaymentMethod.UpdateAsync(userPaymentMethod)).ReturnsAsync(userPaymentMethod);
            _unitOfWorkMock.Setup(x => x.UserPaymentMethod.InsertAsync(userPaymentMethod)).ReturnsAsync(userPaymentMethod);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(new CommandResponse(true));


            // act
            var result = await _domainService.InsertAndInactiveAsync(userPaymentMethod);

            // assert
            _repositoryMock.Verify(x => x.SelectFilterAsync(It.IsAny<Expression<Func<UserPaymentMethod, bool>>>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.UserPaymentMethod.UpdateAsync(userPaymentMethod), Times.Never);
            Assert.NotNull(result);
            result.UserId.Should().Be(result.UserId);
            result.Active.Should().Be(result.Active);
            result.Flag.Should().Be(result.Flag);
            result.LastFourDigits.Should().Be(result.LastFourDigits);
            result.Type.Should().Be(result.Type);
            result.Token.Should().Be(result.Token);
            result.CreatedAt.Should().Be(result.CreatedAt);
            result.UpdatedAt.Should().Be(result.UpdatedAt);
        }

    }
}
