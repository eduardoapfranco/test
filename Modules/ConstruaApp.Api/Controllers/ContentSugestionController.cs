using Application.AppServices.ContentSugestionApplication.Input;
using Application.AppServices.ContentSugestionApplication.ViewModel;
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

namespace ConstruaApp.Api.Controllers
{
    [Route("api/v1/content-sugestion")]
    [Authorize]
    [ApiController]
    public class ContentSugestionController :  BaseController
    {

        private readonly IContentSugestionApplication _contentSugestionApplication;
        ILogger<ContentSugestionController> _logger;

        public ContentSugestionController(INotificationHandler<DomainNotification> notification, IContentSugestionApplication contentSugestionApplication, ILogger<ContentSugestionController> logger) : base(notification)
        {
            _contentSugestionApplication = contentSugestionApplication;
            _logger = logger;
        }
        

        [HttpPost]
        [ProducesResponseType(typeof(Result<ContentSugestionViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] ContentSugestionInput input)
        {
            _logger.LogInformation("ContentSugestionController PostAsync initialized at {date} with input {input}", DateTime.UtcNow, input);
            input.UserId = (int)GetUserLogged().Id;
            return OkOrDefault(await _contentSugestionApplication.InsertAsync(input));
        }
    }
}