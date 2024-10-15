using Domain.Enum;
using Infra.CrossCutting.Repository;
using System;

namespace Domain.Entities
{
    public class Category : BaseEntityDates<int>
    {
        public Category()
        {
            Active = (int) BoolEnum.YES;
            VisibleApp = (int)BoolEnum.YES;
        }

        public string Title { get; set; }
        public string InnerCode { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public int? ParentId { get; set; }
        public string Icon { get; set; }
        public int? UserId { get; set; }
        public byte Active { get; set; }
        public byte VisibleApp { get; set; }
        public string Image { get; set; }
    }
}
