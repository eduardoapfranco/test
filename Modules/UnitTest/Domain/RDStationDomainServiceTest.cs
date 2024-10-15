using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Domain.Entities;
using Domain.Input.RDStation;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.UoW;
using Domain.Services;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using UnitTest.Application.UserApplication.Faker;
using Xunit;

namespace UnitTest.Domain
    {
    public class RDStationDomainServiceTest
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<RDStationDomainService>> _loggerMock;
        private RDStationDomainService _rdStationDomainService;
        private readonly Fixture _fixture;
        private HttpClient _httpClient;

        public RDStationDomainServiceTest()
        {
            _smartNotificationMock = new Mock<ISmartNotification>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<RDStationDomainService>>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _rdStationDomainService = new RDStationDomainService(_userRepositoryMock.Object, _smartNotificationMock.Object, _unitOfWorkMock.Object, new DomainNotificationHandler(), _loggerMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }

        [Fact(DisplayName = "Shoud return success when post RD Station conversion")]
        [Trait("[Domain.Services]-RDStationDomainService", "Domain-PostConversionAsync")]
        public async Task ShouldReturnSuccessWhenPostRDStationConversion()
        {
            //arrange
            User user = UserFaker.CreateUser;
            string responseContent = $@"
                        {{
                        ""event_type"": ""CONVERSION"",
                        ""event_family"":""CDP"",
                        ""event_uuid"":""event-uuid"",
                        ""payload"": {{
                        ""conversion_identifier"": ""Name of the conversion event"",
                        ""name"": ""name of the contact"",
                        ""email"": ""email@email.com"",
                        ""job_title"": ""job title value"",
                        ""state"": ""state of the contact"",
                        ""city"": ""city of the contact"",
                        ""country"": ""country of the contact"",
                        ""personal_phone"": ""phone of the contact"",
                        ""mobile_phone"": ""mobile_phone of the contact"",
                        ""twitter"": ""twitter handler of the contact"",
                        ""facebook"": ""facebook name of the contact"",
                        ""linkedin"": ""linkedin user name of the contact"",
                        ""website"": ""website of the contact"",
                        ""company_name"": ""company name"",
                        ""company_site"": ""company website"",
                        ""company_address"": ""company address"",
                        ""client_tracking_id"": ""lead tracking client_id"",
                        ""traffic_source"": ""Google"",
                        ""traffic_medium"": ""cpc"",
                        ""traffic_campaign"": ""easter-50-off"",
                        ""traffic_value"": ""easter eggs"",
                        ""tags"": [""mql"", ""2019""],
                        ""available_for_mailing"": true,
                        ""legal_bases"": [
                            {{
                            ""category"": ""communications"",
                            ""type"": ""consent"",
                            ""status"": ""granted""
                            }}
                        ],
                        ""cf_my_custom_field"": ""custom field value""
                        }}
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
            _rdStationDomainService._httpClient = _httpClient;

            RDStationInput input = new RDStationInput()
                {
                ApiSecret = "xyz",
                UrlBase = "http://apitest.construa.app"
                };

            //act
            var result = await _rdStationDomainService.PostConversionAsync(user, input);

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.EventUuid);
            }
    }
}
