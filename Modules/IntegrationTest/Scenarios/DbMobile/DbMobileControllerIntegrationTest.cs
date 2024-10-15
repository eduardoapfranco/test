using Application.AppServices.DbMobileApplication.Input;
using Application.AppServices.DbMobileApplication.ViewModel;
using Infra.CrossCutting.Auth;
using IntegrationTest.Config;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest.Scenarios.Auth
{
    public class DbMobileControllerIntegrationTest
    {
        private readonly TestContext _testContext;


        public DbMobileControllerIntegrationTest()
        {
            _testContext = new TestContext();
        }

        //[Fact(DisplayName = "Should return success when user request create db mobile")]
        //[Trait("[IntegrationTest]-DbMobileController", "DbMobileController")]
        //public async Task ShouldReturnSuccessWhenUserRequestCreateDbMobile()
        //{
        //    // arrange
        //    var request = new
        //    {
        //        Url = "/api/v1/db-mobile/generate"
        //    };

        //    var input = new DbMobileInput()
        //    {
        //        Secret = AuthSettings.SecretGenerateDB
        //    };

        //    // act
        //    var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
        //    var result = await ContentHelper<DbMobileViewModel>.GetResponse(response);

        //    // assert
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    Assert.False(result.Error);
        //}

        [Fact(DisplayName = "Should return invalid secret create db mobile")]
        [Trait("[IntegrationTest]-DbMobileController", "DbMobileController")]
        public async Task ShouldReturnInvalidSecretCreateDbMobile()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/db-mobile/generate"
            };

            var input = new DbMobileInput()
            {
                Secret = null
            };

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<DbMobileViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result.Error);
            Assert.Contains("'A chave de criptografia é obrigatória!'", result.Messages[0]);
        }

        [Fact(DisplayName = "Should return download URL create db mobile")]
        [Trait("[IntegrationTest]-DbMobileController", "DbMobileController")]
        public async Task ShouldReturnDownloadUrlCreateDbMobile()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/db-mobile/download"
            };

            var token = await AuthLogin.GetTokenUser(_testContext);

           
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<DbMobileViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);        
        }

        [Fact(DisplayName = "Should return last updated dates")]
        [Trait("[IntegrationTest]-DbMobileController", "DbMobileController")]
        public async Task ShouldReturnLastUpdatedDates()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/db-mobile/last-updated-dates"
                };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<LastUpdatedDatesViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            }
        }
}
