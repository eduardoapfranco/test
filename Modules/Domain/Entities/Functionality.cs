using Infra.CrossCutting.Repository;

namespace Domain.Entities
{
    public class Functionality : BaseEntityDates<int>
    {
        public Functionality()
        {}

        public string Title { get; set; }
        public string Content { get; set; }
        public int? ParentId { get; set; }
        public byte Active { get; set; }
    }
}
