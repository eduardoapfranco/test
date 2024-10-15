using Infra.CrossCutting.UoW.Models;
using System;
using Newtonsoft.Json;

namespace Application.AppServices.ChecklistApplication.ViewModel
{
    public class ChecklistViewModel : BaseResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public int Type { get; set; }
        [JsonProperty("category_id")]
        public int? CategoryId { get; set; }
        [JsonProperty("user_id")]
        public int? UserId { get; set; }
        [JsonProperty("ativo")]
        public byte Active { get; set; }
        [JsonProperty("visible_app")]
        public byte VisibleApp { get; set; }
        public DateTime? CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        [JsonProperty("check_enable")]
        public byte CheckEnable { get; set; }
    }
}
