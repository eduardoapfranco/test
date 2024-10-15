using Infra.CrossCutting.Repository;

namespace Domain.Entities
    {
    public class ContentSugestion : BaseEntityDates<int>
    {
        public int Type { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? ChecklistId { get; set; }
        public int? CategoryId { get; set; }
        public int UserId { get; set; }
        }
}
