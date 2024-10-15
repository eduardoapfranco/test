using Application.AppServices.ContentSugestionApplication.Validators;
using FluentValidation.Results;
using Infra.CrossCutting.Validators;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Application.AppServices.ContentSugestionApplication.Input
    {
    public class ContentSugestionInput : ValidationInput
    {
        public string Title { get; set; }
        public int Type { get; set; }
        public string Content { get; set; }
        [JsonProperty("checklist_id")]
        public int? ChecklistId { get; set; }
        [JsonProperty("category_id")]
        public int? CategoryId { get; set; }
        [JsonProperty("user_Id")]
        public int UserId { get; set; }
        public override bool IsValid()
        {
            ValidationResult = new ContentSugestionInputValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
