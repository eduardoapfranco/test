using Application.Interfaces;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Application.AppServices.OrderApplication.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Context;
using Microsoft.AspNetCore.Authorization;
using Application.AppServices.OrderApplication.ViewModels;

namespace ConstruaApp.Api.Controllers
    {
    [Route("api/v1/order")]
    [ApiController]
    [Authorize]
    public class OrderController : BaseController
        {

        private readonly IOrderApplication _orderApplication;
        private readonly IConfiguration _configuration;
        ILogger<OrderController> _logger;

        public OrderController(INotificationHandler<DomainNotification> notification, IOrderApplication orderApplication, IConfiguration configuration, ILogger<OrderController> logger) : base(notification)
            {
            _orderApplication = orderApplication;
            _configuration = configuration;
            _logger = logger;
            }

        [HttpPost]
        [Route("pagseguro/notifications")]
        [AllowAnonymous]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(typeof(Result<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ProcessPagseguroNotificationAsync([FromForm] WebhookPagSeguroNotificationInput input)
            {
            ILogEventEnricher[] enrichers =
               {
                new PropertyEnricher("NotificationCode", input.NotificationCode),
                new PropertyEnricher("NotificationType", input.NotificationType)
                };

            using (LogContext.Push(enrichers))
                {
                _logger.LogInformation("ProcessPagseguroNotificationAsync initialized at {date} with parameter {@param}", DateTime.UtcNow, input);
                input.UrlBase = _configuration.GetSection("Pagseguro:UrlBase").Value;
                input.Token = _configuration.GetSection("Pagseguro:Token").Value;
                input.Email = _configuration.GetSection("Pagseguro:Email").Value;
                return OkOrDefault(await _orderApplication.ProcessPagseguroNotificationAsync(input));
                }
            }

        [HttpGet]
        [Route("form")]
        [ProducesResponseType(typeof(Result<PagSeguroFormResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetPagseguroOrderFormAsync([FromQuery] int planId)
            {
            int userId = (int)(int)GetUserLogged().Id;
            ILogEventEnricher[] enrichers =
                {
                new PropertyEnricher("userId", userId),
                new PropertyEnricher("planId", planId)
                };
            using (LogContext.Push(enrichers))
                {
                _logger.LogInformation("GetPagseguroOrderFormAsync initialized at {date} with UserId ", DateTime.UtcNow);
                WebhookPagSeguroNotificationInput input = new WebhookPagSeguroNotificationInput()
                    {
                    UrlBase = _configuration.GetSection("Pagseguro:UrlBase").Value,
                    UrlBaseOrderForm = _configuration.GetSection("Pagseguro:UrlBaseOrderForm").Value,
                    UrlRedirectOrderForm = _configuration.GetSection("Pagseguro:UrlRedirectOrderForm").Value,
                    URLNotifications = _configuration.GetSection("Pagseguro:URLNotifications").Value,
                    Token = _configuration.GetSection("Pagseguro:Token").Value,
                    Email = _configuration.GetSection("Pagseguro:Email").Value
                    };

                PagSeguroFormResponse response = new PagSeguroFormResponse()
                    {
                    UrlPagSeguro = await _orderApplication.GetPagseguroOrderFormAsync(userId, planId, input),
                    UrlRedirect = _configuration.GetSection("Pagseguro:UrlRedirectOrderForm").Value,
                    };

                return OkOrDefault(response);
                }
            }
        }
    }
