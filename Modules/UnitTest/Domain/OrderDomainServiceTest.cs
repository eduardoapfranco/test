using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Application.AppServices.OrderApplication.Input.Pagseguro;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.UoW;
using Domain.Services;
using FizzWare.NBuilder;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using UnitTest.Application.OrderApplication.Faker;
using Xunit;

namespace UnitTest.Domain
    {
    public class OrderDomainServiceTest
        {
        private Mock<IWebhookRepository> _webhookRepositoryMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private WebhookDomainService _webhookDomainService;
        private HttpClient _httpClient;
        private Mock<ILogger<WebhookDomainService>> _loggerMock;

        public OrderDomainServiceTest()
            {
            _webhookRepositoryMock = new Mock<IWebhookRepository>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _loggerMock = new Mock<ILogger<WebhookDomainService>>();
            _webhookDomainService = new WebhookDomainService(_webhookRepositoryMock.Object, _smartNotificationMock.Object, _unitOfWorkMock.Object, new DomainNotificationHandler(), _loggerMock.Object);
            }

        [Fact(DisplayName = "Shoud return full pagseguro notification")]
        [Trait("[Domain.Services]-OrderDomainService", "Order-GetFullPagseguroNotificationAsync")]
        public async Task ShouldReturnFullPagseguroNotification()
            {
            // arrange
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
                   Content = new StringContent(@"<transaction>  
                                    <date>2011-02-10T16:13:41.000-03:00</date>  
                                    <code>9E884542-81B3-4419-9A75-BCC6FB495EF1</code>  
                                    <reference>REF1234</reference>  
                                    <type>1</type>  
                                    <status>3</status>  
                                    <lasteventdate>2011-02-15T17:39:14.000-03:00</lasteventdate>  
                                    <paymentmethod>  
                                        <type>1</type>  
                                        <code>101</code>  
                                    </paymentmethod>  
                                    <grossamount>49900.00</grossamount>  
                                    <discountamount>0.00</discountamount>  
                                    <feeamount>0.00</feeamount>  
                                    <netamount>49900.00</netamount>  
                                    <extraamount>0.00</extraamount>  
                                    <installmentcount>1</installmentcount>  
                                    <itemcount>2</itemcount>  
                                    <items>  
                                        <item>  
                                            <id>0001</id>  
                                            <description>Notebook Prata</description>  
                                            <quantity>1</quantity>  
                                            <amount>24300.00</amount>  
                                        </item>  
                                        <item>  
                                            <id>0002</id>  
                                            <description>Notebook Rosa</description>  
                                            <quantity>1</quantity>  
                                            <amount>25600.00</amount>  
                                        </item>  
                                    </items>  
                                    <sender>  
                                        <name>José Comprador</name>  
                                        <email>comprador@uol.com.br</email>  
                                        <phone>  
                                            <areacode>11</areacode>  
                                            <number>56273440</number>  
                                        </phone>  
                                    </sender>  
                                    <shipping>  
                                        <address>  
                                            <street>Av. Brig. Faria Lima</street>  
                                            <number>1384</number>  
                                            <complement>5o andar</complement>  
                                            <district>Jardim Paulistano</district>  
                                            <postalcode>01452002</postalcode>  
                                            <city>Sao Paulo</city>  
                                            <state>SP</state>  
                                            <country>BRA</country>  
                                        </address>  
                                        <type>1</type>  
                                        <cost>21.69</cost>  
                                    </shipping>  
                                </transaction> "),
                   })
               .Verifiable();

            // use real http client with mocked handler here
            _httpClient = new HttpClient(handlerMock.Object)
                {
                BaseAddress = new Uri("http://test.com/"),
                };
            _webhookDomainService._httpClient = _httpClient;

            var input = WebhookFaker.CreatePagSeguroNotificationInput();

            // act
            var result = await _webhookDomainService.GetFullPagseguroNotificationAsync(input);

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            }

        [Fact(DisplayName = "Shoud return empty when can't get full pagseguro notification")]
        [Trait("[Domain.Services]-OrderDomainService", "Order-GetFullPagseguroNotificationAsync")]
        public async Task ShouldReturnEmptylWhenCantGetFullPagseguroNotification()
            {
            // arrange
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
                   Content = new StringContent(String.Empty),
                   })
               .Verifiable();

            // use real http client with mocked handler here
            _httpClient = new HttpClient(handlerMock.Object)
                {
                BaseAddress = new Uri("http://test.com/"),
                };
            _webhookDomainService._httpClient = _httpClient;

            var input = WebhookFaker.CreatePagSeguroNotificationInput();

            // act
            var result = await _webhookDomainService.GetFullPagseguroNotificationAsync(input);

            // assert
            Assert.Empty(result);
            }

        [Fact(DisplayName = "Shoud return empty when can't get pagseguro checkout code")]
        [Trait("[Domain.Services]-OrderDomainService", "Order-GetPagseguroCheckoutCodeAsync")]
        public async Task ShouldReturnEmptyWhenCantGetPagseguroCheckoutCode()
            {
            // arrange
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
                   Content = new StringContent(String.Empty),
                   })
               .Verifiable();

            // use real http client with mocked handler here
            _httpClient = new HttpClient(handlerMock.Object)
                {
                BaseAddress = new Uri("http://test.com/"),
                };
            _webhookDomainService._httpClient = _httpClient;

            var checkout = Builder<Checkout>.CreateNew().Build();
            checkout.Shipping = Builder<TransactionShipping>.CreateNew().Build();
            checkout.Sender = Builder<TransactionSender>.CreateNew().Build();
            checkout.Items = new System.Collections.Generic.List<TransactionItem>()
                {
                Builder<TransactionItem>.CreateNew().Build()
                };

            var input = WebhookFaker.CreatePagSeguroNotificationInput();

            // act
            var result = await _webhookDomainService.GetPagseguroCheckoutCodeAsync(checkout.ToFormParameter(), input);

            // assert
            Assert.Empty(result);
            }

        [Fact(DisplayName = "Shoud return empty when get error in response of pagseguro checkout code")]
        [Trait("[Domain.Services]-OrderDomainService", "Order-GetPagseguroCheckoutCodeAsync")]
        public async Task ShouldReturnEmptyWhenCantGetErrorInResponseOfPagseguroCheckoutCode()
            {
            // arrange
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
                   StatusCode = HttpStatusCode.BadRequest,
                   Content = new StringContent(@"<?xml version=""1.0"" encoding=""ISO-8859-1"" standalone=""yes""?><errors>
                                    <error>
                                        <code>11029</code>
                                        <message>Item amount invalid pattern: 9,90. Must fit the patern: \d+.\d{2}</message>
                                    </error>
                                </errors>"),
                   })
               .Verifiable();

            // use real http client with mocked handler here
            _httpClient = new HttpClient(handlerMock.Object)
                {
                BaseAddress = new Uri("http://test.com/"),
                };
            _webhookDomainService._httpClient = _httpClient;

            var checkout = Builder<Checkout>.CreateNew().Build();
            checkout.Shipping = Builder<TransactionShipping>.CreateNew().Build();
            checkout.Sender = Builder<TransactionSender>.CreateNew().Build();
            checkout.Items = new System.Collections.Generic.List<TransactionItem>()
                {
                Builder<TransactionItem>.CreateNew().Build()
                };
            var input = WebhookFaker.CreatePagSeguroNotificationInput();

            // act
            var result = await _webhookDomainService.GetPagseguroCheckoutCodeAsync(checkout.ToFormParameter(), input);

            // assert
            Assert.Empty(result);
            }

        [Fact(DisplayName = "Shoud return empty when get error in response of pagseguro checkout code")]
        [Trait("[Domain.Services]-OrderDomainService", "Order-GetPagseguroCheckoutCodeAsync")]
        public async Task ShouldReturnCheckoutCodeWhenGetPagseguroCheckoutCodeAsync()
            {
            // arrange
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
                   Content = new StringContent(@"<?xml version=""1.0"" encoding=""ISO-8859-1"" standalone=""yes""?><checkout>
                            <code>8BCB295577774CD664601F856AA0AE07</code>
                            <date>2020-11-10T10:07:18.000-03:00</date>
                        </checkout>"),
                   })
               .Verifiable();

            // use real http client with mocked handler here
            _httpClient = new HttpClient(handlerMock.Object)
                {
                BaseAddress = new Uri("http://test.com/"),
                };
            _webhookDomainService._httpClient = _httpClient;

            var checkout = Builder<Checkout>.CreateNew().Build();
            checkout.Shipping = Builder<TransactionShipping>.CreateNew().Build();
            checkout.Sender = Builder<TransactionSender>.CreateNew().Build();
            checkout.Items = new System.Collections.Generic.List<TransactionItem>()
                {
                Builder<TransactionItem>.CreateNew().Build()
                };
            var input = WebhookFaker.CreatePagSeguroNotificationInput();

            // act
            var result = await _webhookDomainService.GetPagseguroCheckoutCodeAsync(checkout.ToFormParameter(), input);

            // assert
            Assert.Equal("8BCB295577774CD664601F856AA0AE07", result);
            }
        }
    }
