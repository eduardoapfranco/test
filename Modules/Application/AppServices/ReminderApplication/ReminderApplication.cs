using Application.AppServices.ReminderApplication.ViewModel;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.AppServices.ReminderApplication
    {
    public class ReminderApplication : BaseValidationService, IReminderApplication
        {
        private readonly IReminderDomainService _reminderDomainService;
        private readonly ISmartNotification _notification;
        private readonly ILogger<ReminderApplication> _logger;

        public ReminderApplication(IReminderDomainService reminderDomainService, 
            ISmartNotification notification,
            ILogger<ReminderApplication> logger) : base(notification)
            {
            _reminderDomainService = reminderDomainService;
            _notification = notification;
            }

        public ReminderViewModel Map(Reminder reminder)
            {
            return new ReminderViewModel()
                {
                Id = reminder.Id,
                AppId = reminder.AppId,
                Title = reminder.Title,
                Description = reminder.Description,
                StartTime = reminder.StartTime,
                EndTime = reminder.EndTime,
                LastUpdated = reminder.LastUpdated,
                Deleted = reminder.Deleted
                };
            }

        public Reminder Map(long userId, ReminderViewModel reminder)
            {
            return new Reminder()
                {
                Id = reminder.Id,
                AppId = reminder.AppId,
                Description = reminder.Description,
                EndTime = reminder.EndTime,
                StartTime = reminder.StartTime,
                Title = reminder.Title,
                UserId = userId,
                LastUpdated = DateTime.Now,
                Deleted = reminder.Deleted
                };
            }

        public async Task<IEnumerable<ReminderViewModel>> GetRemindersAsync(long userId)
            {
            var list = await _reminderDomainService.GetAll(userId);
            var mappedList = list.Select(p => Map(p));
            return mappedList;
            }

        public async Task<ReminderSyncResponse> SyncAsync(long userId, List<ReminderViewModel> appReminders)
            {

            var dbReminders = (await GetRemindersAsync(userId)).ToList();

            // Primeiro processa as deleções em ambas as pontas
            var toDeleteDb = dbReminders
                .Where(db => db.Deleted || appReminders.Any(app => app.AppId == db.AppId && app.Deleted))   // Existem no DB mas não existem (ou estão deletados) no App
                .ToList();

            var toDeleteApp = appReminders
                .Where(app => app.Deleted || dbReminders.Any(db => db.AppId == app.AppId && db.Deleted))          // Existem no App mas não existem (ou estão deletados) no DB
                .ToList();

            // Agora filtra as deleções em ambas as listas
            dbReminders = dbReminders.Where(db => !toDeleteDb.Any(p => p.AppId == db.AppId) && !toDeleteApp.Any(p => p.AppId == db.AppId)).ToList();
            appReminders = appReminders.Where(app => !toDeleteApp.Any(p => p.AppId == app.AppId) && !toDeleteDb.Any(p => p.AppId == app.AppId)).ToList();

            // Restam apenas os reminders realmente a fazer merge
            var toInsertDb = appReminders
                .Where(app => !dbReminders.Any(db => db.AppId == app.AppId))                    // Existem no App mas nao existem no DB
                .ToList();

            var toUpdateDb = appReminders
                .Where(app => dbReminders.Any(db => db.AppId == app.AppId && app.IsNewer(db)))  // Existem em ambos mas o App é mais novo
                .ToList();

            if (toUpdateDb.Count > 0)
                {
                // Para os que tem de atualizar no banco, seta o Id, que não veio do app
                foreach (var reminder in toUpdateDb)
                    reminder.Id = dbReminders.Where(p => p.AppId == reminder.AppId).Select(p => p.Id).FirstOrDefault();
                }

            var toInsertApp = dbReminders
                .Where(db => !appReminders.Any(app => app.AppId == db.AppId))           // Existem no DB mas não existem no App
                .ToList();

            var toUpdateApp = dbReminders
                .Where(db => appReminders.Any(app => app.AppId == db.AppId && db.IsNewer(app))) // Existem em ambos mas o DB é mais novo
                .ToList();


            IEnumerable<Reminder> mappedInserts = toInsertDb.Select(p => Map(userId, p));
            IEnumerable<Reminder> mappedUpdates = toUpdateDb.Select(p => Map(userId, p));
            IEnumerable<long> mappedDeletes = toDeleteDb.Select(p => p.Id);

            if (await _reminderDomainService.Sync(userId, mappedInserts, mappedDeletes, mappedUpdates))
                {
                return new ReminderSyncResponse()
                    {
                    toDelete = toDeleteApp.Select(p => p.AppId),
                    toInsert = toInsertApp,
                    toUpdate = toUpdateApp,
                    };
                }

            return null;
            }

        public async Task<ReminderViewModel> AddAsync(long userId, ReminderViewModel reminder)
            {
            if (!reminder.IsValidForInsert())
                {
                NotifyErrorsAndValidation(_notification.EmptyPositions(), reminder);
                _logger.LogWarning($"Init register user with param invalid {nameof(AddAsync)} with param: {JsonConvert.SerializeObject(reminder)}");
                return default;
                }

            if (String.IsNullOrWhiteSpace(reminder.AppId))
                reminder.AppId = Guid.NewGuid().ToString();

            reminder.LastUpdated = DateTime.Now;
            reminder.Deleted = false;

            var mapped = Map(userId, reminder);
            var result = await _reminderDomainService.Add(mapped);
            if (result == default)
                return default;

            return Map(result);
            }

        public async Task<ReminderViewModel> GetByAppIdAsync(long userId, string AppId)
            {
            var result = await _reminderDomainService.GetByAppId(userId, AppId);
            if (result == default)
                return default;

            return Map(result);
            }

        public async Task<ReminderViewModel> UpdateAsync(long userId, ReminderViewModel reminder)
            {
            if (!reminder.IsValidForUpdate())
                {
                NotifyErrorsAndValidation(_notification.EmptyPositions(), reminder);
                _logger.LogWarning($"Init register user with param invalid {nameof(UpdateAsync)} with param: {JsonConvert.SerializeObject(reminder)}");
                return default;
                }

            var oldReminder = await GetByAppIdAsync(userId, reminder.AppId);
            if (oldReminder == default)
                {
                // Preciso retornar "Lembrete não encontrado"
                return default;
                }

            reminder.Id = oldReminder.Id;
            reminder.LastUpdated = DateTime.Now;

            var mapped = Map(userId, reminder);
            var result = await _reminderDomainService.Update(mapped);
            if (result == default)
                return default;

            return Map(result);
            }

        public async Task<bool> DeleteAsync(long userId, ReminderViewModel reminder)
            {
            if (!reminder.IsValidForDelete())
                {
                NotifyErrorsAndValidation(_notification.EmptyPositions(), reminder);
                _logger.LogWarning($"Init register user with param invalid {nameof(DeleteAsync)} with param: {JsonConvert.SerializeObject(reminder)}");
                return false;
                }

            var mapped = Map(userId, reminder);
            return await _reminderDomainService.Delete(mapped.Id);
            }

        public async Task<bool> LogicalDeleteAsync(long userId, ReminderViewModel reminder)
            {
            if (!reminder.IsValidForDelete())
                {
                NotifyErrorsAndValidation(_notification.EmptyPositions(), reminder);
                _logger.LogWarning($"Init register user with param invalid {nameof(LogicalDeleteAsync)} with param: {JsonConvert.SerializeObject(reminder)}");
                return false;
                }

            reminder = await GetByAppIdAsync(userId, reminder.AppId);
            if (reminder == default)
                {
                // Preciso retornar "Lembrete não encontrado"
                return false;
                }

            reminder.Deleted = true;
            var result = await UpdateAsync(userId, reminder);

            return result != default;
            }

        public async Task<bool> DeleteByAppIdAsync(long userId, string AppId)
            {
            var reminder = await GetByAppIdAsync(userId, AppId);
            if (reminder == default)
                return false;

            var mapped = Map(userId, reminder);
            return await _reminderDomainService.Delete(mapped.Id);
            }

        }
    }
