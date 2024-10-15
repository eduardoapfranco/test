using Application.Interfaces;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Application.AppServices.SignatureApplication.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Context;
using Microsoft.AspNetCore.Authorization;
using Application.AppServices.SignatureApplication.ViewModels;
using Microsoft.AspNetCore.Hosting;

namespace ConstruaApp.Api.Controllers
{
    [Route("api/v1/signature")]
    [ApiController]
    [Authorize]
    public class SignatureController : BaseController
    {

        private readonly ISignatureApplication _signatureApplication;
        private readonly IConfiguration _configuration;
        ILogger<SignatureController> _logger;
        private IWebHostEnvironment _hostingEnvironment;

        public SignatureController(INotificationHandler<DomainNotification> notification, ISignatureApplication signatureApplication, IConfiguration configuration
            , ILogger<SignatureController> logger, IWebHostEnvironment environment) : base(notification)
        {
            _signatureApplication = signatureApplication;
            _configuration = configuration;
            _logger = logger;
            _hostingEnvironment = environment;
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Result<SignatureViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostSignatureAsync([FromBody] SignatureInput input)
        {
            int userId = (int)(int)GetUserLogged().Id;
            input.UserId = userId;
            input.UrlBase = _configuration.GetSection("Iugu:UrlBase").Value;
            input.Token = _configuration.GetSection("Iugu:Token").Value;

            ILogEventEnricher[] enrichers =
                {
                new PropertyEnricher("userId", userId),
                new PropertyEnricher("SignatureInput", input)
                };
            using (LogContext.Push(enrichers))
            {
                _logger.LogInformation("PostSignatureAsync initialized at {date} with UserId ", DateTime.UtcNow);
            };

            SignatureViewModel response = await _signatureApplication.PostSignatureAsync(input);

            return OkOrDefault(response);
        }

        [HttpPost]
        [Route("change-payment-method")]
        [ProducesResponseType(typeof(Result<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ChangePaymentMethodAsync([FromBody] SignatureInput input)
        {
            int userId = (int)(int)GetUserLogged().Id;
            input.UserId = userId;
            input.UrlBase = _configuration.GetSection("Iugu:UrlBase").Value;
            input.Token = _configuration.GetSection("Iugu:Token").Value;

            ILogEventEnricher[] enrichers =
            {
                new PropertyEnricher("userId", userId),
                new PropertyEnricher("SignatureInput", input)
            };
            using (LogContext.Push(enrichers))
            {
                _logger.LogInformation("ChangePaymentMethodAsync initialized at {date} with UserId ", DateTime.UtcNow);
            };

            var response = await _signatureApplication.ChangePaymentMethodAsync(input);

            return OkOrDefault(response);
        }

        [HttpPost]
        [Route("iugu/webhook")]
        [AllowAnonymous]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(typeof(Result<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ProcessIuguWebhookAsync([FromForm] IuguWebhookInput input)
        {
            ILogEventEnricher[] enrichers =
               {
                new PropertyEnricher("Event", input.Event),
                new PropertyEnricher("Data", input.Data)
                };

            using (LogContext.Push(enrichers))
            {
                _logger.LogInformation("ProcessIuguWebhookAsync initialized at {date} with parameter {@param}", DateTime.UtcNow, input);
                var userPlans = await _signatureApplication.ProcessIuguWebhookAsync(input);
                var retorno = (null != userPlans && !String.IsNullOrEmpty(userPlans.IuguSignatureId));
                return OkOrDefault(retorno);
            }
        }

        [HttpPost]
        [Route("suspend")]
        [ProducesResponseType(typeof(Result<SignatureViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostSuspendSignatureAsync()
        {
            int userId = (int)(int)GetUserLogged().Id;

            SignatureInput input = new SignatureInput()
            {
                UserId = userId,
                UrlBase = _configuration.GetSection("Iugu:UrlBase").Value,
                Token = _configuration.GetSection("Iugu:Token").Value
            };

            ILogEventEnricher[] enrichers =
                {
                new PropertyEnricher("userId", userId),
                new PropertyEnricher("SignatureInput", input)
                };
            using (LogContext.Push(enrichers))
            {
                _logger.LogInformation("PostInactivateSignatureAsync initialized at {date} with UserId {userId}", DateTime.UtcNow, userId);
            };

            SignatureViewModel response = await _signatureApplication.PostChangeStatusSignatureAsync(input, "suspend");

            return OkOrDefault(response);
        }

        [HttpPost]
        [Route("activate")]
        [ProducesResponseType(typeof(Result<SignatureViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostActivateSignatureAsync()
        {
            int userId = (int)(int)GetUserLogged().Id;

            SignatureInput input = new SignatureInput()
            {
                UserId = userId,
                UrlBase = _configuration.GetSection("Iugu:UrlBase").Value,
                Token = _configuration.GetSection("Iugu:Token").Value
            };

            ILogEventEnricher[] enrichers =
                {
                new PropertyEnricher("userId", userId),
                new PropertyEnricher("SignatureInput", input)
                };
            using (LogContext.Push(enrichers))
            {
                _logger.LogInformation("PostInactivateSignatureAsync initialized at {date} with UserId {userId}", DateTime.UtcNow, userId);
            };

            SignatureViewModel response = await _signatureApplication.PostChangeStatusSignatureAsync(input, "activate");

            return OkOrDefault(response);
        }

        [HttpPost]
        [Route("fovea/webhook")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ProcessFoveaWebhookAsync([FromBody] FoveaWebhookInput input)
            {
            _logger.LogInformation("ProcessFoveaWebhookAsync initialized at {date}", DateTime.UtcNow);
            //System.Text.Json.JsonElement input = dynamicInput;
            //input = input.GetRawText();
            _logger.LogInformation("ProcessFoveaWebhookAsync initialized at {date} with parameter {@param}", DateTime.UtcNow, input);

            ILogEventEnricher[] enrichers =
               {
                new PropertyEnricher("FoveaWebhookInput", input),
                };

            using (LogContext.Push(enrichers))
                {
                _logger.LogInformation("ProcessFoveaWebhookAsync initialized at {date} with parameter {@param}", DateTime.UtcNow, input);
                if(input.Password != _configuration.GetSection("Fovea:WebhookPassword").Value)
                    {
                    return Error("Not Authorized");
                    }

                var userPlans = await _signatureApplication.ProcessFoveaWebhookAsync(input, _hostingEnvironment.EnvironmentName);
                var retorno = true;
                return OkOrDefault(retorno);
                }
            }

        [HttpPost]
        [Route("temp/{platform}")]
        [ProducesResponseType(typeof(Result<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateTempSignatureAsync([FromRoute] string platform)
            {
            int userId = (int)(int)GetUserLogged().Id;

            SignatureInput input = new SignatureInput()
            {
                UserId = userId,
                UrlBase = _configuration.GetSection("Fovea:UrlBase").Value,
                Token = _configuration.GetSection("Fovea:WebhookPassword").Value
            };

            ILogEventEnricher[] enrichers =
                {
                new PropertyEnricher("userId", userId),
                new PropertyEnricher("platform", platform)
                };
            using (LogContext.Push(enrichers))
                {
                _logger.LogInformation("CreateTempSignatureAsync initialized at {date} with UserId {userId} and Platform {platform}", DateTime.UtcNow, userId, platform);
                };

            var userPlans = await _signatureApplication.CreateTempSignatureAsync(input, platform);
            var retorno = (null != userPlans && !String.IsNullOrEmpty(userPlans.IuguSignatureId));
            return OkOrDefault(retorno);
            }
        }
}
