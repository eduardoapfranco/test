using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;
using Infra.CrossCutting.Domain.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
    {
    public class ReminderDomainService : DomainService<Reminder, long, IUnitOfWork>, IReminderDomainService
        {
        private readonly ISmartNotification _notification;
        private readonly ILogger<ReminderDomainService> _logger;

        public ReminderDomainService(
           IReminderRepository reminderRepository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<ReminderDomainService> logger
       ) : base(reminderRepository, unitOfWork, messageHandler)
            {
            _notification = notification;
            _logger = logger;
            }

        public async Task<IEnumerable<Reminder>> GetAll(long userId)
            {
            return await _unitOfWork.Reminder.SelectFilterAsync(x => x.UserId == userId);
            }

        public async Task<bool> Sync(long userId, IEnumerable<Reminder> toInsert, IEnumerable<long> toDelete, IEnumerable<Reminder> toUpdate)
            {
            await using (_unitOfWork.BeginTransaction())
                {
                bool result = await _unitOfWork.Reminder.InsertAllAsync(toInsert);
                if (!result)
                    {
                    _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao inserir lembretes");
                    return false;
                    }

                foreach (var reminder in toUpdate)
                    {
                    try
                        {
                        await _unitOfWork.Reminder.UpdateAsync(reminder);
                        }
                    catch (Exception e)
                        {
                        _logger.LogError("Ao atualizar lembrete {0}: {1}", JsonConvert.SerializeObject(reminder), e.Message);
                        _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao atualizar lembretes");
                        return false;
                        }
                    }
                // Aqui seria bom um DeleteManyAsync, mas...
                foreach (var id in toDelete)
                    {
                    result = await _unitOfWork.Reminder.DeleteAsync(id);
                    if (!result)
                        {
                        _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao excluir lembretes");
                        return false;
                        }
                    }

                Commit();
                return true;
                }
            }

        public async Task<Reminder> Add(Reminder reminder)
            {
            try
                {
                await using (_unitOfWork.BeginTransaction())
                    {
                    var result = await _unitOfWork.Reminder.InsertAsync(reminder);
                    if (result == default)
                        {
                        _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao inserir lembretes");
                        return default;
                        }

                    Commit();
                    return result;
                    }
                }
            catch (Exception e)
                {
                _logger.LogError("Ao adicionar lembrete {0}: {1}", JsonConvert.SerializeObject(reminder), e.Message);
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao inserir lembretes");
                return default;
                }
            }

        public async Task<Reminder> GetByAppId(long userId, string AppId)
            {
            try
                {
                var result = await _unitOfWork.Reminder.SelectFilterAsync(p => p.UserId == userId && p.AppId == AppId);
                if (result.Count() < 1)
                    {
                    _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Lembrete não encontrado");
                    return default;
                    }

                return result.ToArray()[0];
                }
            catch (Exception e)
                {
                _logger.LogError("Ao recuperar lembrete: {0}", e.Message);
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao recuperar lembrete");
                return default;
                }
            }

        public async Task<Reminder> Update(Reminder reminder)
            {
            try
                {
                await using (_unitOfWork.BeginTransaction())
                    {
                    await _unitOfWork.Reminder.UpdateAsync(reminder);
                    Commit();
                    return reminder;
                    }
                }
            catch (Exception e)
                {
                _logger.LogError("Ao atualizar lembrete {0}: {1}", JsonConvert.SerializeObject(reminder), e.Message);
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao atualizar lembretes");
                return default;
                }
            }

        public async Task<bool> Delete(long Id)
            {
            try
                {
                await using (_unitOfWork.BeginTransaction())
                    {
                    var reminder = await _unitOfWork.Reminder.SelectByIdAsync(Id);
                    if (reminder == default)
                        throw new Exception("Lembrete {0} não encontrado");
                    reminder.Deleted = true;

                    var result = await _unitOfWork.Reminder.UpdateAsync(reminder); 
                    if (result == default)
                        {
                        _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao excluir lembretes");
                        return false;
                        }

                    Commit();
                    return true;
                    }
                }
            catch (Exception e)
                {
                _logger.LogError("Ao excluir lembrete {0}: {1}", Id, e.Message);
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao excluir lembretes");
                return false;
                }
            }
        }
    }
