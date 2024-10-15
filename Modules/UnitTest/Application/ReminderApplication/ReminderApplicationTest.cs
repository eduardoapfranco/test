using System;
using AutoMapper;
using Domain.Core;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Moq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using UnitTest.Application.CategoryApplication.Faker;
using Xunit;
using App = Application.AppServices;
using System.Linq.Expressions;
using Domain.Entities;
using Application.AppServices.ReminderApplication.ViewModel;
using System.Collections.Generic;
using NuGet.Frameworks;
using Microsoft.Extensions.Logging;

namespace UnitTest.Application.ReminderApplication
    {
    public class ReminderApplicationTest
        {
        private Mock<IReminderDomainService> _reminderDomainServiceMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<ILogger<App.ReminderApplication.ReminderApplication>> _loggerMock;
        private App.ReminderApplication.ReminderApplication _reminderApplication;
        private readonly Fixture _fixture;

        private long userId = 1;
        private string guid1 = Guid.NewGuid().ToString();
        private string guid2 = Guid.NewGuid().ToString();

        public ReminderApplicationTest()
            {
            // configure
            _reminderDomainServiceMock = new Mock<IReminderDomainService>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _loggerMock = new Mock<ILogger<App.ReminderApplication.ReminderApplication>>();
            _reminderApplication = 
                new App.ReminderApplication.ReminderApplication(
                    _reminderDomainServiceMock.Object, 
                    _smartNotificationMock.Object,
                    _loggerMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
            }

        [Fact(DisplayName = "Shoud sync a list of reminders async")]
        [Trait("[Application.AppServices]-ReminderApplication", "Application-SyncAsync")]
        public async Task Sync()
            {
            // arrange
            List<ReminderViewModel> request = new List<ReminderViewModel>()
                {
                new ReminderViewModel()
                    {
                    Id = 1,
                    AppId = guid1,
                    Deleted = false,
                    Description = "Teste",
                    Title = "Teste",
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddDays(1),
                    LastUpdated = DateTime.Now.AddDays(-1)
                    },
                new ReminderViewModel()
                    {
                    Id = 2,
                    AppId = guid2,
                    Deleted = false,
                    Description = "Teste",
                    Title = "Teste",
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddDays(1),
                    LastUpdated = DateTime.Now.AddDays(-1)
                    },
                };

            _reminderDomainServiceMock.Setup(x => x.Sync(It.IsAny<long>(), It.IsAny<IEnumerable<Reminder>>(), It.IsAny<IEnumerable<long>>(), It.IsAny<IEnumerable<Reminder>>())).ReturnsAsync(true);

            // act
            var result = await _reminderApplication.SyncAsync(userId, request);

            // assert
            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Shoud get a list of reminders async")]
        [Trait("[Application.AppServices]-ReminderApplication", "Application-GetListAsync")]
        public async Task GetAll()
            {
            // arrange
            List<Reminder> list = new List<Reminder>()
                {
                new Reminder()
                    {
                    Id = 1,
                    AppId = Guid.NewGuid().ToString(),
                    Deleted = false,
                    Description = "Teste",
                    Title = "Teste",
                    UserId = userId,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddDays(1),
                    LastUpdated = DateTime.Now.AddDays(-1)
                    }
                };

            _reminderDomainServiceMock.Setup(x => x.GetAll(It.IsAny<long>())).ReturnsAsync(list);

            // act
            var result = await _reminderApplication.GetRemindersAsync(userId);

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            }

        [Fact(DisplayName = "Shoud add a reminder async")]
        [Trait("[Application.AppServices]-ReminderApplication", "Application-GetListAsync")]
        public async Task Add()
            {
            var reminder = new Reminder()
                {
                Id = 1,
                AppId = Guid.NewGuid().ToString(),
                Deleted = false,
                Description = "Teste",
                Title = "Teste",
                UserId = userId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(1),
                LastUpdated = DateTime.Now.AddDays(-1)
                };

            var reminderVm = _reminderApplication.Map(reminder);

            _reminderDomainServiceMock.Setup(x => x.Add(It.IsAny<Reminder>())).ReturnsAsync(reminder);

            var result = await _reminderApplication.AddAsync(userId, reminderVm);
            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Shoud get a reminder by App Id async")]
        [Trait("[Application.AppServices]-ReminderApplication", "Application-GetByAppIdAsync")]
        public async Task GetByAppIdAsync()
            {
            var reminder = new Reminder()
                {
                Id = 1,
                AppId = Guid.NewGuid().ToString(),
                Deleted = false,
                Description = "Teste",
                Title = "Teste",
                UserId = userId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(1),
                LastUpdated = DateTime.Now.AddDays(-1)
                };

            _reminderDomainServiceMock.Setup(x => x.GetByAppId(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(reminder);

            var result = await _reminderApplication.GetByAppIdAsync(userId, "bca");
            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Shoud update a reminder async")]
        [Trait("[Application.AppServices]-ReminderApplication", "Application-UpdateAsync")]
        public async Task UpdateAsync()
            {
            var reminder = new Reminder()
                {
                Id = 1,
                AppId = Guid.NewGuid().ToString(),
                Deleted = false,
                Description = "Teste",
                Title = "Teste",
                UserId = userId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(1),
                LastUpdated = DateTime.Now.AddDays(-1)
                };

            _reminderDomainServiceMock.Setup(x => x.Update(It.IsAny<Reminder>())).ReturnsAsync(reminder);
            _reminderDomainServiceMock.Setup(x => x.GetByAppId(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(reminder);

            var result = await _reminderApplication.UpdateAsync(userId, _reminderApplication.Map(reminder));
            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Shoud delete a reminder async")]
        [Trait("[Application.AppServices]-ReminderApplication", "Application-DeleteAsync")]
        public async Task DeleteAsync()
            {
            var reminder = new Reminder()
                {
                Id = 1,
                AppId = Guid.NewGuid().ToString(),
                Deleted = false,
                Description = "Teste",
                Title = "Teste",
                UserId = userId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(1),
                LastUpdated = DateTime.Now.AddDays(-1)
                };

            _reminderDomainServiceMock.Setup(x => x.Delete(It.IsAny<long>())).ReturnsAsync(true);

            var result = await _reminderApplication.DeleteAsync(userId, _reminderApplication.Map(reminder));
            Assert.True(result);
            }

        [Fact(DisplayName = "Shoud delete a reminder by App Id async")]
        [Trait("[Application.AppServices]-ReminderApplication", "Application-DeleteByAppIdAsync")]
        public async Task DeleteByAppIdAsync()
            {
            var reminder = new Reminder()
                {
                Id = 1,
                AppId = Guid.NewGuid().ToString(),
                Deleted = false,
                Description = "Teste",
                Title = "Teste",
                UserId = userId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(1),
                LastUpdated = DateTime.Now.AddDays(-1)
                };

            _reminderDomainServiceMock.Setup(x => x.GetByAppId(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(reminder);
            _reminderDomainServiceMock.Setup(x => x.Delete(It.IsAny<long>())).ReturnsAsync(true);

            var result = await _reminderApplication.DeleteByAppIdAsync(userId, "abc");
            Assert.True(result);
            }
        }
    }
