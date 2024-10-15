using Application.AppServices.OrderApplication.Input;
using Application.AppServices.OrderApplication.Input.Pagseguro;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Domain.Entities;
using Domain.Input;
using Domain.Interfaces.Services;
using FizzWare.NBuilder;
using Infra.CrossCutting.Email.Interfaces;
using Infra.CrossCutting.Notification.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTest.Application.OrderApplication.Faker;
using UnitTest.Application.UserApplication.Faker;
using Xunit;
using App = Application.AppServices;


namespace UnitTest.Application.OrderApplication
{
    public class OrderApplicationTest
        {
        private Mock<IWebhookDomainService> _webhookDomainServiceMock;
        private App.OrderApplication.OrderApplication _orderApplication;
        private Mock<IMapper> _mapperMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private readonly Fixture _fixture;
        private Mock<ILogger<App.OrderApplication.OrderApplication>> _loggerMock;
        private Mock<IUserDomainService> _userDomainServiceMock;
        private Mock<IPlanDomainService> _planDomainServiceMock;
        private Mock<IEmailSendService> _emailSendServiceMock;
        private Mock<ISignatureDomainService> _signatureDomainServiceMock;
        public OrderApplicationTest()
            {
            // configure
            _webhookDomainServiceMock = new Mock<IWebhookDomainService>();
            _userDomainServiceMock = new Mock<IUserDomainService>();
            _signatureDomainServiceMock = new Mock<ISignatureDomainService>();
            _planDomainServiceMock = new Mock<IPlanDomainService>();
            _mapperMock = new Mock<IMapper>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _emailSendServiceMock = new Mock<IEmailSendService>();
            _loggerMock = new Mock<ILogger<App.OrderApplication.OrderApplication>>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _orderApplication = new App.OrderApplication.OrderApplication(_webhookDomainServiceMock.Object, _smartNotificationMock.Object, _mapperMock.Object, _loggerMock.Object, _userDomainServiceMock.Object, _planDomainServiceMock.Object, _emailSendServiceMock.Object, _signatureDomainServiceMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
            }

        [Fact(DisplayName = "Shoud return true when get full pagseguro notification and save into database")]
        [Trait("[Application.AppServices]-OrderApplication", "Application-ProcessPagseguroNotificationAsync")]
        public async Task ShouldReturnTrueWhenGetFullPagseguroNotificationAndSaveIntoDatabase()
            {
            // arrange
            var input = WebhookFaker.CreateWebhookPagSeguroNotificationInput();
            var webhook = Builder<Webhook>.CreateNew().Build();
            var pagseguroTransactionInput = Builder<PagSeguroTransactionInput>.CreateNew().Build();
            pagseguroTransactionInput.Status = TransactionStatus.PAGA;
            pagseguroTransactionInput.Sender = Builder<TransactionSender>.CreateNew().Build();
            var pagseguroTransactionXML = pagseguroTransactionInput.ToXML();

            _webhookDomainServiceMock.Setup(x => x.InsertAsync(It.IsAny<Webhook>())).ReturnsAsync(webhook);
            _webhookDomainServiceMock.Setup(x => x.GetFullPagseguroNotificationAsync(It.IsAny<PagSeguroNotificationInput>()))
                .ReturnsAsync(pagseguroTransactionXML);

            _userDomainServiceMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<User>())).ReturnsAsync(new User());
            _signatureDomainServiceMock.Setup(x => x.UpdateUserToPremiumPlanAsync(It.IsAny<User>(), It.IsAny<Plan>(), null)).ReturnsAsync(new User());

            // act
            var result = await _orderApplication.ProcessPagseguroNotificationAsync(input);

            // assert
            Assert.True(result);
            }

        [Fact(DisplayName = "Shoud return false when can't get full pagseguro notification and save into database")]
        [Trait("[Application.AppServices]-OrderApplication", "Application-ProcessPagseguroNotificationAsync")]
        public async Task ShouldReturnFalseWhenCantGetFullPagseguroNotificationAndSaveIntoDatabase()
            {
            // arrange
            var input = WebhookFaker.CreateWebhookPagSeguroNotificationInput();
            Webhook webhook = null;
            var pagseguroTransactionInput = Builder<PagSeguroTransactionInput>.CreateNew().Build();
            pagseguroTransactionInput.Status = TransactionStatus.PAGA;
            pagseguroTransactionInput.Sender = Builder<TransactionSender>.CreateNew().Build();
            var pagseguroTransactionXML = string.Empty;

            _webhookDomainServiceMock.Setup(x => x.InsertAsync(It.IsAny<Webhook>())).ReturnsAsync(webhook);
            _webhookDomainServiceMock.Setup(x => x.GetFullPagseguroNotificationAsync(It.IsAny<PagSeguroNotificationInput>()))
                .ReturnsAsync(pagseguroTransactionXML);

            _userDomainServiceMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<User>())).ReturnsAsync(new User());
            _signatureDomainServiceMock.Setup(x => x.UpdateUserToPremiumPlanAsync(It.IsAny<User>(), It.IsAny<Plan>(), null)).ReturnsAsync(new User());

            // act
            var result = await _orderApplication.ProcessPagseguroNotificationAsync(input);

            // assert
            Assert.False(result);
            }

        [Fact(DisplayName = "Shoud return pagseguro order form url when get pagseguro order form async")]
        [Trait("[Application.AppServices]-OrderApplication", "Application-GetPagseguroOrderFormAsync")]
        public async Task ShouldReturnPagSeguroOrderFormUrlWhenGetPagseguroOrderFormAsync()
            {
            // arrange
            var input = WebhookFaker.CreateWebhookPagSeguroNotificationInput();
            User userFaker = UserFaker.CreateUser;
            Plan planFaker = Builder<Plan>.CreateNew().Build();
            string orderCode = "OrderConstrua";

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(userFaker);
            _planDomainServiceMock.Setup(x => x.GetPremiumPlanAsync(It.IsAny<int>())).ReturnsAsync(planFaker);

            _webhookDomainServiceMock.Setup(x => x.GetPagseguroCheckoutCodeAsync(It.IsAny<Dictionary<string, string>>(),
                It.IsAny<PagSeguroNotificationInput>())).ReturnsAsync(orderCode);


            // act
            var result = await _orderApplication.GetPagseguroOrderFormAsync(userFaker.Id, planFaker.Id, input);

            // assert
            Assert.NotNull(result);
            Assert.Contains(orderCode, result);
            Assert.Contains(input.UrlBaseOrderForm, result);
            }

        [Fact(DisplayName = "Shoud return null when can't get pagseguro order form async")]
        [Trait("[Application.AppServices]-OrderApplication", "Application-GetPagseguroOrderFormAsync")]
        public async Task ShouldReturnNullWhenCantGetPagseguroOrderFormAsync()
            {
            // arrange
            var input = WebhookFaker.CreateWebhookPagSeguroNotificationInput();
            User userFaker = UserFaker.CreateUser;
            Plan planFaker = Builder<Plan>.CreateNew().Build();
            string orderCode = string.Empty;

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(userFaker);
            _planDomainServiceMock.Setup(x => x.GetPremiumPlanAsync(It.IsAny<int>())).ReturnsAsync(planFaker);

            _webhookDomainServiceMock.Setup(x => x.GetPagseguroCheckoutCodeAsync(It.IsAny<Dictionary<string, string>>(),
                It.IsAny<PagSeguroNotificationInput>())).ReturnsAsync(orderCode);


            // act
            var result = await _orderApplication.GetPagseguroOrderFormAsync(userFaker.Id, planFaker.Id, input);

            // assert
            Assert.Null(result);
            }

        }
    }
