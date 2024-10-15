using Infra.CrossCutting.Repository;

namespace Domain.Entities
{
    public class ProfileFunctionality : BaseEntityDates<int>
    {
        public int ProfileId { get; set; }
        public int FunctionalityId { get; set; }
    }
}
