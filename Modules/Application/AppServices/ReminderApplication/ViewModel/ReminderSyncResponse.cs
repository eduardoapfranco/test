using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AppServices.ReminderApplication.ViewModel
    {
    public class ReminderSyncResponse
        {
        public IEnumerable<ReminderViewModel> toInsert { get; set; }
        public IEnumerable<ReminderViewModel> toUpdate { get; set; }
        public IEnumerable<string> toDelete { get; set; }
        }
    }
