using Application.Interfaces;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Context;
using Microsoft.AspNetCore.Authorization;
using Application.AppServices.AreaApplication.ViewModel;
using Domain.Interfaces.Services;
using AutoMapper;
using System;
using System.Collections.Generic;

namespace ConstruaApp.Api.Controllers
    {
    [Route("api/v1/area")]
    [ApiController]
    [Authorize]
    public class AreaController : BaseController
        {

        private readonly IAreaDomainService _areaDomainService;
        private readonly IMapper _mapper;
        ILogger<AreaController> _logger;

        public AreaController(INotificationHandler<DomainNotification> notification, IAreaDomainService areaDomainService
            ,IMapper mapper, ILogger<AreaController> logger) : base(notification)
            {
            _areaDomainService = areaDomainService;
            _logger = logger;
            _mapper = mapper;
            }

        [HttpGet]
        [ProducesResponseType(typeof(Result<AreaViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAreasAsync()
            {
            
            _logger.LogInformation("GetAreasAsync initialized at {@date} ", DateTime.UtcNow);
            var entityArea = await _areaDomainService.GetAreasAsync();
            return OkOrDefault(_mapper.Map<IEnumerable<AreaViewModel>>(entityArea));
            }
        }
    }