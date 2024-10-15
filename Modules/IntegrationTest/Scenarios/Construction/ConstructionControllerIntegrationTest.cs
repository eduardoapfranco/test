using Application.AppServices.ConstructionApplication.Input;
using Application.AppServices.ConstructionApplication.ViewModel;
using IntegrationTest.Config;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using System;
using IntegrationTest.Scenarios.Construction.Faker;
using System.Collections.Generic;
using Infra.CrossCutting.Controllers;
using Application.AppServices.ChecklistApplication.Input;
using Application.AppServices.ConstructionReportApplication.ViewModel;
using Newtonsoft.Json;

namespace IntegrationTest.Scenarios.Construction
    {
    public class ConstructionControllerIntegrationTest
        {
        private readonly TestContext _testContext;

        public ConstructionControllerIntegrationTest()
            {
            _testContext = new TestContext();
            }

        [Fact(DisplayName = "Should return not authorized when try to post construction without token")]
        [Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        public async Task ShouldReturnNotAuthorizedWhenTryToPostConstructionWithouToken()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/construction"
                };

            var input = ConstructionFaker.CreateInput();

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<ConstructionViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

        [Fact(DisplayName = "Should return success when post construction")]
        [Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        public async Task ShouldReturnSuccessWhenPostConstruction()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/construction"
                };

            var input = ConstructionFaker.CreateInput();

            var jsonInput = JsonConvert.SerializeObject(input);
            var contentString = new StringContent(jsonInput, Encoding.UTF8, "application/json");

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.PostAsync(request.Url, contentString);
            var result = await ContentHelper<ConstructionViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.IsType<ConstructionViewModel>(result.Data);
            Assert.NotNull(result.Data);
            Assert.False(result.Error);
            Assert.Equal(input.Nome, result.Data.Nome);

            //after
            ConstructionTestConfig.constructionId = result.Data.Id;
            ConstructionTestConfig.constructionAppId = result.Data.AppId;
            }

        [Fact(DisplayName = "Should return error when post construction with an existent id")]
        [Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        public async Task ShouldReturnErrorWhenPostConstructionWithAnExistentId()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/construction"
                };

            var input = ConstructionFaker.CreateInput();

            var jsonInput = JsonConvert.SerializeObject(input);
            var contentString = new StringContent(jsonInput, Encoding.UTF8, "application/json");

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);


            input.Id = await ConstructionTestConfig.GetConstructionId(_testContext);
            input.AppId = await ConstructionTestConfig.GetConstructionAppId(_testContext);
            jsonInput = JsonConvert.SerializeObject(input);
            contentString = new StringContent(jsonInput, Encoding.UTF8, "application/json");

            // act
            var response = await _testContext.Client.PostAsync(request.Url, contentString);
            var result = await ContentHelper<ConstructionViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Null(result.Data);
            Assert.True(result.Error);
            Assert.Equal("Falha ao criar a construção pois o id informado já consta na base", result.Messages[0]);
            }

        [Fact(DisplayName = "Should return success when put construction")]
        [Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        public async Task ShouldReturnSuccessWhenPutConstruction()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/construction"
                };

            var input = ConstructionFaker.CreateInput();

            var jsonInput = JsonConvert.SerializeObject(input);
            var contentString = new StringContent(jsonInput, Encoding.UTF8, "application/json");

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            await ConstructionTestConfig.GetConstructionId(_testContext);

            // act
            input.Id = await ConstructionTestConfig.GetConstructionId(_testContext);
            input.AppId = await ConstructionTestConfig.GetConstructionAppId(_testContext);
            input.Nome = "Obra Após o PUT";

            jsonInput = JsonConvert.SerializeObject(input);
            contentString = new StringContent(jsonInput, Encoding.UTF8, "application/json");

            var response = await _testContext.Client.PutAsync(request.Url, contentString);
            var result = await ContentHelper<ConstructionViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.IsType<ConstructionViewModel>(result.Data);
            Assert.NotEqual(0, result.Data.Id);
            Assert.False(result.Error);
            Assert.Equal(input.Nome, result.Data.Nome);
            }

        [Fact(DisplayName = "Should return 404 when put construction with unexistent id")]
        [Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        public async Task ShouldReturn404WhenPutConstructionWithUnexistentId()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/construction"
                };

            var input = ConstructionFaker.CreateInput();

            var jsonInput = JsonConvert.SerializeObject(input);
            var contentString = new StringContent(jsonInput, Encoding.UTF8, "application/json");

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.PutAsync(request.Url, contentString);
            var result = await ContentHelper<ConstructionViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

        [Fact(DisplayName = "Should return success when get construction list")]
        [Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        public async Task ShouldReturnSuccessWhenGetConstructionList()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/construction"
                };

            var input = ConstructionFaker.CreateInput();

            var jsonInput = JsonConvert.SerializeObject(input);
            var contentString = new StringContent(jsonInput, Encoding.UTF8, "application/json");

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            await ConstructionTestConfig.GetConstructionId(_testContext);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<ConstructionViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<List<ConstructionViewModel>>>(result);
            }

        [Fact(DisplayName = "Should return success when sync constructions")]
        [Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        public async Task ShouldReturnSuccessWhenSyncConstruction()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/construction/sync"
                };

            //var inputToUpdate = ConstructionFaker.CreateInput();
            //var inputToDelete = ConstructionFaker.CreateInput();

            //var jsonInputToUpdate = JsonConvert.SerializeObject<ConstructionInput>(inputToUpdate);
            //var contentStringToUpdate = new StringContent(jsonInputToUpdate, Encoding.UTF8, "application/json");


            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            //var responseToUpdate = await _testContext.Client.PostAsync(request.Url, contentStringToUpdate);
            //var resultPostToUpdate = await ContentHelper<ConstructionViewModel>.GetResponse(responseToUpdate);

            //var requestGetList = new
            //    {
            //    Url = "/api/v1/construction"
            //    };
            //var responseGetList = await _testContext.Client.GetAsync(requestGetList.Url);
            //var resultGetList = await ContentHelper<List<ConstructionViewModel>>.GetResponse(responseGetList);

            var inputToSync = new List<ConstructionViewModel>() { ConstructionFaker.CreateViewModel() };
            //foreach (ConstructionViewModel constructionItem in resultGetList.Data)
            //    {
            //    constructionItem.Deleted = true;
            //    inputToSync.Add(constructionItem);
            //    }

            var jsonInputToSync = JsonConvert.SerializeObject(inputToSync);
            var contentStringToSync = new StringContent(jsonInputToSync, Encoding.UTF8, "application/json");

            // act
            var response = await _testContext.Client.PostAsync(request.Url, contentStringToSync);
            var result = await ContentHelper<ConstructionSyncResponse>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<ConstructionSyncResponse>>(result);

            Assert.IsType<List<ConstructionViewModel>>(result.Data.toInsert);
            var toInsert = (List<ConstructionViewModel>)result.Data.toInsert;
            Assert.NotEmpty(toInsert);

            Assert.IsType<List<ConstructionViewModel>>(result.Data.toUpdate);
            var toUpdate = (List<ConstructionViewModel>)result.Data.toUpdate;
            Assert.Empty(toUpdate);

            Assert.IsType<List<string>>(result.Data.toDelete);
            var toDelete = (List<string>)result.Data.toDelete;
            Assert.Empty(toDelete);
            //Assert.Equal(resultGetList.Data.Count, toDelete.Count);
            }

        //ConstructionReportsTests

        [Fact(DisplayName = "Should return not authorized when try to post construction report without token")]
        [Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        public async Task ShouldReturnNotAuthorizedWhenTryToPostConstructionReportWithoutToken()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/construction/report/pdf"
                };

            var input = ConstructionFaker.CreateInput();

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

        [Fact(DisplayName = "Should return not authorized when try to get construction report without token")]
        [Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        public async Task ShouldReturnNotAuthorizedWhenTryToGetConstructionReportWithoutToken()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/construction/report/1/pdf"
                };

            // act
            var response = await _testContext.Client.GetAsync(request.Url);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

        [Fact(DisplayName = "Should return not authorized when try to get list of construction report without token")]
        [Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        public async Task ShouldReturnNotAuthorizedWhenTryToGetListOfConstructionReportWithoutToken()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/construction/1/report"
                };

            // act
            var response = await _testContext.Client.GetAsync(request.Url);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

        [Fact(DisplayName = "Should return construction report when get report")]
        [Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        public async Task ShouldReturnConstructionReportWhenGetReport()
            {
            // arrange
            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            await ConstructionTestConfig.GetConstructionId(_testContext);
            await ConstructionTestConfig.GetConstructionReportId(_testContext);
            // act

            var request = new
                {
                Url = "/api/v1/construction/report/" + ConstructionTestConfig.constructionReportId + "/pdf"
                };


            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<ConstructionReportViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<ConstructionReportViewModel>>(result);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Url);
            }

        [Fact(DisplayName = "Should return list of construction report")]
        [Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        public async Task ShouldReturnListOfConstructionReport()
            {
            // arrange
            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            await ConstructionTestConfig.GetConstructionId(_testContext);
            await ConstructionTestConfig.GetConstructionReportId(_testContext);

            var request = new
                {
                Url = "/api/v1/construction/" + ConstructionTestConfig.constructionAppId + "/report"
                };


            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<ConstructionReportViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<List<ConstructionReportViewModel>>>(result);
            }

        [Fact(DisplayName = "Should return construction not found when post construction report with unexistend construction")]
        [Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        public async Task ShouldReturnConstructionNotFoundWhenPostConstructionReportWithUnexistentConstruction()
            {
            // arrange
            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            await ConstructionTestConfig.GetConstructionId(_testContext);

            var input = new ExportChecklistInput()
                {
                CategoryId = 47
                };
            input.Dados = new ExportChecklistDadosInput();
            input.ConstructionAppId = "XYZ";
            input.Dados.Obra = new ExportChecklistDadosObraInput()
                {
                Nome = "Obra Teste",
                Responsavel = "Responsável",
                Contratante = "Contratante",
                Inicio = new DateTime(),
                Termino = new DateTime()
                };

            var request = new
                {
                Url = "/api/v1/construction/report/pdf"
                };


            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<ConstructionReportViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result.Error);
            Assert.IsType<Result<ConstructionReportViewModel>>(result);
            Assert.Null(result.Data);
            Assert.Single(result.Messages);
            }

        //[Fact(DisplayName = "Should return success when post construction report")]
        //[Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        //public async Task ShouldReturnSuccessWhenPostConstructionReport()
        //    {
        //    // arrange
        //    var token = await AuthLogin.GetTokenUser(_testContext);
        //    _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        //    await ConstructionTestConfig.GetConstructionId(_testContext);

        //    var input = new ExportChecklistInput()
        //        {
        //        CategoryId = 47
        //        };
        //    input.Dados = new ExportChecklistDadosInput();
        //    input.ConstructionAppId = ConstructionTestConfig.constructionAppId;
        //    input.Dados.DataComprovante = DateTime.Now;
        //    input.Dados.Obra = new ExportChecklistDadosObraInput()
        //        {
        //        Nome = "Obra Teste",
        //        Responsavel = "Responsável",
        //        Contratante = "Contratante",
        //        Inicio = new DateTime(),
        //        Termino = new DateTime(),
        //        };

        //    var request = new
        //        {
        //        Url = "/api/v1/construction/report/pdf"
        //        };


        //    // act
        //    var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
        //    var result = await ContentHelper<ConstructionReportViewModel>.GetResponse(response);

        //    // assert
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    Assert.False(result.Error);
        //    Assert.IsType<Result<ConstructionReportViewModel>>(result);
        //    Assert.NotNull(result.Data.Url);

            //pos
            //ConstructionTestConfig.constructionReportId = result.Data.Id;
            //}

        //[Fact(DisplayName = "Should return success when try to delete construction report")]
        //[Trait("[IntegrationTest]-ConstructionController", "ConstructionController")]
        //public async Task ShouldReturnSuccessWhenTryToDeleteConstructionReport()
        //    {
        //    // arrange
        //    var token = await AuthLogin.GetTokenUser(_testContext);
        //    _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        //    await ConstructionTestConfig.GetConstructionId(_testContext);
        //    await ConstructionTestConfig.GetConstructionReportId(_testContext);

        //    var request = new
        //        {
        //        Url = "/api/v1/construction/report/" + ConstructionTestConfig.constructionReportId + "/pdf"
        //        };


        //    // act
        //    var response = await _testContext.Client.DeleteAsync(request.Url);
        //    var result = await ContentHelper<bool>.GetResponse(response);

        //    // assert
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    Assert.False(result.Error);
        //    Assert.True(result.Data);

        //    //pos
        //    ConstructionTestConfig.constructionReportId = 0;
        //    }
        }
    }
