using Domain.Enum;

namespace Domain.ValueObjects
{
    public class ChecklistSectionExportVO
    {
        public int Id { get; set; }
        public ChecklistTypeEnum Type { get; set; }
        public string Title { get; set; }
        public bool IsCheck { get; set; }
        public int? GroupId { get; set; }
    }
}
