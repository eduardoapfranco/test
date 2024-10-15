using System;
using Domain.Enum;
using Infra.CrossCutting.Repository;

namespace Domain.Entities
{
    public class ChecklistSqlite : BaseEntityDates<int>
    {
        public ChecklistSqlite(int id, string title, string content, int order, int type, int? categoryId, int? userId, int active, int visibleApp, int checkEnable, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            Title = title;
            Content = content;
            Order = order;
            Type = type;
            CategoryId = categoryId;
            UserId = userId;
            Active = active;
            VisibleApp = visibleApp;
            CheckEnable = checkEnable;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public string Title { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public int Type { get; set; }
        public int? CategoryId { get; set; }
        public int? UserId { get; set; }
        public int Active { get; set; }
        public int VisibleApp { get; set; }
        public int CheckEnable { get; set; }
    }
}
