using Application.AppServices.ChecklistApplication.Input;
using Application.AppServices.ConstructionApplication.Input;
using Application.AppServices.ConstructionApplication.ViewModel;
using Application.AppServices.ConstructionReportApplication.ViewModel;
using IntegrationTest.Scenarios.Construction.Faker;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntegrationTest.Config
    {
    public static class ConstructionTestConfig
        {
        public static int constructionId = 0;
        public static string constructionAppId = String.Empty;
        public static ConstructionViewModel construction;
        public static  int constructionReportId = 0;

        public static async Task<int> GetConstructionId(TestContext _testContext)
            {
            if (ConstructionTestConfig.constructionId > 0)
                {
                return ConstructionTestConfig.constructionId;
                }

            ConstructionTestConfig.construction = await GetConstruction(_testContext);
            return ConstructionTestConfig.construction.Id;
            }

        public static async Task<string> GetConstructionAppId(TestContext _testContext)
            {
            if (!String.IsNullOrEmpty(ConstructionTestConfig.constructionAppId))
                {
                return ConstructionTestConfig.constructionAppId;
                }

            ConstructionTestConfig.construction = await GetConstruction(_testContext);
            return ConstructionTestConfig.construction.AppId;
            }

        public static async Task<ConstructionViewModel> GetConstruction(TestContext _testContext)
            {

            var request = new
                {
                Url = "/api/v1/construction"
                };

            var inputSetup = ConstructionFaker.CreateInput();

            var jsonInputSetup = JsonConvert.SerializeObject(inputSetup);
            var contentStringSetup = new StringContent(jsonInputSetup, Encoding.UTF8, "application/json");

            if (null == _testContext.Client.DefaultRequestHeaders.Authorization)
                {
                var tokenSetup = await AuthLogin.GetTokenUser(_testContext);
                _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenSetup);
                }
            // act
            var responseSetup = await _testContext.Client.PostAsync(request.Url, contentStringSetup);
            var resultSetup = await ContentHelper<ConstructionViewModel>.GetResponse(responseSetup);
            ConstructionTestConfig.constructionId = resultSetup.Data.Id;
            ConstructionTestConfig.constructionAppId = resultSetup.Data.AppId;
            ConstructionTestConfig.construction = resultSetup.Data;
            return resultSetup.Data;
            }

        public static async Task<int> GetConstructionReportId(TestContext _testContext)
            {
            if (ConstructionTestConfig.constructionReportId > 0)
                {
                return ConstructionTestConfig.constructionReportId;
                }

            var input = new ExportChecklistInput()
                {
                CategoryId = 47
                };
            input.Dados = new ExportChecklistDadosInput();
            input.Dados.DataComprovante = DateTime.Now;
            input.ConstructionAppId = ConstructionTestConfig.constructionAppId;
            input.Dados.Obra = new ExportChecklistDadosObraInput()
                {
                Nome = "Obra Teste",
                Responsavel = "Responsável",
                Contratante = "Contratante",
                Inicio = new DateTime(),
                Termino = new DateTime()
                };

            var requestPost = new
                {
                Url = "/api/v1/construction/report/pdf"
                };

            var responsePost = await _testContext.Client.PostAsync(requestPost.Url, ContentHelper<object>.FormatStringContent(input));
            var resultPost = await ContentHelper<ConstructionReportViewModel>.GetResponse(responsePost);
            ConstructionTestConfig.constructionReportId = resultPost.Data.Id;
            return ConstructionTestConfig.constructionReportId;
            }
        }
}
