using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.UoW;
using Domain.Services;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NuGet.Frameworks;
using UnitTest.Application.CategoryApplication.Faker;
using Xunit;

namespace UnitTest.Domain
    {
    public class ReminderDomainTest
        {
        private Mock<IReminderRepository> _reminderRepositoryMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private ReminderDomainService _reminderDomainService;
        private Mock<ILogger<ReminderDomainService>> _loggerMock;

        private long userId = 1;    // Um usuario
        private string AppId1 = Guid.NewGuid().ToString();
        private string AppId2 = Guid.NewGuid().ToString();
        private string AppId3 = Guid.NewGuid().ToString();


        public ReminderDomainTest()
            {
            _reminderRepositoryMock = new Mock<IReminderRepository>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _loggerMock = new Mock<ILogger<ReminderDomainService>>();

            _reminderDomainService = new ReminderDomainService(
                _reminderRepositoryMock.Object,
                _smartNotificationMock.Object,
                _unitOfWorkMock.Object,
                new DomainNotificationHandler(),
                _loggerMock.Object
                );
            }


        [Fact(DisplayName = "Shoud return true when sync succeeds including synced reminders")]
        [Trait("[Domain.Services]-ReminderDomainService", "TestSync")]
        public async Task TestSync()
            {

            IEnumerable<Reminder> toInsert = new List<Reminder>()  // Inserir um lembrete inicialmente
                {
                new Reminder()
                    {
                    AppId = AppId1,
                    Deleted = false,
                    Description = "Incluido app 1",
                    Title = "Titulo app 1",
                    UserId = userId,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    },
                new Reminder()
                    {
                    AppId = AppId2,
                    Deleted = false,
                    Description = "Incluido app 2",
                    Title = "Titulo app 2",
                    UserId = userId,
                    StartTime = DateTime.Now.AddMinutes(10),
                    EndTime = DateTime.Now.AddHours(1).AddMinutes(10),
                    },
                };
            IEnumerable<long> toDelete = new List<long>() { 1, 4, 66 };  // A deletar
            IEnumerable<Reminder> toUpdate = new List<Reminder>(); // Nada a atualizar inicialmente

            _unitOfWorkMock.Setup(x => x.Reminder.InsertAllAsync(It.IsAny<IEnumerable<Reminder>>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.Reminder.UpdateAsync(It.IsAny<Reminder>())).ReturnsAsync(new Reminder());
            _unitOfWorkMock.Setup(x => x.Reminder.DeleteAsync(It.IsAny<long>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(new CommandResponse(true));

            // Sincroniza
            var result = await _reminderDomainService.Sync(userId, toInsert, toDelete, toUpdate);

            Assert.True(result);
            }


        [Fact(DisplayName = "Shoud return true when adding one reminder")]
        [Trait("[Domain.Services]-ReminderDomainService", "Domain-TestAdd")]
        public async Task TestAdd()
            {
            var lembrete = new Reminder()
                {
                AppId = AppId3,
                Deleted = false,
                Description = "Incluido app 3",
                Title = "Titulo app 3",
                UserId = userId,
                StartTime = DateTime.Now.AddMinutes(10),
                EndTime = DateTime.Now.AddHours(1).AddMinutes(10),
                };

            _unitOfWorkMock.Setup(x => x.Reminder.InsertAsync(It.IsAny<Reminder>())).ReturnsAsync(lembrete);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(new CommandResponse(true));

            var result = await _reminderDomainService.Add(lembrete);
            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Shoud return true when updating one reminder")]
        [Trait("[Domain.Services]-ReminderDomainService", "Domain-TestUpdate")]
        public async Task TestUpdate()
            {
            var lembrete = new Reminder()
                {
                Id = 1,
                AppId = AppId3,
                Deleted = false,
                Description = "Atualizado app 3",
                Title = "Titulo app 3",
                UserId = userId,
                StartTime = DateTime.Now.AddMinutes(10),
                EndTime = DateTime.Now.AddHours(1).AddMinutes(10),
                };

            _unitOfWorkMock.Setup(x => x.Reminder.UpdateAsync(It.IsAny<Reminder>())).ReturnsAsync(lembrete);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(new CommandResponse(true));

            var result = await _reminderDomainService.Update(lembrete);
            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Shoud return true when deleting one reminder")]
        [Trait("[Domain.Services]-ReminderDomainService", "Domain-TestDeleted")]
        public async Task TestDelete()
            {
            var lembrete = new Reminder()
                {
                Id = 1,
                AppId = AppId3,
                Deleted = false,
                Description = "Atualizado app 3",
                Title = "Titulo app 3",
                UserId = userId,
                StartTime = DateTime.Now.AddMinutes(10),
                EndTime = DateTime.Now.AddHours(1).AddMinutes(10),
                };

            _unitOfWorkMock.Setup(x => x.Reminder.SelectByIdAsync(It.IsAny<long>())).ReturnsAsync(lembrete);
            _unitOfWorkMock.Setup(x => x.Reminder.UpdateAsync(It.IsAny<Reminder>())).ReturnsAsync(lembrete);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(new CommandResponse(true));

            var result = await _reminderDomainService.Delete(lembrete.Id);
            Assert.True(result);
            }


        }
    }
