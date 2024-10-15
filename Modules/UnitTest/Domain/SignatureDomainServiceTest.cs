using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Input;
using Domain.Input.Iugu;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.UoW;
using Domain.Services;
using FizzWare.NBuilder;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using UnitTest.Application.UserApplication.Faker;
using UnitTest.Domain.Faker;
using Xunit;

namespace UnitTest.Domain
    {
    public class SignatureDomainServiceTest
        {
        private Mock<IUserPlansRepository> _userPlansRepositoryMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private SignatureDomainService _signatureDomainService;
        private HttpClient _httpClient;
        private Mock<ILogger<SignatureDomainService>> _loggerMock;

        public SignatureDomainServiceTest()
            {
            _userPlansRepositoryMock = new Mock<IUserPlansRepository>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _loggerMock = new Mock<ILogger<SignatureDomainService>>();
            _signatureDomainService = new SignatureDomainService(_userPlansRepositoryMock.Object, _smartNotificationMock.Object, _unitOfWorkMock.Object, new DomainNotificationHandler(), _loggerMock.Object);
            }

        [Fact(DisplayName = "Shoud return iugu customer")]
        [Trait("[Domain.Services]-SignatureDomainService", "Signature-PostUserAsync")]
        public async Task ShouldReturnIuguCustomer()
            {
            //arrange
            User user = UserFaker.CreateUser;
            string responseContent = "{ \"id\": \"77C2565F6F064A26ABED4255894224F0\", \"email\": \""+ user.Email+ "\", \"name\":\"" + user.Name + "\", \"notes\": \"Anotações Gerais\", \"created_at\": \"2013-11-18T14:58:30-02:00\", \"updated_at\": \"2013-11-18T14:58:30-02:00\", \"custom_variables\":[] }";
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
                   {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(responseContent)
                   })
               .Verifiable();

            // use real http client with mocked handler here
            _httpClient = new HttpClient(handlerMock.Object)
                {
                BaseAddress = new Uri("http://test.com/"),
                };
            _signatureDomainService._httpClient = _httpClient;

            IuguInput input = new IuguInput()
                {
                Token = "65cdb06aba25521619e5f61dfc453878",
                UrlBase = "http://apitest.construa.app"
                };

            //act
            var result = await _signatureDomainService.PostUserAsync(user, input);

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Id);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Email, result.Email);
            }

        [Fact(DisplayName = "Shoud return iugu payment method")]
        [Trait("[Domain.Services]-SignatureDomainService", "Signature-PostUserPaymentMethodAsync")]
        public async Task ShouldReturnIuguPaymentMethod()
            {
            //arrange
            string iuguCustomerId = Guid.NewGuid().ToString();
            string paymentMethodToken = Guid.NewGuid().ToString();
            string paymentMethodId = Guid.NewGuid().ToString();
            string responseContent = "{\"id\": \"" + paymentMethodId + "\",\"description\": \"Meu Cartão de Crédito\",\"item_type\": \"credit_card\",\"data\": { \"holder_name\": \"Joao Silva\", \"display_number\": \"XXXX-XXXX-XXXX-1111\", \"brand\": \"visa\", \"month\":12,        \"year\":2022}}";
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
                   {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(responseContent)
                   })
               .Verifiable();

            // use real http client with mocked handler here
            _httpClient = new HttpClient(handlerMock.Object)
                {
                BaseAddress = new Uri("http://test.com/"),
                };
            _signatureDomainService._httpClient = _httpClient;

            IuguInput input = new IuguInput()
                {
                Token = "65cdb06aba25521619e5f61dfc453878",
                UrlBase = "http://apitest.construa.app"
                };

            //act
            var result = await _signatureDomainService.PostUserPaymentMethodAsync(iuguCustomerId, paymentMethodToken, input);

            // assert
            Assert.NotNull(result);
            Assert.NotEqual(String.Empty, result.Id);
            Assert.Equal(paymentMethodId, result.Id);
            }

        [Fact(DisplayName = "Shoud return iugu payment method errors when cant create iugu payment method")]
        [Trait("[Domain.Services]-SignatureDomainService", "Signature-PostUserPaymentMethodAsync")]
        public async Task ShouldReturnIuguPaymentMethodErrorsWhenCanCreateIuguPaymentMethod()
            {
            //arrange
            string iuguCustomerId = Guid.NewGuid().ToString();
            string paymentMethodToken = Guid.NewGuid().ToString();
            string paymentMethodId = Guid.NewGuid().ToString();
            string responseContent = "{\"errors\":{\"token\":[\"Esse token j\\u00e1 foi usado.\"]}}";
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
                   {
                   StatusCode = HttpStatusCode.UnprocessableEntity,
                   Content = new StringContent(responseContent)
                   })
               .Verifiable();

            // use real http client with mocked handler here
            _httpClient = new HttpClient(handlerMock.Object)
                {
                BaseAddress = new Uri("http://test.com/"),
                };
            _signatureDomainService._httpClient = _httpClient;

            IuguInput input = new IuguInput()
                {
                Token = "65cdb06aba25521619e5f61dfc453878",
                UrlBase = "http://apitest.construa.app"
                };

            //act
            var result = await _signatureDomainService.PostUserPaymentMethodAsync(iuguCustomerId, paymentMethodToken, input);

            // assert
            Assert.NotNull(result);
            Assert.Null(result.Id);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("token=Esse token já foi usado.", result.GetErrors());
            Assert.Equal("token", result.Errors.First().Key);
            Assert.Equal("Esse token já foi usado.", result.Errors.First().Value.First());
            }

        [Fact(DisplayName = "Shoud return iugu signature")]
        [Trait("[Domain.Services]-SignatureDomainService", "Signature-PostSignatureMethodAsync")]
        public async Task ShouldReturnIuguSignature()
            {
            //arrange
            Plan plan = PlanFaker.CreatePlanPremium();
            string iuguCustomerId = Guid.NewGuid().ToString();
            string iuguSignatureId = Guid.NewGuid().ToString();
            string responseContent = $@"
                        {{
                        ""id"": ""{iuguSignatureId}"",
                        ""suspended"": false,
                        ""plan_identifier"": ""id1"",
                        ""price_cents"": 200,
                        ""currency"": ""BRL"",
                        ""features"": {{
                            ""feat"": {{
                                ""name"": ""Feature"",
                                ""value"": 0
                            }}
                        }},
                        ""expires_at"": null,
                        ""created_at"": ""2013-11-19T11:24:29-02:00"",
                        ""updated_at"": ""2013-11-19T11:24:43-02:00"",
                        ""customer_name"": ""Nome do Cliente"",
                        ""customer_email"": ""email@email.com"",
                        ""cycled_at"": null,
                        ""credits_min"": 0,
                        ""credits_cycle"": null,
                        ""customer_id"": ""{iuguCustomerId}"",
                        ""plan_name"": ""plan1"",
                        ""customer_ref"": ""Nome do Cliente"",
                        ""plan_ref"": ""plan1"",
                        ""active"": true,
                        ""in_trial"": null,
                        ""credits"": 0,
                        ""credits_based"": false,
                        ""recent_invoices"": null,
                        ""subitems"": [{{
                            ""id"": ""6D518D88B33F48FEA8964D5573E220D3"",
                            ""description"": ""Item um"",
                            ""quantity"": 1,
                            ""price_cents"": 1000,
                            ""price"": ""R$ 10,00"",
                            ""total"": ""R$ 10,00""
                        }}],
                        ""logs"": [{{
                            ""id"": ""477388CC4C024520B552641724A62970"",
                            ""description"": ""Fatura criada"",
                            ""notes"": ""Fatura criada 1x Ativação de Assinatura: plan1 = R$ 2,00;1x Item um = R$ 10,00;"",
                            ""created_at"": ""2013-11-19T11:24:43-02:00""
                        }}, {{
                            ""id"": ""706436F169CE465B806163964A25400A"",
                            ""description"": ""Assinatura Criada"",
                            ""notes"": ""Assinatura Criada"",
                            ""created_at"": ""2013-11-19T11:24:29-02:00""
                        }}],
                        ""custom_variables"":[]
                    }}";
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
                   {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(responseContent)
                   })
               .Verifiable();

            // use real http client with mocked handler here
            _httpClient = new HttpClient(handlerMock.Object)
                {
                BaseAddress = new Uri("http://test.com/"),
                };
            _signatureDomainService._httpClient = _httpClient;

            IuguInput input = new IuguInput()
                {
                Token = "65cdb06aba25521619e5f61dfc453878",
                UrlBase = "http://apitest.construa.app"
                };

            //act
            var result = await _signatureDomainService.PostSignatureMethodAsync(plan, iuguCustomerId, input);

            // assert
            Assert.NotNull(result);
            Assert.NotEqual(String.Empty, result.Id);
            Assert.Equal(result.Id, iuguSignatureId);
            }

        [Fact(DisplayName = "Shoud return user plans with due date with 15 days when trial plan")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-GetUserPlanToUpdateUserToPremiumPlan")]
        public void ShouldReturnUserplansWithDueDateWith15DaysWhenTrialPlan()
            {
            // arrange 
            var user = Builder<User>.CreateNew().Build();
            var plan = Builder<Plan>.CreateNew().Build();
            var planType = Builder<PlanType>.CreateNew().Build();
            planType.Days = 15;
            plan.PlanType = planType;
            // act 
            var result = _signatureDomainService.GetUserPlanToUpdateUserToPremiumPlan(user, plan);

            // assert
            Assert.NotNull(result);
            Assert.Equal(DateTime.Now.AddDays(15).Date, result.DueDateAt.Date);
            }

        [Fact(DisplayName = "Shoud return user plans with due date with 1 month when monthly preemium plan")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-GetUserPlanToUpdateUserToPremiumPlan")]
        public void ShouldReturnUserplansWithDueDateWith1MonthWhenMonthlyPremiumPlan()
            {
            // arrange 
            var user = Builder<User>.CreateNew().Build();
            var plan = Builder<Plan>.CreateNew().Build();
            var planType = Builder<PlanType>.CreateNew().Build();
            planType.Days = 30;
            plan.PlanType = planType;
            // act 
            var result = _signatureDomainService.GetUserPlanToUpdateUserToPremiumPlan(user, plan);

            // assert
            Assert.NotNull(result);
            Assert.Equal(DateTime.Now.AddDays(30).Date, result.DueDateAt.Date);
            }

        [Fact(DisplayName = "Shoud return user plans with due date with 3 months when quarterly preemium plan")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-GetUserPlanToUpdateUserToPremiumPlan")]
        public void ShouldReturnUserplansWithDueDateWith3MonthsWhenQuarterlyPremiumPlan()
            {
            // arrange 
            var user = Builder<User>.CreateNew().Build();
            var plan = Builder<Plan>.CreateNew().Build();
            var planType = Builder<PlanType>.CreateNew().Build();
            planType.Days = 90;
            plan.PlanType = planType;
            // act 
            var result = _signatureDomainService.GetUserPlanToUpdateUserToPremiumPlan(user, plan);

            // assert
            Assert.NotNull(result);
            Assert.Equal(DateTime.Now.AddDays(90).Date, result.DueDateAt.Date);
            }

        [Fact(DisplayName = "Shoud return user plans with due date with 6 months when semiannual preemium plan")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-GetUserPlanToUpdateUserToPremiumPlan")]
        public void ShouldReturnUserplansWithDueDateWith6MonthsWhenSemiannualPremiumPlan()
            {
            // arrange 
            var user = Builder<User>.CreateNew().Build();
            var plan = Builder<Plan>.CreateNew().Build();
            var planType = Builder<PlanType>.CreateNew().Build();
            planType.Days = 180;
            plan.PlanType = planType;
            // act 
            var result = _signatureDomainService.GetUserPlanToUpdateUserToPremiumPlan(user, plan);

            // assert
            Assert.NotNull(result);
            Assert.Equal(DateTime.Now.AddDays(180).Date, result.DueDateAt.Date);
            }

        [Fact(DisplayName = "Shoud return user plans with due date with 12 months when anual preemium plan")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-GetUserPlanToUpdateUserToPremiumPlan")]
        public void ShouldReturnUserplansWithDueDateWith12MonthsWhenAnualPremiumPlan()
            {
            // arrange 
            var user = Builder<User>.CreateNew().Build();
            var plan = Builder<Plan>.CreateNew().Build();
            var planType = Builder<PlanType>.CreateNew().Build();
            planType.Days = 365;
            plan.PlanType = planType;
            // act 
            var result = _signatureDomainService.GetUserPlanToUpdateUserToPremiumPlan(user, plan);

            // assert
            Assert.NotNull(result);
            Assert.Equal(DateTime.Now.AddDays(365).Date, result.DueDateAt.Date);
            }
        }
    }
