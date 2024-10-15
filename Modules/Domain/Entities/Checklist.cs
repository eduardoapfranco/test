using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enum;
using Infra.CrossCutting.Repository;

namespace Domain.Entities
{
    public class Checklist : BaseEntityDates<int>
    {
        public Checklist()
        {
            Active = (int) BoolEnum.YES;
            VisibleApp = (int)BoolEnum.YES;
            CheckEnable = (int)BoolEnum.YES;
        }

        public string Title { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public int Type { get; set; }
        public int? CategoryId { get; set; }
        public int? UserId { get; set; }
        public byte Active { get; set; }
        public byte VisibleApp { get; set; }
        public byte CheckEnable { get; set; }
        [NotMapped]
        public int? GroupId { get; set; }
    }
}
