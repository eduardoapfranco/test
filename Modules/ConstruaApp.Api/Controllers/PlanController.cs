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
using Domain.Interfaces.Services;
using Domain.Entities;
using System.Collections.Generic;
using Domain.Enum;
using Newtonsoft.Json;

namespace ConstruaApp.Api.Controllers
{
    [Route("api/v1/plan")]
    [Authorize]
    [ApiController]
    public class PlanController :  BaseController
    {

        private readonly IPlanDomainService _planDomainService;
        ILogger<PlanController> _logger;

        public PlanController(INotificationHandler<DomainNotification> notification, IPlanDomainService planDomainService, ILogger<PlanController> logger) : base(notification)
        {
            _planDomainService = planDomainService;
            _logger = logger;
        }
        

        [HttpGet]
        [Route("premium")]
        [ProducesResponseType(typeof(Result<IEnumerable<Plan>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAsync()
        {            
            try
            {
                var result = await _planDomainService.GetPlansPremiumWithType();
                return OkOrDefault(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"InternalServerError for {nameof(GetAsync)} with exception: { JsonConvert.SerializeObject(ex)}");
                return InternalServerError(new Exception("Internal server error!"));
            }
        }


        [HttpGet]
        [Route("premium/without-auth")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<IEnumerable<Plan>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNoAuthAsync()
        {
            try
            {
                var result = await _planDomainService.GetPlansPremiumWithType();
                return OkOrDefault(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"InternalServerError for {nameof(GetAsync)} with exception: { JsonConvert.SerializeObject(ex)}");
                return InternalServerError(new Exception("Internal server error!"));
            }
        }
    }
}