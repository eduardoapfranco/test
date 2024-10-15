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
using Application.AppServices.VersionApplication.ViewModel;
using Domain.Interfaces.Services;
using AutoMapper;
using System;

namespace ConstruaApp.Api.Controllers
    {
    [Route("api/v1/version")]
    [ApiController]
    [Authorize]
    public class VersionController : BaseController
        {

        private readonly IVersionDomainService _versionDomainService;
        private readonly IMapper _mapper;
        ILogger<VersionController> _logger;

        public VersionController(INotificationHandler<DomainNotification> notification, IVersionDomainService versionDomainService
            ,IMapper mapper, ILogger<VersionController> logger) : base(notification)
            {
            _versionDomainService = versionDomainService;
            _logger = logger;
            _mapper = mapper;
            }

        [HttpGet]
        [Route("{platform}/{version=latest}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<VersionViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetVersionAsync([FromRoute] string platform, string version)
            {
            ILogEventEnricher[] enrichers =
            {
                new PropertyEnricher("platform", platform),
                new PropertyEnricher("version", version)
                };
            using (LogContext.Push(enrichers))
                {
                _logger.LogInformation("GetVersionAsync initialized at {@date} with parameters {@platform} and {@version} ", DateTime.UtcNow, platform, version);
                var entityVersion = await _versionDomainService.GetVersionAsync(platform, version);
                return OkOrDefault(_mapper.Map<VersionViewModel>(entityVersion));

                }
            }
        }
    }