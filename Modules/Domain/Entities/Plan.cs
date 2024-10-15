using Domain.Enum;
using Infra.CrossCutting.Repository;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Plan : BaseEntityDates<int>
    {
        public string IdGoogle { get; set; }
        public string IdApple { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public decimal Value { get; set; }
        public decimal ValueSave { get; set; }
        public decimal ValueFinally { get; set; }
        public PlanTypesEnum Type { get; set; }
        public byte Active { get; set; }
        [NotMapped]
        public PlanType PlanType { get; set; }        
    }
}