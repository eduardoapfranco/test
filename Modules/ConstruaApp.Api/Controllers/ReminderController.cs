using Application.AppServices.ReminderApplication.ViewModel;
using Application.Interfaces;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ConstruaApp.Api.Controllers
    {
    [Route("api/v1/reminder")]
    [Authorize]
    [ApiController]
    public class ReminderController : BaseController
        {
        private readonly IReminderApplication _reminderApplication;
        private readonly ILogger<ReminderController> _logger;

        public ReminderController(INotificationHandler<DomainNotification> notification,
            IReminderApplication reminderApplication,
            ILogger<ReminderController> logger) : base(notification)
            {
            _reminderApplication = reminderApplication;
            _logger = logger;
            }

        [Route("all"), HttpGet]
        [ProducesResponseType(typeof(Result<IEnumerable<ReminderViewModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AllReminders()
            {
            _logger.LogInformation("ReminderController all-reminders executed at {date}", DateTime.UtcNow);

            long userId = (int)GetUserLogged().Id;
            var result = await _reminderApplication.GetRemindersAsync(userId);

            return OkOrDefault(result);
            }


        [Route("sync"), HttpPost]
        [ProducesResponseType(typeof(Result<ReminderSyncResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SyncReminders([FromBody] List<ReminderViewModel> appReminders)
            {
            _logger.LogInformation("ReminderController sync-reminders executed at {date}", DateTime.UtcNow);

            long userId = (int)GetUserLogged().Id;
            var response = await _reminderApplication.SyncAsync(userId, appReminders);

            return OkOrDefault(response);
            }

        [Route("test"), HttpPost]
        [ProducesResponseType(typeof(Result<ReminderSyncResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> TestReminders([FromBody] List<ReminderViewModel> appReminders)
        {
            _logger.LogInformation("ReminderController sync-reminders executed at {date}", DateTime.UtcNow);

            long userId = (int)GetUserLogged().Id;
            var response = await _reminderApplication.SyncAsync(userId, appReminders);

            return OkOrDefault(response);
        }

        [Route("add"), HttpPost]
        [ProducesResponseType(typeof(Result<ReminderViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddReminder([FromBody] ReminderViewModel reminder)
            {
            _logger.LogInformation("ReminderController add-reminder executed at {date}", DateTime.UtcNow);

            long userId = (int)GetUserLogged().Id;
            var result = await _reminderApplication.AddAsync(userId, reminder);

            return OkOrDefault(result);
            }

        [Route("update"), HttpPut]
        [ProducesResponseType(typeof(Result<ReminderViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateReminder([FromBody] ReminderViewModel reminder)
            {
            _logger.LogInformation("ReminderController update-reminder executed at {date}", DateTime.UtcNow);

            long userId = (int)GetUserLogged().Id;
            reminder.Deleted = false;
            var result = await _reminderApplication.UpdateAsync(userId, reminder);

            return OkOrDefault(result);
            }

        [Route("delete/{AppId}"), HttpDelete]
        [ProducesResponseType(typeof(Result<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteReminder([FromRoute] string AppId)
            {
            _logger.LogInformation("ReminderController delete-reminder executed at {date}", DateTime.UtcNow);

            long userId = (int)GetUserLogged().Id;
            var result = await _reminderApplication.LogicalDeleteAsync(userId, new ReminderViewModel() { AppId = AppId });

            return OkOrDefault(result);
            }
        }
    }
