using Application.AppServices.RatingApplication.Input;
using Application.AppServices.RatingApplication.ViewModel;
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
    [Route("api/v1/rating")]
    [Authorize]
    [ApiController]
    public class RatingController :  BaseController
    {

        private readonly IRatingApplication _ratingApplication;
        ILogger<RatingController> _logger;

        public RatingController(INotificationHandler<DomainNotification> notification, IRatingApplication ratingApplication, ILogger<RatingController> logger) : base(notification)
        {
            _ratingApplication = ratingApplication;
            _logger = logger;
        }
        

        [HttpPost]
        [ProducesResponseType(typeof(Result<RatingViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] RatingInput input)
        {
            _logger.LogInformation("RatingController PostAsync initialized at {date} with input {input}", DateTime.UtcNow, input);
            input.UserId = (int)GetUserLogged().Id;
            return OkOrDefault(await _ratingApplication.InsertAsync(input));
        }
    }
}