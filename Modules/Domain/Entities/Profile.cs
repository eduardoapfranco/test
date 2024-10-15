using Infra.CrossCutting.Repository;

namespace Domain.Entities
{
    public class Profile : BaseEntityDates<int>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int PlanId { get; set; }
        public byte Active { get; set; }        
    }
}
