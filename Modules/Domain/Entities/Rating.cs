using Infra.CrossCutting.Repository;

namespace Domain.Entities
    {
    public class Rating : BaseEntityDates<int>
    {
        public int _Rating { get; set; }
        public string Comment { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? ChecklistId { get; set; }
        public int? CategoryId { get; set; }
        public int UserId { get; set; }
    }
}
