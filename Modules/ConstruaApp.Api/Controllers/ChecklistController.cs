using Application.AppServices.ChecklistApplication.Input;
using Application.AppServices.ChecklistApplication.ViewModel;
using Application.Interfaces;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ConstruaApp.Api.Controllers
{
    [Route("api/v1")]
    [Authorize]
    [ApiController]
    public class ChecklistController : BaseController
    {

        private readonly IChecklistApplication _checklistApplication;
        private readonly ILogger<ChecklistController> _logger;

        public ChecklistController(INotificationHandler<DomainNotification> notification, IChecklistApplication checklistApplication, ILogger<ChecklistController> logger) : base(notification)
        {
            _checklistApplication = checklistApplication;
            _logger = logger;
        }


        [HttpGet]
        [Route("checklists-sync")]
        [ProducesResponseType(typeof(Result<IEnumerable<ChecklistViewModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetChecklistsLastDateUpdatedAsync([FromQuery] DateTime? lastDateSync)
        {
            return OkOrDefault(await _checklistApplication.GetChecklistsLastDateUpdatedAsync(lastDateSync));
        }

        [HttpGet]
        [Route("checklists/category/{id}")]
        [ProducesResponseType(typeof(Result<IEnumerable<ChecklistViewModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetChecklistsLastDateUpdatedAsync([FromRoute] int id)
        {
            return OkOrDefault(await _checklistApplication.SelectByCategoryIdAsync(id));
        }

        [HttpPost]
        [Route("checklists/pdf-by-category")]
        [ProducesResponseType(typeof(ExportChecklistPDFViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ExportChecklistsToPDF([FromBody] ExportChecklistInput input)
        {
            try
            {
                _logger.LogInformation($"Request for {nameof(ExportChecklistsToPDF)} with param: { JsonConvert.SerializeObject(input)}");
                var result = await _checklistApplication.ExportChecklistsToPDF(input);
                return OkOrDefault(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"InternalServerError for {nameof(ExportChecklistsToPDF)} with exception: { JsonConvert.SerializeObject(ex)}");
                return InternalServerError(new Exception("Internal server error!"));
            }
        }

        [HttpPost]
        [Route("checklists/pdf-by-category/base-64")]
        [ProducesResponseType(typeof(ExportChecklistPDFViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ExportChecklistsToPDFBase64([FromBody] ExportChecklistInput input)
        {
            try
            {
                _logger.LogInformation($"Request for {nameof(ExportChecklistsToPDF)} with param: { JsonConvert.SerializeObject(input)}");
                var result = await _checklistApplication.ExportChecklistsToPDF(input);
                return Ok(result.Pdf);
            }
            catch (Exception ex)
            {
                _logger.LogError($"InternalServerError for {nameof(ExportChecklistsToPDF)} with exception: { JsonConvert.SerializeObject(ex)}");
                return InternalServerError(new Exception("Internal server error!"));
            }
        }

        [HttpPost]
        [Route("checklists/pdf-by-category/json")]
        [ProducesResponseType(typeof(ExportChecklistViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ExportChecklists([FromBody] ExportChecklistInput input)
        {
            try
            {
                _logger.LogInformation($"Request for {nameof(ExportChecklists)} with param: { JsonConvert.SerializeObject(input)}");
                var result = await _checklistApplication.ExportChecklists(input);
                return OkOrDefault(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"InternalServerError for {nameof(ExportChecklistsToPDF)} with exception: { JsonConvert.SerializeObject(ex)}");
                return InternalServerError(new Exception("Internal server error!"));
            }
        }
    }
}