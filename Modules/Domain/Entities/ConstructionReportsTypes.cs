using Infra.CrossCutting.Repository;

namespace Domain.Entities
    {
    public class ConstructionReportsTypes : BaseEntityDates<int>
    {
       
        public string Name { get; set; }
        
    }
}
