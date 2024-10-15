using Application.AppServices.SignatureApplication.Input;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Domain.Entities;
using Domain.Enum;
using Domain.Input;
using Domain.Input.Iugu;
using Domain.Interfaces.Services;
using FizzWare.NBuilder;
using Infra.CrossCutting.Email.Interfaces;
using Infra.CrossCutting.Notification.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnitTest.Application.UserApplication.Faker;
using Xunit;
using App = Application.AppServices;


namespace UnitTest.Application.SignatureApplication
{
    public class SignatureApplicationTest
        {
        private Mock<ISignatureDomainService> _signatureDomainServiceMock;
        private App.SignatureApplication.SignatureApplication _signatureApplication;
        private Mock<IMapper> _mapperMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private readonly Fixture _fixture;
        private Mock<ILogger<App.SignatureApplication.SignatureApplication>> _loggerMock;
        private Mock<IUserDomainService> _userDomainServiceMock;
        private Mock<IPlanDomainService> _planDomainServiceMock;
        private Mock<IEmailSendService> _emailSendServiceMock;
        private Mock<IWebhookDomainService> _webhookDomainServiceMock;
        private Mock<IUserPaymentMethodDomainService> _userPaymentMethodDomainServiceMock;

        public SignatureApplicationTest()
            {
            // configure
            _signatureDomainServiceMock = new Mock<ISignatureDomainService>();
            _userDomainServiceMock = new Mock<IUserDomainService>();
            _planDomainServiceMock = new Mock<IPlanDomainService>();
            _mapperMock = new Mock<IMapper>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _emailSendServiceMock = new Mock<IEmailSendService>();
            _webhookDomainServiceMock = new Mock<IWebhookDomainService>();
            _userPaymentMethodDomainServiceMock = new Mock<IUserPaymentMethodDomainService>();
            _loggerMock = new Mock<ILogger<App.SignatureApplication.SignatureApplication>>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _signatureApplication = new App.SignatureApplication.SignatureApplication(_signatureDomainServiceMock.Object, _smartNotificationMock.Object, _mapperMock.Object, _loggerMock.Object, _userDomainServiceMock.Object, _planDomainServiceMock.Object, _emailSendServiceMock.Object, _webhookDomainServiceMock.Object, _userPaymentMethodDomainServiceMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
            }

        [Fact(DisplayName = "Shoud return iugu signature id when create iugu signature")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-PostSignatureAsync")]
        public async Task ShouldReturnIuguSignatureId()
            {
            // arrange
            var input = Builder<SignatureInput>.CreateNew().Build();

            var iuguSignature = Builder<Signature>.CreateNew().Build();
            var iuguCustomer = Builder<Customer>.CreateNew().Build();
            var iuguPaymentMethod = Builder<PaymentMethod>.CreateNew().Build();
            var recentInvoices = Builder<Invoice>.CreateListOfSize(1).Build();
            iuguSignature.RecentInvoices = recentInvoices.ToList();
            iuguSignature.RecentInvoices.First().Status = "paid";
            User userFaker = UserFaker.CreateUser;

            _signatureDomainServiceMock.Setup(x => x.PostUserAsync(It.IsAny<User>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguCustomer);
            _signatureDomainServiceMock.Setup(x => x.PostUserPaymentMethodAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguPaymentMethod);
            _signatureDomainServiceMock.Setup(x => x.PostSignatureMethodAsync(It.IsAny<Plan>(), It.IsAny<string>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguSignature);

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(userFaker);
            _userDomainServiceMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(userFaker);
            _signatureDomainServiceMock.Setup(x => x.UpdateUserToPremiumPlanAsync(It.IsAny<User>(), It.IsAny<Plan>(), It.IsAny<string>())).ReturnsAsync(userFaker);

            _planDomainServiceMock.Setup(x => x.GetPremiumPlanAsync(It.IsAny<int>())).ReturnsAsync(new Plan());

            // act
            var result = await _signatureApplication.PostSignatureAsync(input);

            // assert
            Assert.NotEmpty(result.IuguSignature.Id);
            }

        [Fact(DisplayName = "Shoud return null when cant create iugu customer")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-PostSignatureAsync")]
        public async Task ShouldReturnNullWhenCantCreateIuguCustomer()
            {
            // arrange
            var input = Builder<SignatureInput>.CreateNew().Build();

            var iuguSignature = Builder<Signature>.CreateNew().Build();
            var iuguCustomer = Builder<Customer>.CreateNew().Build();
            var iuguPaymentMethod = Builder<PaymentMethod>.CreateNew().Build();

            var user = UserFaker.CreateUser;
            iuguCustomer.Id = null;
            user.IuguCustomerId = null;

            _signatureDomainServiceMock.Setup(x => x.PostUserAsync(It.IsAny<User>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguCustomer);
            _signatureDomainServiceMock.Setup(x => x.PostUserPaymentMethodAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguPaymentMethod);
            _signatureDomainServiceMock.Setup(x => x.PostSignatureMethodAsync(It.IsAny<Plan>(), It.IsAny<string>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguSignature);

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(user);
            _userDomainServiceMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);
            _signatureDomainServiceMock.Setup(x => x.UpdateUserToPremiumPlanAsync(It.IsAny<User>(), It.IsAny<Plan>(), null)).ReturnsAsync(user);

            _planDomainServiceMock.Setup(x => x.GetPremiumPlanAsync(It.IsAny<int>())).ReturnsAsync(new Plan());

            // act
            var result = await _signatureApplication.PostSignatureAsync(input);

            // assert
            Assert.Null(result);
            //Assert.Equal("Falha ao criar o usuário na Iugu", result.Message);
            }

        [Fact(DisplayName = "Shoud return null when cant create iugu payment method")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-PostSignatureAsync")]
        public async Task ShouldReturnNullWhenCantCreateIuguPaymentMethod()
            {
            // arrange
            var input = Builder<SignatureInput>.CreateNew().Build();

            var iuguSignature = Builder<Signature>.CreateNew().Build();
            var iuguCustomer = Builder<Customer>.CreateNew().Build();
            var iuguPaymentMethod = Builder<PaymentMethod>.CreateNew().Build();

            var user = UserFaker.CreateUser;
            iuguPaymentMethod.Id = null;

            _signatureDomainServiceMock.Setup(x => x.PostUserAsync(It.IsAny<User>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguCustomer);
            _signatureDomainServiceMock.Setup(x => x.PostUserPaymentMethodAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguPaymentMethod);
            _signatureDomainServiceMock.Setup(x => x.PostSignatureMethodAsync(It.IsAny<Plan>(), It.IsAny<string>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguSignature);

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(user);
            _userDomainServiceMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);
            _signatureDomainServiceMock.Setup(x => x.UpdateUserToPremiumPlanAsync(It.IsAny<User>(), It.IsAny<Plan>(), null)).ReturnsAsync(user);

            _planDomainServiceMock.Setup(x => x.GetPremiumPlanAsync(It.IsAny<int>())).ReturnsAsync(new Plan());

            // act
            var result = await _signatureApplication.PostSignatureAsync(input);

            // assert
            Assert.Null(result);
            //Assert.Equal("Falha ao criar o método de pagamento na Iugu", result.Message);
            }

        [Fact(DisplayName = "Shoud return null when cant create iugu signature")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-PostSignatureAsync")]
        public async Task ShouldReturnNullWhenCantCreateIuguSignature()
            {
            // arrange
            var input = Builder<SignatureInput>.CreateNew().Build();

            var iuguSignature = Builder<Signature>.CreateNew().Build();
            var iuguCustomer = Builder<Customer>.CreateNew().Build();
            var iuguPaymentMethod = Builder<PaymentMethod>.CreateNew().Build();

            var user = UserFaker.CreateUser;
            iuguSignature.Id = null;

            _signatureDomainServiceMock.Setup(x => x.PostUserAsync(It.IsAny<User>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguCustomer);
            _signatureDomainServiceMock.Setup(x => x.PostUserPaymentMethodAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguPaymentMethod);
            _signatureDomainServiceMock.Setup(x => x.PostSignatureMethodAsync(It.IsAny<Plan>(), It.IsAny<string>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguSignature);

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(user);
            _userDomainServiceMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);
            _signatureDomainServiceMock.Setup(x => x.UpdateUserToPremiumPlanAsync(It.IsAny<User>(), It.IsAny<Plan>(), null)).ReturnsAsync(user);

            _planDomainServiceMock.Setup(x => x.GetPremiumPlanAsync(It.IsAny<int>())).ReturnsAsync(new Plan());

            // act
            var result = await _signatureApplication.PostSignatureAsync(input);

            // assert
            Assert.Null(result);
            //Assert.Equal("Falha na criação da assinatura", result.Message);
            }

        [Fact(DisplayName = "Shoud return null when iugu signature is'nt paid")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-PostSignatureAsync")]
        public async Task ShouldReturnNullWhenIuguSignatureIsntPaid()
            {
            // arrange
            var input = Builder<SignatureInput>.CreateNew().Build();

            var iuguSignature = Builder<Signature>.CreateNew().Build();
            var iuguCustomer = Builder<Customer>.CreateNew().Build();
            var iuguPaymentMethod = Builder<PaymentMethod>.CreateNew().Build();
            var recentInvoices = Builder<Invoice>.CreateListOfSize(1).Build();
            iuguSignature.RecentInvoices = recentInvoices.ToList();
            iuguSignature.RecentInvoices.First().Status = "pending";
            var user = UserFaker.CreateUser;

            _signatureDomainServiceMock.Setup(x => x.PostUserAsync(It.IsAny<User>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguCustomer);
            _signatureDomainServiceMock.Setup(x => x.PostUserPaymentMethodAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguPaymentMethod);
            _signatureDomainServiceMock.Setup(x => x.PostSignatureMethodAsync(It.IsAny<Plan>(), It.IsAny<string>(), It.IsAny<IuguInput>())).ReturnsAsync(iuguSignature);

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(user);
            _userDomainServiceMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);
            _signatureDomainServiceMock.Setup(x => x.UpdateUserToPremiumPlanAsync(It.IsAny<User>(), It.IsAny<Plan>(), null)).ReturnsAsync(user);

            _planDomainServiceMock.Setup(x => x.GetPremiumPlanAsync(It.IsAny<int>())).ReturnsAsync(new Plan());

            // act
            var result = await _signatureApplication.PostSignatureAsync(input);

            // assert
            Assert.NotNull(result.IuguSignature.Id);
            //Assert.Equal("Falha na cobrança", result.Message);
            }

        [Fact(DisplayName = "Should Return SignatureViewModel With Suspend False When Post Change Status Signature Async With Active Status")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-PostSignatureAsync")]
        public async Task ShouldReturnSignatureViewModelWithSuspendFalseWhenPostChangeStatusSignatureASyncWithActiveStatus()
            {
            // arrange
            var input = Builder<SignatureInput>.CreateNew().Build();
            var userFaker = UserFaker.CreateUser;
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            var user = UserFaker.CreateUser;

            var iuguSignature = Builder<Signature>.CreateNew().Build();

            var actionStatus = "active";
            _signatureDomainServiceMock.Setup(x => x.GetUserPlanWithIuguIdByUserId(It.IsAny<int>())).ReturnsAsync(userPlan);
            _signatureDomainServiceMock.Setup(x => x.PostChangeStatusSignatureAsync(It.IsAny<IuguInput>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(iuguSignature);

            // act
            var result = await _signatureApplication.PostChangeStatusSignatureAsync(input, actionStatus);

            // assert
            Assert.NotNull(result);
            Assert.NotNull(result.IuguSignature);

            //Assert.Equal("Falha na criação da assinatura", result.Message);
            }

        [Fact(DisplayName = "Should Return SignatureViewModel Null When Cant Find User Plan When Post Change Status Signature Async")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-PostSignatureAsync")]
        public async Task ShouldReturnSignatureViewModelNullWhenCantFindUserPlanWhenPostChangeStatusSignatureAsync()
            {
            // arrange
            var input = Builder<SignatureInput>.CreateNew().Build();
            var userFaker = UserFaker.CreateUser;
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            var user = UserFaker.CreateUser;

            var iuguSignature = Builder<Signature>.CreateNew().Build();

            var actionStatus = "active";
            _signatureDomainServiceMock.Setup(x => x.GetUserPlanWithIuguIdByUserId(It.IsAny<int>())).ReturnsAsync((UserPlans)null);
            _signatureDomainServiceMock.Setup(x => x.PostChangeStatusSignatureAsync(It.IsAny<IuguInput>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(iuguSignature);

            // act
            var result = await _signatureApplication.PostChangeStatusSignatureAsync(input, actionStatus);

            // assert
            Assert.Null(result);

            //Assert.Equal("Falha na criação da assinatura", result.Message);
            }

        [Fact(DisplayName = "Should Return SignatureViewModel Null When Invalid Status When Post Change Status Signature Async")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-PostSignatureAsync")]
        public async Task ShouldReturnSignatureViewModelNullWhenInvalidStatusWhenPostChangeStatusSignatureAsync()
            {
            // arrange
            var input = Builder<SignatureInput>.CreateNew().Build();
            var userFaker = UserFaker.CreateUser;
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            var user = UserFaker.CreateUser;

            var iuguSignature = Builder<Signature>.CreateNew().Build();

            var actionStatus = "unknown";
            _signatureDomainServiceMock.Setup(x => x.GetUserPlanWithIuguIdByUserId(It.IsAny<int>())).ReturnsAsync(userPlan);
            _signatureDomainServiceMock.Setup(x => x.PostChangeStatusSignatureAsync(It.IsAny<IuguInput>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(iuguSignature);

            // act
            var result = await _signatureApplication.PostChangeStatusSignatureAsync(input, actionStatus);

            // assert
            Assert.Null(result);

            //Assert.Equal("Falha na criação da assinatura", result.Message);
            }

        [Fact(DisplayName = "Shoud return User Plan paid when process iugu webhook with Iugu paid webhook")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-ProcessIuguWebhookAsync")]
        public async Task ShouldReturnUserPlanWhenProcessIuguWebhookAsyncWithIuguPaidWebhook()
            {
            // arrange
            var userFaker = UserFaker.CreateUser;
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            userPlan.StatusPayment = (sbyte)BoolEnum.NO;

            IuguWebhookInput input = Builder<IuguWebhookInput>.CreateNew().Build();
            input.Data = Builder<IuguWebhookDataInput>.CreateNew().Build();
            input.Data.Status = "paid";
            input.Event = "invoice.status_changed";

            var webhook = Builder<Webhook>.CreateNew().Build();

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(userFaker);
            
            _signatureDomainServiceMock.Setup(x => x.GetUserPlanByPartnerSignatureId(It.IsAny<string>())).ReturnsAsync(userPlan);
            _signatureDomainServiceMock.Setup(x => x.UpdateAsync(It.IsAny<UserPlans>())).ReturnsAsync(userPlan);

            _webhookDomainServiceMock.Setup(x => x.InsertAsync(It.IsAny<Webhook>())).ReturnsAsync(webhook);

            // act
            var result = await _signatureApplication.ProcessIuguWebhookAsync(input);

            // assert
            Assert.NotNull(result);
            Assert.Equal((sbyte)BoolEnum.YES, result.StatusPayment);
            }

        [Fact(DisplayName = "Shoud return User Plan paid when process iugu webhook with Iugu paid webhook when already paid plan")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-ProcessIuguWebhookAsync")]
        public async Task ShouldReturnUserPlanWhenProcessIuguWebhookAsyncWithIuguPaidWebhookAndAlreadyPaidPlan()
            {
            // arrange
            var userFaker = UserFaker.CreateUser;
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            userPlan.StatusPayment = (sbyte)BoolEnum.YES;

            IuguWebhookInput input = Builder<IuguWebhookInput>.CreateNew().Build();
            input.Data = Builder<IuguWebhookDataInput>.CreateNew().Build();
            input.Data.Status = "paid";
            input.Event = "invoice.status_changed";

            var webhook = Builder<Webhook>.CreateNew().Build();

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(userFaker);

            _signatureDomainServiceMock.Setup(x => x.GetUserPlanByPartnerSignatureId(It.IsAny<string>())).ReturnsAsync(userPlan);
            _signatureDomainServiceMock.Setup(x => x.UpdateAsync(It.IsAny<UserPlans>())).ReturnsAsync(userPlan);

            _webhookDomainServiceMock.Setup(x => x.InsertAsync(It.IsAny<Webhook>())).ReturnsAsync(webhook);

            // act
            var result = await _signatureApplication.ProcessIuguWebhookAsync(input);

            // assert
            Assert.NotNull(result);
            Assert.Equal((sbyte)BoolEnum.YES, result.StatusPayment);
            }

        [Fact(DisplayName = "Shoud return User Plan not paid when process iugu webhook with Iugu pending webhook")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-ProcessIuguWebhookAsync")]
        public async Task ShouldReturnNotPaidWhenCouldProcessIuguWebhookAsyncWithIuguPendingWebhook()
            {
            // arrange
            var userFaker = UserFaker.CreateUser;
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            userPlan.StatusPayment = (sbyte)BoolEnum.NO;

            IuguWebhookInput input = Builder<IuguWebhookInput>.CreateNew().Build();
            input.Data = Builder<IuguWebhookDataInput>.CreateNew().Build();
            input.Data.Status = "pending";
            input.Event = "invoice.status_changed";

            var webhook = Builder<Webhook>.CreateNew().Build();

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(userFaker);

            _signatureDomainServiceMock.Setup(x => x.GetUserPlanByPartnerSignatureId(It.IsAny<string>())).ReturnsAsync(userPlan);
            _signatureDomainServiceMock.Setup(x => x.UpdateAsync(It.IsAny<UserPlans>())).ReturnsAsync(userPlan);

            _webhookDomainServiceMock.Setup(x => x.InsertAsync(It.IsAny<Webhook>())).ReturnsAsync(webhook);

            // act
            var result = await _signatureApplication.ProcessIuguWebhookAsync(input);

            // assert
            Assert.NotNull(result);
            Assert.Equal((sbyte)BoolEnum.NO, result.StatusPayment);
            }

        [Fact(DisplayName = "Shoud return null when cant find user plan when process iugu webhook with Iugu invoice status changed webhook")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-ProcessIuguWebhookAsync")]
        public async Task ShouldReturnNullWhenCantFindSignatureWhenProcessIuguWebhookInvoiceStatusChangedAsyncWithIuguPendingWebhook()
            {
            // arrange
            var userFaker = UserFaker.CreateUser;
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            userPlan.StatusPayment = (sbyte)BoolEnum.NO;

            IuguWebhookInput input = Builder<IuguWebhookInput>.CreateNew().Build();
            input.Data = Builder<IuguWebhookDataInput>.CreateNew().Build();
            input.Data.Status = "pending";
            input.Event = "invoice.status_changed";

            var webhook = Builder<Webhook>.CreateNew().Build();

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(userFaker);

            _signatureDomainServiceMock.Setup(x => x.GetUserPlanByPartnerSignatureId(It.IsAny<string>())).ReturnsAsync((UserPlans)null);
            _signatureDomainServiceMock.Setup(x => x.UpdateAsync(It.IsAny<UserPlans>())).ReturnsAsync(userPlan);

            _webhookDomainServiceMock.Setup(x => x.InsertAsync(It.IsAny<Webhook>())).ReturnsAsync(webhook);

            // act
            var result = await _signatureApplication.ProcessIuguWebhookAsync(input);

            // assert
            Assert.Null(result);
            }

        [Fact(DisplayName = "Shoud return null with unknow received event when process iugu webhook")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-ProcessIuguWebhookAsync")]
        public async Task ShouldReturnNullWithUnknowReceivedEventWhenProcessIuguWebhookAsync()
            {
            // arrange
            var userFaker = UserFaker.CreateUser;
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            userPlan.StatusPayment = (sbyte)BoolEnum.NO;

            IuguWebhookInput input = Builder<IuguWebhookInput>.CreateNew().Build();
            input.Data = Builder<IuguWebhookDataInput>.CreateNew().Build();
            input.Data.Status = "paid";

            var webhook = Builder<Webhook>.CreateNew().Build();

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(userFaker);

            _signatureDomainServiceMock.Setup(x => x.GetUserPlanByPartnerSignatureId(It.IsAny<string>())).ReturnsAsync(userPlan);
            _signatureDomainServiceMock.Setup(x => x.UpdateAsync(It.IsAny<UserPlans>())).ReturnsAsync(userPlan);

            _webhookDomainServiceMock.Setup(x => x.InsertAsync(It.IsAny<Webhook>())).ReturnsAsync(webhook);

            // act
            var result = await _signatureApplication.ProcessIuguWebhookAsync(input);

            // assert
            Assert.Null(result);
            }

        [Fact(DisplayName = "Shoud return User Plan when process iugu webhook with Iugu renewed webhook")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-ProcessIuguWebhookAsync")]
        public async Task ShouldReturnUserPlanWhenProcessIuguWebhookAsyncWithIuguRenewedWebhook()
            {
            // arrange
            var userFaker = UserFaker.CreateUser;
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            var plan = Builder<Plan>.CreateNew().Build();
            var planType = Builder<PlanType>.CreateNew().Build();
            planType.Days = 30;
            plan.PlanType = planType;
            userPlan.StatusPayment = (sbyte)BoolEnum.YES;
            userPlan.DueDateAt = DateTime.Now.AddDays(-1);
            userPlan.CreatedAt = DateTime.Now.AddDays(-1);
            userPlan.UpdatedAt = DateTime.Now.AddDays(-1);

            IuguWebhookInput input = Builder<IuguWebhookInput>.CreateNew().Build();
            input.Data = Builder<IuguWebhookDataInput>.CreateNew().Build();
            input.Event = "subscription.renewed";

            var webhook = Builder<Webhook>.CreateNew().Build();

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(userFaker);
            _planDomainServiceMock.Setup(x => x.GetPremiumPlanAsync(It.IsAny<int>())).ReturnsAsync(plan);

            _signatureDomainServiceMock.Setup(x => x.GetUserPlanByPartnerSignatureId(It.IsAny<string>())).ReturnsAsync(userPlan);
            _signatureDomainServiceMock.Setup(x => x.UpdateUserToPremiumPlanAsync(It.IsAny<User>(), It.IsAny<Plan>(), It.IsAny<string>())).ReturnsAsync(userFaker);

            _webhookDomainServiceMock.Setup(x => x.InsertAsync(It.IsAny<Webhook>())).ReturnsAsync(webhook);

            // act
            var result = await _signatureApplication.ProcessIuguWebhookAsync(input);

            // assert
            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Shoud return User Plan when process iugu webhook with Iugu renewed webhook earlier than user plan created at")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-ProcessIuguWebhookAsync")]
        public async Task ShouldReturnNullWhenProcessIuguWebhookAsyncWithIuguRenewedWebhookEarlierThanUserPlanCreatedAt()
            {
            // arrange
            var userFaker = UserFaker.CreateUser;
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            var plan = Builder<Plan>.CreateNew().Build();
            var planType = Builder<PlanType>.CreateNew().Build();
            planType.Days = 30;
            plan.PlanType = planType;
            userPlan.StatusPayment = (sbyte)BoolEnum.YES;
            userPlan.DueDateAt = DateTime.Now.AddDays(1);
            userPlan.CreatedAt = DateTime.Now.AddDays(1);
            userPlan.UpdatedAt = DateTime.Now.AddDays(1);

            IuguWebhookInput input = Builder<IuguWebhookInput>.CreateNew().Build();
            input.Data = Builder<IuguWebhookDataInput>.CreateNew().Build();
            input.Event = "subscription.renewed";

            var webhook = Builder<Webhook>.CreateNew().Build();

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(userFaker);
            _planDomainServiceMock.Setup(x => x.GetPremiumPlanAsync(It.IsAny<int>())).ReturnsAsync(plan);

            _signatureDomainServiceMock.Setup(x => x.GetUserPlanByPartnerSignatureId(It.IsAny<string>())).ReturnsAsync(userPlan);
            _signatureDomainServiceMock.Setup(x => x.UpdateUserToPremiumPlanAsync(It.IsAny<User>(), It.IsAny<Plan>(), It.IsAny<string>())).ReturnsAsync(userFaker);

            _webhookDomainServiceMock.Setup(x => x.InsertAsync(It.IsAny<Webhook>())).ReturnsAsync(webhook);

            // act
            var result = await _signatureApplication.ProcessIuguWebhookAsync(input);

            // assert
            Assert.Null(result);
            }

        [Fact(DisplayName = "Shoud return User Plan when process iugu webhook with Iugu renewed webhook at same day than user plan created")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-ProcessIuguWebhookAsync")]
        public async Task ShouldReturnNullWhenProcessIuguWebhookAsyncWithIuguRenewedWebhookAtSameDayThanUserPlanCreated()
            {
            // arrange
            var userFaker = UserFaker.CreateUser;
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            var plan = Builder<Plan>.CreateNew().Build();
            var planType = Builder<PlanType>.CreateNew().Build();
            planType.Days = 30;
            plan.PlanType = planType;
            userPlan.StatusPayment = (sbyte)BoolEnum.YES;
            userPlan.DueDateAt = DateTime.Now;

            IuguWebhookInput input = Builder<IuguWebhookInput>.CreateNew().Build();
            input.Data = Builder<IuguWebhookDataInput>.CreateNew().Build();
            input.Event = "subscription.renewed";

            var webhook = Builder<Webhook>.CreateNew().Build();

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(userFaker);
            _planDomainServiceMock.Setup(x => x.GetPremiumPlanAsync(It.IsAny<int>())).ReturnsAsync(plan);

            _signatureDomainServiceMock.Setup(x => x.GetUserPlanByPartnerSignatureId(It.IsAny<string>())).ReturnsAsync(userPlan);
            _signatureDomainServiceMock.Setup(x => x.UpdateUserToPremiumPlanAsync(It.IsAny<User>(), It.IsAny<Plan>(), It.IsAny<string>())).ReturnsAsync(userFaker);

            _webhookDomainServiceMock.Setup(x => x.InsertAsync(It.IsAny<Webhook>())).ReturnsAsync(webhook);

            // act
            var result = await _signatureApplication.ProcessIuguWebhookAsync(input);

            // assert
            Assert.Null(result);
            }

        [Fact(DisplayName = "Shoud return null when can't find user plan when process iugu webhook with Iugu renewed webhook")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-ProcessIuguWebhookAsync")]
        public async Task ShouldReturnNullWhenCantFindUserPlanWhenProcessIuguWebhookAsyncWithIuguRenewedWebhook()
            {
            // arrange
            var userFaker = UserFaker.CreateUser;
            var userPlan = new UserPlans();
            var plan = Builder<Plan>.CreateNew().Build();
            var planType = Builder<PlanType>.CreateNew().Build();
            planType.Days = 30;
            plan.PlanType = planType;
            userPlan.StatusPayment = (sbyte)BoolEnum.YES;
            userPlan.DueDateAt = DateTime.Now;

            IuguWebhookInput input = Builder<IuguWebhookInput>.CreateNew().Build();
            input.Data = Builder<IuguWebhookDataInput>.CreateNew().Build();
            input.Event = "subscription.renewed";

            var webhook = Builder<Webhook>.CreateNew().Build();

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(userFaker);
            _planDomainServiceMock.Setup(x => x.GetPremiumPlanAsync(It.IsAny<int>())).ReturnsAsync(plan);

            _signatureDomainServiceMock.Setup(x => x.GetUserPlanByPartnerSignatureId(It.IsAny<string>())).ReturnsAsync((UserPlans)null);
            _signatureDomainServiceMock.Setup(x => x.UpdateUserToPremiumPlanAsync(It.IsAny<User>(), It.IsAny<Plan>(), It.IsAny<string>())).ReturnsAsync(userFaker);

            _webhookDomainServiceMock.Setup(x => x.InsertAsync(It.IsAny<Webhook>())).ReturnsAsync(webhook);

            // act
            var result = await _signatureApplication.ProcessIuguWebhookAsync(input);

            // assert
            Assert.Null(result);
            }

        [Fact(DisplayName = "Shoud return default when find user plan not paid when process iugu webhook with Iugu renewed webhook")]
        [Trait("[Application.AppServices]-SignatureApplication", "Application-ProcessIuguWebhookAsync")]
        public async Task ShouldReturnDefaultWhenFindUserPlanNotPaidWhenProcessIuguWebhookAsyncWithIuguRenewedWebhook()
            {
            // arrange
            var userFaker = UserFaker.CreateUser;
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            var plan = Builder<Plan>.CreateNew().Build();
            var planType = Builder<PlanType>.CreateNew().Build();
            planType.Days = 30;
            plan.PlanType = planType;
            userPlan.StatusPayment = (sbyte)BoolEnum.NO;
            userPlan.DueDateAt = DateTime.Now;

            IuguWebhookInput input = Builder<IuguWebhookInput>.CreateNew().Build();
            input.Data = Builder<IuguWebhookDataInput>.CreateNew().Build();
            input.Event = "subscription.renewed";

            var webhook = Builder<Webhook>.CreateNew().Build();

            _userDomainServiceMock.Setup(x => x.GetUserAsync(It.IsAny<int>())).ReturnsAsync(userFaker);
            _planDomainServiceMock.Setup(x => x.GetPremiumPlanAsync(It.IsAny<int>())).ReturnsAsync(plan);

            _signatureDomainServiceMock.Setup(x => x.GetUserPlanByPartnerSignatureId(It.IsAny<string>())).ReturnsAsync(userPlan);
            _signatureDomainServiceMock.Setup(x => x.UpdateUserToPremiumPlanAsync(It.IsAny<User>(), It.IsAny<Plan>(), It.IsAny<string>())).ReturnsAsync(userFaker);

            _webhookDomainServiceMock.Setup(x => x.InsertAsync(It.IsAny<Webhook>())).ReturnsAsync(webhook);

            // act
            var result = await _signatureApplication.ProcessIuguWebhookAsync(input);

            // assert
            Assert.Null(result);
            }
        }
    }
