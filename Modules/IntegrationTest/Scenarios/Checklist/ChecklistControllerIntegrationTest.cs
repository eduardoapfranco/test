using Application.AppServices.ChecklistApplication.Input;
using Application.AppServices.ChecklistApplication.ViewModel;
using Bogus;
using Domain.Enum;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Controllers;
using Infra.Data.Context;
using Infra.Data.Repository;
using IntegrationTest.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest.Scenarios.Checklist

{
    public class ChecklistControllerIntegrationTest
    {
        private readonly TestContext _testContext;


        public ChecklistControllerIntegrationTest()
        {
            _testContext = new TestContext();
        }

        [Fact(DisplayName = "Should return not authorized when try to get checklist list without token")]
        [Trait("[IntegrationTest]-ChecklistController", "ChecklistController")]
        public async Task ShouldReturnNotAuthorizedWhenTryToGetChecklistListWithouToken()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/checklists-sync"
                };

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<ChecklistViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

        [Fact(DisplayName = "Should return fail when parameter LastDateSync isn't informed")]
        [Trait("[IntegrationTest]-ChecklistController", "ChecklistController")]
        public async Task ShouldReturnFailWhenParameterLastDateSyncIsntInformed()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/checklists-sync"
            };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<ChecklistViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Single(result.Messages);
            Assert.Contains("O parâmetro lastDateSync é obrigatório", result.Messages[0]);
        }

        [Fact(DisplayName = "Should return empty checklists when does'nt exists checklists to sync")]
        [Trait("[IntegrationTest]-ChecklistController", "ChecklistController")]
        public async Task ShouldReturnEmptyChecklistsWhenDoesntExistCategoriesToSync()
        {
            // arrange
            DateTime lastDateSync = DateTime.Today.AddDays(+1);
            var request = new
            {
                Url = "/api/v1/checklists-sync" + "?lastDateSync=" + lastDateSync.ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo)
            };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<ChecklistViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
        }

        [Fact(DisplayName = "Should return list of checklists to sync")]
        [Trait("[IntegrationTest]-ChecklistController", "ChecklistController")]
        public async Task ShouldReturnListOfCategoriesToSync()
        {
            // arrange
            DateTime lastDateSync = DateTime.Today.AddDays(-120);
            var request = new
            {
                Url = "/api/v1/checklists-sync" + "?lastDateSync=" + lastDateSync.ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo)
            };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<ChecklistViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<List<ChecklistViewModel>>>(result);
        }

        //[Fact(DisplayName = "Should export pdf by category")]
        //[Trait("[IntegrationTest]-ChecklistController", "ChecklistController")]
        //public async Task ShouldExportPDFByCategory()
        //{
        //    // arrange
        //    var input = new ExportChecklistInput()
        //    {
        //        CategoryId = 47
        //    };
        //    input.Dados = new ExportChecklistDadosInput();
        //    input.Dados.Obra = new ExportChecklistDadosObraInput()
        //        {
        //        Nome = "Obra Teste",
        //        Responsavel = "Responsável",
        //        Contratante = "Contratante",
        //        Inicio = new DateTime(),
        //        Termino = new DateTime()
        //        };

        //    var request = new
        //    {
        //        Url = "/api/v1/checklists/pdf-by-category"
        //    };

        //    var token = await AuthLogin.GetTokenUser(_testContext);
        //    _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        //    // act
        //    var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
        //    var result = await ContentHelper<ExportChecklistPDFViewModel>.GetResponse(response);

        //    // assert
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    Assert.False(result.Error);
        //    Assert.IsType<Result<ExportChecklistPDFViewModel>>(result);
        //    Assert.NotNull(result.Data.Pdf);
        //}

        [Fact(DisplayName = "Should return list of checklists by category to sync")]
        [Trait("[IntegrationTest]-ChecklistController", "ChecklistController")]
        public async Task ShouldReturnListOfChecklistsByCategories()
            {
            // arrange
            int categoryId = 1;
            var request = new
                {
                Url = "/api/v1/checklists/category/" + categoryId
                };

            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // act
            var response = await _testContext.Client.GetAsync(request.Url);
            var result = await ContentHelper<List<ChecklistViewModel>>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<List<ChecklistViewModel>>>(result);
            }
        }
}
