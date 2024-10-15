using Application.AppServices.VersionApplication.ViewModel;
using Infra.CrossCutting.Controllers;
using IntegrationTest.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest.Scenarios.Version

    {
    public class VersionControllerIntegrationTest
    {
        private readonly TestContext _testContext;


        public VersionControllerIntegrationTest()
        {
            _testContext = new TestContext();
        }

        [Fact(DisplayName = "Should return not found when parameter platform isn't informed")]
        [Trait("[IntegrationTest]-VersionController", "VersionController")]
        public async Task ShouldReturnNotFoundWhenParameterPlatformSyncIsntInformed()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/version/"
            };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<VersionViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(DisplayName = "Should return empty when does'nt exist any version to informed platform")]
        [Trait("[IntegrationTest]-VersionController", "VersionController")]
        public async Task ShouldReturnEmptyWhenDoesntExistVersionsToInformedPlatform()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/version/notexists"
            };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<VersionViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
        }

        [Fact(DisplayName = "Should return last android version")]
        [Trait("[IntegrationTest]-VersionController", "VersionController")]
        public async Task ShouldReturnLastAndroidVersion()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/version/android"
            };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<VersionViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<VersionViewModel>>(result);
        }

        [Fact(DisplayName = "Should return specified ios version")]
        [Trait("[IntegrationTest]-VersionController", "VersionController")]
        public async Task ShouldReturnSpecifiedIOSVersion()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/version/ios/1.0.0"
                };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<VersionViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<VersionViewModel>>(result);
            }
        }
}
