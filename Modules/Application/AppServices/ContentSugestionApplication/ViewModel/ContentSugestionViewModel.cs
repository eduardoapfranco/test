using Infra.CrossCutting.UoW.Models;
using System;
using Newtonsoft.Json;

namespace Application.AppServices.ContentSugestionApplication.ViewModel
    {
    public class ContentSugestionViewModel : BaseResult
    { 
        public long Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        [JsonProperty("checklist_id")]
        public int? ChecklistId { get; set; }
        [JsonProperty("category_id")]
        public int? CategoryId { get; set; }
        [JsonProperty("user_Id")]
        public int UserId { get; set; }
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        }
}
