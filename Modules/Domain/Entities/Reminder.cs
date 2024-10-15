using Infra.CrossCutting.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
    {
    public class Reminder : BaseEntity<long>
        {
        public string AppId { get; set; }
        public long UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool Deleted { get; set; }
        }
    }
