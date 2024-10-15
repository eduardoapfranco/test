using Infra.CrossCutting.Repository;

namespace Domain.Entities
{
    public class ProfileCategory : BaseEntityDates<int>
    {
        public int ProfileId { get; set; }
        public int CategoryId { get; set; }        
    }
}
