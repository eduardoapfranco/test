using ConstruaApp.Api.Controllers;
using Domain.Entities;
using Domain.Interfaces.Services;
using FizzWare.NBuilder;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Controllers
{
    public class PlanControllerTest
    {
        private DomainNotificationHandler _notificationHandler;
        private Mock<IPlanDomainService> _planDomainServiceMock;
        private PlanController _controller;
        private Mock<ILogger<PlanController>> _loggerMock;
        
        public PlanControllerTest()
        {
            _planDomainServiceMock = new Mock<IPlanDomainService>();
            _loggerMock = new Mock<ILogger<PlanController>>();
            _notificationHandler = new DomainNotificationHandler();
            _controller = new PlanController(_notificationHandler, _planDomainServiceMock.Object, _loggerMock.Object);
        }


        [Fact(DisplayName = "Should return plans")]
        [Trait("[WebApi.Controllers]-PlanController", "Controllers-GetAsync")]
        public async Task ShouldReturnPlansPremium()
        {
            var plans = Builder<Plan>.CreateListOfSize(5).Build();
            _planDomainServiceMock.Setup(x => x.GetPlansPremiumWithType()).ReturnsAsync(plans);

            var result = await _controller.GetAsync();

            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<IEnumerable<Plan>>>(okObjectResult.Value);
            Assert.NotNull(result);

        }

        [Fact(DisplayName = "Should return plans without auth")]
        [Trait("[WebApi.Controllers]-PlanController", "Controllers-GetNoAuthAsync")]
        public async Task ShouldReturnPlansPremiumWithoutAuth()
        {
            var plans = Builder<Plan>.CreateListOfSize(5).Build();
            _planDomainServiceMock.Setup(x => x.GetPlansPremiumWithType()).ReturnsAsync(plans);

            var result = await _controller.GetNoAuthAsync();

            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<IEnumerable<Plan>>>(okObjectResult.Value);
            Assert.NotNull(result);

        }

        [Fact(DisplayName = "Should return internal server error get plans premium async")]
        [Trait("[WebApi.Controllers]-PlanController", "Controllers-GetAsync")]
        public async Task ShouldReturnInternalServerErrorPlansPremium()
        {

            _planDomainServiceMock.Setup(x => x.GetPlansPremiumWithType()).ThrowsAsync(new Exception("error"));

            var result = await _controller.GetAsync();
            Assert.IsType<ObjectResult>(result);
            var okObjectResult = (ObjectResult)result;
            Assert.Equal((int) HttpStatusCode.InternalServerError, okObjectResult.StatusCode);
            Assert.NotNull(result);
        }
    }
}
