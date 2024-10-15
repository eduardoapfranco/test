using Application.AppServices.CategoryApplication.ViewModel;
using Application.AppServices.ReminderApplication.ViewModel;
using Application.Interfaces;
using ConstruaApp.Api.Controllers;
using FizzWare.NBuilder;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Controllers
    {
    public class ReminderControllerTests
        {
        private DomainNotificationHandler _notificationHandler;
        private Mock<IReminderApplication> _reminderApplicationMock;
        private Mock<ILogger<ReminderController>> _loggerMock;
        private ReminderController _controller;

        private long userId = 1;

        public ReminderControllerTests()
            {
            _reminderApplicationMock = new Mock<IReminderApplication>();
            _notificationHandler = new DomainNotificationHandler();
            _loggerMock = new Mock<ILogger<ReminderController>>();
            _controller = new ReminderController(_notificationHandler, _reminderApplicationMock.Object, _loggerMock.Object);
            }

        [Fact(DisplayName = "Should return list of reminders after get async")]
        [Trait("[WebApi.Controllers]-ReminderController", "Controllers-AllReminders")]
        public async Task AllReminders()
            {
            _reminderApplicationMock.Setup(x => x.GetRemindersAsync(It.IsAny<long>())).ReturnsAsync(new List<ReminderViewModel>());
            var result = await _controller.AllReminders();
            Assert.IsType<OkObjectResult>(result);
            }

        [Fact(DisplayName = "Should sync reminders async")]
        [Trait("[WebApi.Controllers]-ReminderController", "Controllers-SyncReminders")]
        public async Task SyncReminders()
            {
            _reminderApplicationMock.Setup(x => x.SyncAsync(It.IsAny<long>(), It.IsAny<List<ReminderViewModel>>()))
                                    .ReturnsAsync(new ReminderSyncResponse());

            var result = await _controller.SyncReminders(new List<ReminderViewModel>());
            Assert.IsType<OkObjectResult>(result);
            }

        [Fact(DisplayName = "Should add a reminder async")]
        [Trait("[WebApi.Controllers]-ReminderController", "Controllers-AddReminders")]
        public async Task AddReminder()
            {
            var reminder = new ReminderViewModel()
                {
                AppId = Guid.NewGuid().ToString(),
                Deleted = false,
                Description = null,
                Title = "Teste",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                LastUpdated = DateTime.Now,
                };

            _reminderApplicationMock.Setup(x => x.AddAsync(It.IsAny<long>(), It.IsAny<ReminderViewModel>())).ReturnsAsync(new ReminderViewModel());
            var result = await _controller.AddReminder(reminder);
            Assert.IsType<OkObjectResult>(result);
            }

        [Fact(DisplayName = "Should update a reminder async")]
        [Trait("[WebApi.Controllers]-ReminderController", "Controllers-UpdateReminder")]
        public async Task UpdateReminder()
            {
            var reminder = new ReminderViewModel()
                {
                AppId = Guid.NewGuid().ToString(),
                Deleted = false,
                Description = null,
                Title = "Teste",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                LastUpdated = DateTime.Now,
                };

            _reminderApplicationMock.Setup(x => x.GetByAppIdAsync(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(new ReminderViewModel());
            _reminderApplicationMock.Setup(x => x.UpdateAsync(It.IsAny<long>(), It.IsAny<ReminderViewModel>())).ReturnsAsync(new ReminderViewModel());

            var result = await _controller.UpdateReminder(reminder);
            Assert.IsType<OkObjectResult>(result);
            }

        [Fact(DisplayName = "Should delete a reminder async")]
        [Trait("[WebApi.Controllers]-ReminderController", "Controllers-DeleteReminder")]
        public async Task DeleteReminder()
            {
            var Id = Guid.NewGuid().ToString();

            _reminderApplicationMock.Setup(x => x.GetByAppIdAsync(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(new ReminderViewModel());
            _reminderApplicationMock.Setup(x => x.UpdateAsync(It.IsAny<long>(), It.IsAny<ReminderViewModel>())).ReturnsAsync(new ReminderViewModel());

            var result = await _controller.DeleteReminder(Id);
            Assert.IsType<OkObjectResult>(result);
            }
        }
    }
