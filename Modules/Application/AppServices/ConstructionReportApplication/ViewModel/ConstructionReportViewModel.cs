using Domain.Enum;
using Infra.CrossCutting.UoW.Models;
using System;
using Newtonsoft.Json;

namespace Application.AppServices.ConstructionReportApplication.ViewModel
    {
    public class ConstructionReportViewModel : BaseResult
        {

        public int Id { get; set; }
        [JsonProperty("construction_id")]
        public int ConstructionId { get; set; }
        public string Title { get; set; }
        [JsonProperty("mime_type")]
        public string MimeType { get; set; }
        [JsonProperty("pictures_quantity")]
        public int PicturesQuantity { get; set; }
        public string Comments { get; set; }
        public string Guarantee { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        public decimal Value { get; set; }
        public decimal Discount { get; set; }
        [JsonProperty("type_id")]
        public ConstructionReportType TypeId { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        }
    }
