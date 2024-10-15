using Application.AppServices.SignatureApplication.Input;
using Domain.Entities;
using IntegrationTest.Config;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest.Scenarios.Signature

    {
    public class SignatureControllerIntegrationTest
    {
        private readonly TestContext _testContext;


        public SignatureControllerIntegrationTest()
        {
            _testContext = new TestContext();
        }

        //[Fact(DisplayName = "Should return not found when parameter platform isn't informed")]
        //[Trait("[IntegrationTest]-SignatureController", "SignatureController")]
        //public async Task ShouldReturnNotFoundWhenParameterPlatformSyncIsntInformed()
        //{
        //    // arrange
        //    var request = new
        //    {
        //        Url = "/api/v1/signature/fovea/webhook"
        //        };

        //    var token = await AuthLogin.GetTokenUser(_testContext);
        //    _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        //    string jsonInput = $@"
        //            {{
        //                ""type"": ""purchases.updated"",
        //                ""applicationUsername"": ""renatolfilho@gmail.com"",
        //                ""purchases"": {{
        //                    ""google:premium_anual"": {{
        //                        ""productId"": ""google:premium_anual"",
        //                        ""purchaseId"": ""google:GPA.3328-1364-6149-70168"",
        //                        ""sandbox"": true,
        //                        ""platform"": ""google"",
        //                        ""purchaseDate"": ""2021-10-11T20:03:46.878Z"",
        //                        ""expirationDate"": ""2021-10-11T20:04:51.613Z"",
        //                        ""cancelationReason"": ""System.Replaced"",
        //                        ""isBillingRetryPeriod"": false,
        //                        ""isPending"": false,
        //                        ""renewalIntent"": ""Lapse"",
        //                        ""isAcknowledged"": true,
        //                        ""isExpired"": true
        //                    }},
        //                    ""google:premium_mensal"": {{
        //                        ""productId"": ""google:premium_mensal"",
        //                        ""purchaseId"": ""google:GPA.3382-0953-6326-35151"",
        //                        ""sandbox"": true,
        //                        ""platform"": ""google"",
        //                        ""purchaseDate"": ""2021-10-11T20:04:50.261Z"",
        //                        ""expirationDate"": ""2021-10-11T20:43:24.553Z"",
        //                        ""cancelationReason"": ""Customer.OtherReason"",
        //                        ""isBillingRetryPeriod"": false,
        //                        ""isPending"": false,
        //                        ""renewalIntent"": ""Lapse"",
        //                        ""renewalIntentChangeDate"": ""2021-10-11T20:11:17.004Z"",
        //                        ""isAcknowledged"": true,
        //                        ""isExpired"": false
        //                    }}
        //                }},
        //                ""password"": ""ae8f1cb7-2173-46bf-97cd-1472cb9cd98f""
        //            }}";

        //    var input = JsonSerializer.Deserialize<FoveaWebhookInput>(jsonInput);

        //    // act
        //    var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
        //    var result = await ContentHelper<UserPlans>.GetResponse(response);

        //    // assert
        //    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            //}
        }
}
