using Application.AppServices.ReminderApplication.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
    {
    public interface IReminderApplication
        {
        Task<IEnumerable<ReminderViewModel>> GetRemindersAsync(long userId);
        Task<ReminderSyncResponse> SyncAsync(long userId, List<ReminderViewModel> appReminders);
        Task<ReminderViewModel> AddAsync(long userId, ReminderViewModel reminder);
        Task<ReminderViewModel> GetByAppIdAsync(long userId, string AppId);
        Task<ReminderViewModel> UpdateAsync(long userId, ReminderViewModel reminder);
        Task<bool> DeleteAsync(long userId, ReminderViewModel reminder);
        Task<bool> DeleteByAppIdAsync(long userId, string AppId);
        Task<bool> LogicalDeleteAsync(long userId, ReminderViewModel reminder);
        }
    }
