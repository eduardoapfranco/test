using Application.AppServices.AreaApplication.ViewModel;
using Infra.CrossCutting.Controllers;
using IntegrationTest.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest.Scenarios.Area

    {
    public class AreaControllerIntegrationTest
        {
        private readonly TestContext _testContext;


        public AreaControllerIntegrationTest()
            {
            _testContext = new TestContext();
            }
        [Fact(DisplayName = "Should return not authorized when try to get areas list without token")]
        [Trait("[IntegrationTest]-AreaController", "AreaController")]
        public async Task ShouldReturnNotAuthorizedWhenTryToGetAreasListWithoutToken()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/area"
                };

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<AreaViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

        [Fact(DisplayName = "Should return list of areas async")]
        [Trait("[IntegrationTest]-AreaController", "AreaController")]
        public async Task ShouldReturnListOfAreasToSync()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/area"
                };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<AreaViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<List<AreaViewModel>>>(result);
            }
        }
    }
