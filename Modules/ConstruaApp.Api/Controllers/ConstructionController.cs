using Application.AppServices.ConstructionApplication.Input;
using Application.AppServices.ConstructionApplication.ViewModel;
using Application.Interfaces;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Application.AppServices.ConstructionReportApplication.ViewModel;
using Application.AppServices.ChecklistApplication.Input;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace ConstruaApp.Api.Controllers
{
    [Route("api/v1/construction")]
    [Authorize]
    [ApiController]
    public class ConstructionController :  BaseController
    {

        private readonly IConstructionApplication _constructionApplication;
        private readonly IConstructionReportApplication _constructionReportApplication;
        ILogger<ConstructionController> _logger;
        private IConfiguration _configuration;

        public ConstructionController(INotificationHandler<DomainNotification> notification, IConstructionApplication constructionApplication, 
            ILogger<ConstructionController> logger, IConstructionReportApplication constructionReportApplication,
            IConfiguration configuration) : base(notification)
        {
            _constructionApplication = constructionApplication;
            _constructionReportApplication = constructionReportApplication;
            _logger = logger;
            _configuration = configuration;
        }
        

        [HttpPost]
        [ProducesResponseType(typeof(Result<ConstructionViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] ConstructionInput input)
        {
            _logger.LogInformation("ConstructionController PostAsync initialized at {date} with input {input}", DateTime.UtcNow, input);
            input.UserId = (int)GetUserLogged().Id;
            return OkOrDefault(await _constructionApplication.InsertAsync(input));
        }

        [HttpPut]
        [ProducesResponseType(typeof(Result<ConstructionViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PutAsync([FromBody] ConstructionInput input)
            {
            _logger.LogInformation("ConstructionController PutAsync initialized at {date} with input {input}", DateTime.UtcNow, input);
            input.UserId = (int)GetUserLogged().Id;
            return OkOrNotFound(await _constructionApplication.UpdateAsync(input));
            }

        [HttpGet]
        [ProducesResponseType(typeof(Result<IEnumerable<ConstructionViewModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAsync()
            {
            _logger.LogInformation("ConstructionController ListAsync initialized at {date}", DateTime.UtcNow);
            return OkOrDefault(await _constructionApplication.ListAsync((int)GetUserLogged().Id));
            }

        [Route("sync"), HttpPost]
        [ProducesResponseType(typeof(Result<ConstructionSyncResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SyncConstructions([FromBody] List<ConstructionViewModel> appConstructions)
            {
            _logger.LogInformation("ConstructionController SyncAsync executed at {date}", DateTime.UtcNow);

            int userId = (int)GetUserLogged().Id;
            var response = await _constructionApplication.SyncAsync(userId, appConstructions);

            return OkOrDefault(response);
            }

        [HttpPost]
        [Route("report/pdf")]
        [ProducesResponseType(typeof(ConstructionReportViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostReportAsync([FromBody] ExportChecklistInput input)
            {
            try
                {
                int userId = (int)GetUserLogged().Id;
                _logger.LogInformation($"Request for {nameof(PostReportAsync)} with param: { JsonConvert.SerializeObject(input)}");
                var result = await _constructionReportApplication.InsertAsync(userId, input, _configuration.GetSection("ConnectionStrings:FolderDbMobile").Value);
                return OkOrDefault(result);
                }
            catch (Exception ex)
                {
                _logger.LogError($"InternalServerError for {nameof(PostReportAsync)} with exception: { JsonConvert.SerializeObject(ex)}");
                return InternalServerError(new Exception("Internal server error!"));
                }
            }
        [HttpGet]
        [Route("{constructionAppId}/report")]
        [ProducesResponseType(typeof(Result<IEnumerable<ConstructionReportViewModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ListConstructionReportsAsync([FromRoute] string constructionAppId)
            {
            _logger.LogInformation("ConstructionController ListConstructionReportsAsync initialized at {date}", DateTime.UtcNow);
            return OkOrDefault(await _constructionReportApplication.ListReportsAsync((int)GetUserLogged().Id, constructionAppId, _configuration.GetSection("ConnectionStrings:FolderDbMobile").Value));
            }

        [HttpGet]
        [Route("report/{id}/pdf")]
        [ProducesResponseType(typeof(Result<IEnumerable<ConstructionReportViewModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetConstructionReportsAsync([FromRoute] int id)
            {
            _logger.LogInformation("ConstructionController ListConstructionReportsAsync initialized at {date}", DateTime.UtcNow);
            return OkOrDefault(await _constructionReportApplication.GetReportAsync((int)GetUserLogged().Id, id, _configuration.GetSection("ConnectionStrings:FolderDbMobile").Value));
            }

        [Route("report/{id}/pdf"), HttpDelete]
        [ProducesResponseType(typeof(Result<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteConstructionReportsAsync([FromRoute] int id)
            {
            _logger.LogInformation("ConstructionController DeleteConstructionReportsAsync initialized at {date}", DateTime.UtcNow);
            return OkOrDefault(await _constructionReportApplication.DeleteReportAsync((int)GetUserLogged().Id, id, _configuration.GetSection("ConnectionStrings:FolderDbMobile").Value));
            }
        }
}