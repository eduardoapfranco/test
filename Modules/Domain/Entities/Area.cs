using Infra.CrossCutting.Repository;

namespace Domain.Entities
    {
    public class Area : BaseEntityDates<int>
    {
        public string Name { get; set; }
    }
}
