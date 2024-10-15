using IntegrationTest.Config;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest.Scenarios.Plan
{
    public class PlanControllerIntegrationTest
    {
        private readonly TestContext _testContext;


        public PlanControllerIntegrationTest()
        {
            _testContext = new TestContext();
        }

        [Fact(DisplayName = "Should return premiun plans")]
        [Trait("[IntegrationTest]-PlanController", "GetAsync")]
        public async Task ShouldReturnPremiunPlans()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/plan/premium"
            };

            var user = await AuthLogin.GetUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);

            // act
            var response = await _testContext.Client.GetAsync($"{request.Url}");
            var result = await ContentHelper<IEnumerable<Domain.Entities.Plan>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var planLists = (IEnumerable<Domain.Entities.Plan>)result.Data;
            var planTitles = planLists.Select(x => x.Title).Distinct().ToList();
            Assert.Single(planTitles);
            Assert.Equal("Premium", planTitles.FirstOrDefault());
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Should return premiun plans without auth")]
        [Trait("[IntegrationTest]-PlanController", "GetNoAuthAsync")]
        public async Task ShouldReturnPremiunPlansWithoutAuth()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/plan/premium/without-auth"
            };

            // act
            var response = await _testContext.Client.GetAsync($"{request.Url}");
            var result = await ContentHelper<IEnumerable<Domain.Entities.Plan>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var planLists = (IEnumerable<Domain.Entities.Plan>)result.Data;
            var planTitles = planLists.Select(x => x.Title).Distinct().ToList();
            Assert.Single(planTitles);
            Assert.Equal("Premium", planTitles.FirstOrDefault());
            Assert.NotNull(result);
        }
    }
}
