using Application.AppServices.OrderApplication.Input;
using Infra.CrossCutting.Controllers;
using IntegrationTest.Config;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest.Scenarios.Order

    {
    public class OrderControllerIntegrationTest
    {
        private readonly TestContext _testContext;


        public OrderControllerIntegrationTest()
        {
            _testContext = new TestContext();
        }

        //[Fact(DisplayName = "Should return success when process pagseguro notification")]
        //[Trait("[IntegrationTest]-WebhookController", "WebhookController")]
        //public async Task ShouldReturnSuccessWhenProcessPagseguroNotification()
        //    {
        //    // arrange
        //    var request = new
        //        {
        //        Url = "/api/v1/webhooks/pagseguro/notifications"
        //        };

        //    var formParameters = new Dictionary<string, string>();
        //    formParameters.Add("NotificationCode", "notificationCode");
        //    formParameters.Add("NotificationType", "transaction");

        //    // act
        //    var response = await _testContext.Client.PostAsync(request.Url, new FormUrlEncodedContent(formParameters));
        //    var result = await ContentHelper<bool>.GetResponse(response);

        //    // assert
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    Assert.False(result.Error);
        //    Assert.NotNull(result);
        //    Assert.True(result.Data);
        //    }
    }
}
