using Application.AppServices.RatingApplication.Input;
using Application.AppServices.RatingApplication.ViewModel;
using IntegrationTest.Config;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;

namespace IntegrationTest.Scenarios.Rating
    {
    public class RatingControllerIntegrationTest
    {
        private readonly TestContext _testContext;


        public RatingControllerIntegrationTest()
        {
            _testContext = new TestContext();
        }

        [Fact(DisplayName = "Should return not authorized when try to post rating without token")]
        [Trait("[IntegrationTest]-RatingController", "RatingController")]
        public async Task ShouldReturnNotAuthorizedWhenTryToPostRatingWithouToken()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/rating"
                };

            var input = new RatingInput()
                {
                _Rating = 5,
                Comment = "Comment",
                Title = "Title",
                CategoryId = 191,
                UserId = 1
                };

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<RatingViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

        [Fact(DisplayName = "Should return success when post rating with category")]
        [Trait("[IntegrationTest]-RatingController", "RatingController")]
        public async Task ShouldReturnSuccessWhenPostRatingWithCategory()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/rating"
            }; 

            var input = new RatingInput()
            {
                _Rating = 5,
                Comment = "Comment",
                Title = "Title",
                CategoryId = 191,
                UserId = 1
            };

            var jsonInput = JsonConvert.SerializeObject(input);
            var contentString = new StringContent(jsonInput, Encoding.UTF8, "application/json");

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.PostAsync(request.Url, contentString);
            var result = await ContentHelper<RatingViewModel>.GetResponse(response);          

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.IsType<RatingViewModel>(result.Data);
            Assert.NotNull(result.Data.Id);
            Assert.False(result.Error);
            Assert.Equal("Comment", result.Data.Comment);
        }

        [Fact(DisplayName = "Should return success when post rating with checklist")]
        [Trait("[IntegrationTest]-RatingController", "RatingController")]
        public async Task ShouldReturnSuccessWhenPostRatingWithChecklist()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/rating"
                };

            var input = new RatingInput()
                {
                _Rating = 5,
                Comment = "Comment",
                Title = "Title",
                ChecklistId = 54,
                UserId = 1
                };

            var jsonInput = JsonConvert.SerializeObject(input);
            var contentString = new StringContent(jsonInput, Encoding.UTF8, "application/json");

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.PostAsync(request.Url, contentString);
            var result = await ContentHelper<RatingViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.IsType<RatingViewModel>(result.Data);
            Assert.NotNull(result.Data.Id);
            Assert.False(result.Error);
            Assert.Equal("Comment", result.Data.Comment);
            }

        [Fact(DisplayName = "Should return failed when post rating without checklist and category")]
        [Trait("[IntegrationTest]-RatingController", "RatingController")]
        public async Task ShouldReturnSuccessWhenPostRatingWithoutChecklistAndCategory()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/rating"
                };

            var input = new RatingInput()
                {
                _Rating = 5,
                Comment = "Comment",
                Title = "Title",
                UserId = 1
                };

            var jsonInput = JsonConvert.SerializeObject(input);
            var contentString = new StringContent(jsonInput, Encoding.UTF8, "application/json");

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.PostAsync(request.Url, contentString);
            var result = await ContentHelper<RatingViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result.Error);
            Assert.NotEmpty(result.Messages);
            }
        }
}
