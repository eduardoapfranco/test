using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.UoW;
using Domain.Services;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using UnitTest.Domain.Faker;
using Xunit;
using System.Linq;

namespace UnitTest.Domain
{
    public class PlanDomainServiceTest
    {
        private Mock<IPlanRepository> _planRepositoryMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<PlanDomainService>> _loggerMock;
        private PlanDomainService _planDomainService;

        public PlanDomainServiceTest()
        {
            _planRepositoryMock = new Mock<IPlanRepository>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<PlanDomainService>>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _planDomainService = new PlanDomainService(_planRepositoryMock.Object, _smartNotificationMock.Object, _unitOfWorkMock.Object, new DomainNotificationHandler(), _loggerMock.Object);
        }

        [Fact(DisplayName = "Shoud return list of plans freemium")]
        [Trait("[Domain.Services]-PlanDomainService", "Plan-GetFreemiumPlanAsync")]
        public async Task ShouldReturnFreemiumPlan()
        {
            // arrange
            var plans = PlanFaker.CreateListPlansFreemium();
            _planRepositoryMock.Setup(x => x.SelectFilterAsync(x => x.Title.Equals("Freemium") && x.Active.Equals(1))).ReturnsAsync(plans);

            // act
            var result = await _planDomainService.GetFreemiumPlanAsync();

            // assert
            Assert.NotNull(result);
            Assert.IsType<Plan>(result);
        }

        [Fact(DisplayName = "Shoud return plan premium")]
        [Trait("[Domain.Services]-PlanDomainService", "Plan-GetPremiumPlanAsync")]
        public async Task ShouldReturnPremiumPlan()
        {
            // arrange
            var plans = PlanFaker.CreateListPlansPreemium();
            _planRepositoryMock.Setup(x => x.GetWithType(It.IsAny<int>())).ReturnsAsync(plans.FirstOrDefault);

            // act
            var result = await _planDomainService.GetPremiumPlanAsync(It.IsAny<int>());

            // assert
            Assert.NotNull(result);
            Assert.IsType<Plan>(result);
        }

        [Fact(DisplayName = "Shoud return list of plans premiuns")]
        [Trait("[Domain.Services]-PlanDomainService", "Plan-GetPremiumPlanAsync")]
        public async Task ShouldReturnPremiumPlans()
        {
            // arrange
            var plans = PlanFaker.CreateListPlansPreemium();
            _planRepositoryMock.Setup(x => x.GetPlansPremiumWithType()).ReturnsAsync(plans);

            // act
            var result = await _planDomainService.GetPlansPremiumWithType();

            // assert
            Assert.NotNull(result);
        }
    }
}


