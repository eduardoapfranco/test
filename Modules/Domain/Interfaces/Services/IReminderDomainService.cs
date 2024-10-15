using Domain.Entities;
using Infra.CrossCutting.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
    {
    public interface IReminderDomainService : IDomainService<Reminder, long>
        {
        Task<bool> Sync(long userId, IEnumerable<Reminder> toInsert, IEnumerable<long> toDelete, IEnumerable<Reminder> toUpdate);
        Task<Reminder> Add(Reminder reminder);
        Task<Reminder> GetByAppId(long userId, string AppId);
        Task<Reminder> Update(Reminder reminder);
        Task<bool> Delete(long Id);
        Task<IEnumerable<Reminder>> GetAll(long userId);
        }
    }
