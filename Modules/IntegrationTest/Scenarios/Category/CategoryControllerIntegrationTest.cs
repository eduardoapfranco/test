using Application.AppServices.CategoryApplication.ViewModel;
using Infra.CrossCutting.Controllers;
using IntegrationTest.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest.Scenarios.Category

    {
    public class CategoryControllerIntegrationTest
    {
        private readonly TestContext _testContext;


        public CategoryControllerIntegrationTest()
        {
            _testContext = new TestContext();
        }

        [Fact(DisplayName = "Should return not authorized when try to get category list without token")]
        [Trait("[IntegrationTest]-CategoryController", "CategoryController")]
        public async Task ShouldReturnNotAuthorizedWhenTryToGetCategoryListWithoutToken()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/categories-sync"
                };

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<CategoryViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

        [Fact(DisplayName = "Should return fail when parameter LastDateSync isn't informed")]
        [Trait("[IntegrationTest]-CategoryController", "CategoryController")]
        public async Task ShouldReturnFailWhenParameterLastDateSyncIsntInformed()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/categories-sync"
            };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<CategoryViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result.Error);
            Assert.Single(result.Messages);
            Assert.Contains("O parâmetro lastDateSync é obrigatório", result.Messages[0]);
        }

        [Fact(DisplayName = "Should return empty categories when does'nt exists categories to sync")]
        [Trait("[IntegrationTest]-CategoryController", "CategoryController")]
        public async Task ShouldReturnEmptyCategoriesWhenDoesntExistCategoriesToSync()
        {
            // arrange
            DateTime lastDateSync = DateTime.Today.AddDays(+1);
            var request = new
            {
                Url = "/api/v1/categories-sync" + "?lastDateSync=" + lastDateSync.ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo)
            };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<CategoryViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
        }

        [Fact(DisplayName = "Should return list of categories to sync")]
        [Trait("[IntegrationTest]-CategoryController", "CategoryController")]
        public async Task ShouldReturnListOfCategoriesToSync()
        {
            // arrange
            DateTime lastDateSync = DateTime.Today.AddDays(-120);
            var request = new
            {
                Url = "/api/v1/categories-sync" + "?lastDateSync=" + lastDateSync.ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo)
            };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<CategoryViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<List<CategoryViewModel>>>(result);
        }

        [Fact(DisplayName = "Should return list of root categories based on profile")]
        [Trait("[IntegrationTest]-CategoryController", "CategoryController")]
        public async Task ShouldReturnListOfRootCategoriesBasedOnProfile()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/categories"
                };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<CategoryViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<List<CategoryViewModel>>>(result);
            }

        [Fact(DisplayName = "Should return list of categories by parent category")]
        [Trait("[IntegrationTest]-CategoryController", "CategoryController")]
        public async Task ShouldReturnListOfCategoriesByParentCategory()
            {
            // arrange
            int categoryId = 1;
            var request = new
                {
                Url = "/api/v1/categories/parent/" + categoryId
                };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<CategoryViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<List<CategoryViewModel>>>(result);
            }

        [Fact(DisplayName = "Should return list with all categories")]
        [Trait("[IntegrationTest]-CategoryController", "CategoryController")]
        public async Task ShouldReturnListWithAllCategories()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/categories/all"
                };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<CategoryViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<List<CategoryViewModel>>>(result);
            }

        [Fact(DisplayName = "Should return category by id")]
        [Trait("[IntegrationTest]-CategoryController", "CategoryController")]
        public async Task ShouldReturnCategoryById()
            {
            // arrange
            int categoryId = 1;
            var request = new
                {
                Url = "/api/v1/categories/" + categoryId
                };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<CategoryViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<List<CategoryViewModel>>>(result);
            }
        }
}
