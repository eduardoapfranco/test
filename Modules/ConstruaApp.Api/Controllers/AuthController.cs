using Application.AppServices.UserApplication.Input;
using Application.AppServices.UserApplication.ViewModel;
using Application.Interfaces;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ConstruaApp.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class AuthController :  BaseController
    {

        private readonly IUserApplication _userApplication;

        private readonly ILogger<AuthController> _logger;

        public AuthController(INotificationHandler<DomainNotification> notification, IUserApplication userApplication, ILogger<AuthController> logger) : base(notification)
        {
            _userApplication = userApplication;
            _logger = logger;
        }        

        [HttpPost]
        [Route("/api/v1/register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<UserViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] UserInput input)
        {
            return OkOrDefault(await _userApplication.InsertMobileAsync(input));
        }

        [HttpPost]
        [Route("/api/v1/login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<UserViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginInput input)
        {
            return OkOrDefault(await _userApplication.LoginAsync(input));
        }

        [HttpPut]
        [Route("/api/v1/update")]
        [ProducesResponseType(typeof(Result<UserControlAccessVOViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateInput input)
        {
            var userId = (int)GetUserLogged().Id;
            return OkOrDefault(await _userApplication.UpdateAsync(userId, input));
        }

        [HttpPut]
        [Route("/api/v1/update/password")]
        [ProducesResponseType(typeof(Result<UserViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromBody] UserUpdatePasswordInput input)
        {
            var userId = (int)GetUserLogged().Id;
            return OkOrDefault(await _userApplication.UpdatePasswordAsync(userId, input));
        }

        [HttpPost]
        [Route("/api/v1/request-reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<UserRequestPasswordResetViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RequestPasswordResetAsync([FromBody] UserRequestPasswordResetInput input)
        {
            return OkOrDefault(await _userApplication.RequestPasswordResetAsync(input));
        }

        [HttpPost]
        [Route("/api/v1/reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<UserViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] UserResetPasswordInput input)
        {
            return OkOrDefault(await _userApplication.ResetPasswordAsync(input));
        }


        [HttpGet]
        [Route("/api/v1/auth/profile")]    
        [ProducesResponseType(typeof(Result<UserControlAccessVOViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetControlAccessAsync()
        {
            try
            {
                var userId = (int)GetUserLogged().Id;
                var userLogged = await _userApplication.GetControlAccessAsync(userId);
                userLogged.Id = userId;
                userLogged.Name = GetUserLogged().Name;
                userLogged.Email = GetUserLogged().Email;
                return OkOrDefault(userLogged);
            }
            catch (Exception ex)
            {
                _logger.LogError($"InternalServerError for {nameof(GetControlAccessAsync)} with exception: { JsonConvert.SerializeObject(ex)}");
                return InternalServerError(new Exception("Internal server error!"));
            }
        }
    }
}