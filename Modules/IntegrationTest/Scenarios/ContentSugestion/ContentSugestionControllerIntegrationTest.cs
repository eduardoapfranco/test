using Application.AppServices.ContentSugestionApplication.Input;
using Application.AppServices.ContentSugestionApplication.ViewModel;
using IntegrationTest.Config;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest.Scenarios.ContentSugestion
    {
    public class ContentSugestionControllerIntegrationTest
    {
        private readonly TestContext _testContext;


        public ContentSugestionControllerIntegrationTest()
        {
            _testContext = new TestContext();
        }

        [Fact(DisplayName = "Should return not authorized when try to post contentSugestion without token")]
        [Trait("[IntegrationTest]-ContentSugestionController", "ContentSugestionController")]
        public async Task ShouldReturnNotAuthorizedWhenTryToPostContentSugestionWithouToken()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/content-sugestion"
                };

            var input = new ContentSugestionInput()
                {
                Content = "Comment",
                Type = 1,
                CategoryId = 191,
                UserId = 1,
                Title = "Titulo"
                };

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<ContentSugestionViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

        [Fact(DisplayName = "Should return success when post contentSugestion with category")]
        [Trait("[IntegrationTest]-ContentSugestionController", "ContentSugestionController")]
        public async Task ShouldReturnSuccessWhenPostContentSugestionWithCategory()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/content-sugestion"
                };

            var input = new ContentSugestionInput()
                {
                Content = "Comment",
                Type = 1,
                CategoryId = 191,
                UserId = 1,
                Title = "Titulo"
                };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<ContentSugestionViewModel>.GetResponse(response);          

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.IsType<ContentSugestionViewModel>(result.Data);
            Assert.NotNull(result.Data.Id);
            Assert.False(result.Error);
            Assert.Equal("Comment", result.Data.Content);
        }

        [Fact(DisplayName = "Should return success when post contentSugestion with checklist")]
        [Trait("[IntegrationTest]-ContentSugestionController", "ContentSugestionController")]
        public async Task ShouldReturnSuccessWhenPostContentSugestionWithChecklist()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/content-sugestion"
                };

            var input = new ContentSugestionInput()
                {
                Content = "Comment",
                Type = 1,
                ChecklistId = 54,
                UserId = 1,
                Title = "Titulo"
                };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<ContentSugestionViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.IsType<ContentSugestionViewModel>(result.Data);
            Assert.False(result.Error);
            Assert.Equal("Comment", result.Data.Content);
            }

        [Fact(DisplayName = "Should return success when post contentSugestion without checklist and category")]
        [Trait("[IntegrationTest]-ContentSugestionController", "ContentSugestionController")]
        public async Task ShouldReturnSuccessWhenPostContentSugestionWithoutChecklistAndCategory()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/content-sugestion"
                };

            var input = new ContentSugestionInput()
                {
                Content = "Comment",
                Type = 1,
                UserId = 1,
                Title = "Titulo"
                };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<ContentSugestionViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.Null(result.Messages);
            }
        }
}
