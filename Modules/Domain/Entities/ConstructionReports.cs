using Domain.Enum;
using Infra.CrossCutting.Repository;
using System;

namespace Domain.Entities
    {
    public class ConstructionReports : BaseEntityDates<int>
    {
        public int ConstructionId { get; set; }
        public string Title { get; set; }
        public string MimeType { get; set; }
        public int PicturesQuantity { get; set; }
        public string Comments { get; set; }
        public string Guarantee { get; set; }
        public decimal? Value { get; set; }
        public decimal? Discount { get; set; }
        public ConstructionReportType TypeId { get; set; }
        public DateTime? AssociatedDate { get; set; }
        public string BlobId { get; set; }
    }
}
