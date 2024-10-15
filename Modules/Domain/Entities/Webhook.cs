using Infra.CrossCutting.Repository;

namespace Domain.Entities
    {
    public class Webhook : BaseEntityDates<int>
    {
        public string Partner { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public string Content { get; set; }
    }
}
