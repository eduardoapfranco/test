using Infra.CrossCutting.Repository;

namespace Domain.Entities
{
    public class PlanType : BaseEntityDates<int>
    {
        public string Title { get; set; }       

        public int Days { get; set; }
    }
}
