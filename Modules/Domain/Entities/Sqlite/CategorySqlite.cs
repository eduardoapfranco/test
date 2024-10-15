using System;
using Domain.Enum;
using Infra.CrossCutting.Repository;

namespace Domain.Entities.Sqlite
{
    public class CategorySqlite: BaseEntity<int>
    {

        public CategorySqlite(int id, string title, string innerCode, string content, int order, int? parentId, string icon, int active, int visibleApp, string image, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            Title = title;
            InnerCode = innerCode;
            Content = content;
            Order = order;
            ParentId = parentId;
            Icon = icon;
            Active = active;
            VisibleApp = visibleApp;
            Image = image;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public string Title { get; set; }
        public string InnerCode { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public int? ParentId { get; set; }
        public string Icon { get; set; }
        public int Active { get; set; }
        public int VisibleApp { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
