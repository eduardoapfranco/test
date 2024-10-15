using Application.Interfaces;
using ConstruaApp.Api.Controllers;
using Domain.Entities;
using Infra.CrossCutting.Notification.Handler;
using Microsoft.Extensions.Configuration;
using Moq;
using UnitTest.Application.OrderApplication.Faker;
using FizzWare.NBuilder;
using Infra.CrossCutting.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Application.AppServices.OrderApplication.Input;
using Application.AppServices.OrderApplication.ViewModels;

namespace UnitTest.Controllers
    {
    public class OrderControllerTest
    {
        private DomainNotificationHandler _notificationHandler;
        private Mock<IOrderApplication> _orderApplicationMock;
        private Mock<IConfiguration> _configurationMock;
        private OrderController _controller;
        private Mock<ILogger<OrderController>> _loggerMock;

        public OrderControllerTest()
        {
            _orderApplicationMock = new Mock<IOrderApplication>();
            _notificationHandler = new DomainNotificationHandler();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<OrderController>>();
            _controller = new OrderController(_notificationHandler, _orderApplicationMock.Object, _configurationMock.Object, _loggerMock.Object);
        }

        [Fact(DisplayName = "Should return true when process pagseguro notification async")]
        [Trait("[WebApi.Controllers]-OrderController", "Controllers-PostAsync")]
        public async Task ShouldReturnTrueWhenProcessPagseguroNotificationAsync()
            {
            //arrange
            var input = WebhookFaker.CreateWebhookPagSeguroNotificationInput();
            var webhook = Builder<Webhook>.CreateNew().Build();
            _orderApplicationMock.Setup(x => x.ProcessPagseguroNotificationAsync(input)).ReturnsAsync(true);

            var configurationSection = new Mock<IConfigurationSection>();

            configurationSection.Setup(x => x.Value).Returns("A");

            _configurationMock.Setup(x => x.GetSection("Pagseguro:UrlBase")).Returns(configurationSection.Object);
            _configurationMock.Setup(x => x.GetSection("Pagseguro:Token")).Returns(configurationSection.Object);
            _configurationMock.Setup(x => x.GetSection("Pagseguro:Email")).Returns(configurationSection.Object);

            //act
            var result = await _controller.ProcessPagseguroNotificationAsync(input);

            //assert
            Assert.NotNull(input.NotificationCode);
            Assert.NotNull(input.NotificationType);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<bool>>(okObjectResult.Value);

            var okObjectResultValue = (Result<bool>)okObjectResult.Value;
            Assert.True(okObjectResultValue.Data);
            }

        [Fact(DisplayName = "Should return false when can't process pagseguro notification async")]
        [Trait("[WebApi.Controllers]-OrderController", "Controllers-PostAsync")]
        public async Task ShouldReturnFalseWhenCantProcessPagseguroNotificationAsync()
            {
            //arrange
            var input = WebhookFaker.CreateWebhookPagSeguroNotificationInput();
            var webhook = Builder<Webhook>.CreateNew().Build();
            _orderApplicationMock.Setup(x => x.ProcessPagseguroNotificationAsync(input)).ReturnsAsync(false);

            var configurationSection = new Mock<IConfigurationSection>();

            configurationSection.Setup(x => x.Value).Returns("A");

            _configurationMock.Setup(x => x.GetSection("Pagseguro:UrlBase")).Returns(configurationSection.Object);
            _configurationMock.Setup(x => x.GetSection("Pagseguro:Token")).Returns(configurationSection.Object);
            _configurationMock.Setup(x => x.GetSection("Pagseguro:Email")).Returns(configurationSection.Object);

            //act
            var result = await _controller.ProcessPagseguroNotificationAsync(input);

            //assert
            Assert.NotNull(input.NotificationCode);
            Assert.NotNull(input.NotificationType);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<bool>>(okObjectResult.Value);

            var okObjectResultValue = (Result<bool>)okObjectResult.Value;
            Assert.False(okObjectResultValue.Data);
            }

        [Fact(DisplayName = "Should redirect when get pagseguro order form async")]
        [Trait("[WebApi.Controllers]-OrderController", "Controllers-PostAsync")]
        public async Task ShouldRedirectWhenGetPagseguroOrderFormAsync()
            {
            //arrange
            var input = WebhookFaker.CreateWebhookPagSeguroNotificationInput();
            var user = Builder<User>.CreateNew().Build();
            var plan = Builder<Plan>.CreateNew().Build();
            string urlRedirect = "http://www.construaapp.com.br";
            _orderApplicationMock.Setup(x => x.GetPagseguroOrderFormAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<WebhookPagSeguroNotificationInput>())).ReturnsAsync(urlRedirect);

            var configurationSection = new Mock<IConfigurationSection>();

            configurationSection.Setup(x => x.Value).Returns("A");

            _configurationMock.Setup(x => x.GetSection("Pagseguro:UrlBase")).Returns(configurationSection.Object);
            _configurationMock.Setup(x => x.GetSection("Pagseguro:UrlBaseOrderForm")).Returns(configurationSection.Object);
            _configurationMock.Setup(x => x.GetSection("Pagseguro:UrlRedirectOrderForm")).Returns(configurationSection.Object);
            _configurationMock.Setup(x => x.GetSection("Pagseguro:URLNotifications")).Returns(configurationSection.Object);
            _configurationMock.Setup(x => x.GetSection("Pagseguro:Token")).Returns(configurationSection.Object);
            _configurationMock.Setup(x => x.GetSection("Pagseguro:Email")).Returns(configurationSection.Object);

            //act
            var result = await _controller.GetPagseguroOrderFormAsync(plan.Id);

            //assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<PagSeguroFormResponse>>(okObjectResult.Value);

            var okObjectResultValue = (Result<PagSeguroFormResponse>)okObjectResult.Value;
            Assert.Equal(urlRedirect, okObjectResultValue.Data.UrlPagSeguro);
            }

        [Fact(DisplayName = "Should return null when can't get pagseguro order form async")]
        [Trait("[WebApi.Controllers]-OrderController", "Controllers-PostAsync")]
        public async Task ShouldReturnFalseWhenCantGetPagseguroOrderFormAsync()
            {
            //arrange
            var input = WebhookFaker.CreateWebhookPagSeguroNotificationInput();
            var user = Builder<User>.CreateNew().Build();
            var plan = Builder<Plan>.CreateNew().Build();
            string urlRedirect = string.Empty;
            _orderApplicationMock.Setup(x => x.GetPagseguroOrderFormAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<WebhookPagSeguroNotificationInput>())).ReturnsAsync(urlRedirect);

            var configurationSection = new Mock<IConfigurationSection>();

            configurationSection.Setup(x => x.Value).Returns("A");
            
            _configurationMock.Setup(x => x.GetSection("Pagseguro:UrlBase")).Returns(configurationSection.Object);
            _configurationMock.Setup(x => x.GetSection("Pagseguro:UrlBaseOrderForm")).Returns(configurationSection.Object);
            _configurationMock.Setup(x => x.GetSection("Pagseguro:UrlRedirectOrderForm")).Returns(configurationSection.Object);
            _configurationMock.Setup(x => x.GetSection("Pagseguro:URLNotifications")).Returns(configurationSection.Object);
            _configurationMock.Setup(x => x.GetSection("Pagseguro:Token")).Returns(configurationSection.Object);
            _configurationMock.Setup(x => x.GetSection("Pagseguro:Email")).Returns(configurationSection.Object);

            //act
            var result = await _controller.GetPagseguroOrderFormAsync(plan.Id);

            //assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<PagSeguroFormResponse>>(okObjectResult.Value);

            var okObjectResultValue = (Result<PagSeguroFormResponse>)okObjectResult.Value;
            Assert.Equal(urlRedirect, okObjectResultValue.Data.UrlPagSeguro);
            }
        }
}
