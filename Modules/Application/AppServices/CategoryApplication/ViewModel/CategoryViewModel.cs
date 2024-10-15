using Infra.CrossCutting.UoW.Models;
using System;
using Newtonsoft.Json;

namespace Application.AppServices.CategoryApplication.ViewModel
{
    public class CategoryViewModel : BaseResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [JsonProperty("codigo_interno")]
        public string InnerCode { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        [JsonProperty("parent_id")]
        public int? ParentId { get; set; }
        public string Icon { get; set; }
        [JsonProperty("user_id")]
        public int? UserId { get; set; }
        [JsonProperty("ativo")]
        public byte Active { get; set; }
        [JsonProperty("visible_app")]
        public byte VisibleApp { get; set; }
        public string Image { get; set; }
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        [JsonProperty("title_autocomplete")]
        public string TitleAutoComplete { get; set; }
    }
}
