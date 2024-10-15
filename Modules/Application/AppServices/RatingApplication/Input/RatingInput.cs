using Application.AppServices.RatingApplication.Validators;
using FluentValidation.Results;
using Infra.CrossCutting.Validators;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Application.AppServices.RatingApplication.Input
    {
    public class RatingInput : ValidationInput
    {
        [JsonProperty("rating")]
        public int _Rating { get; set; }
        public string Comment { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        [JsonProperty("checklist_id")]
        public int? ChecklistId { get; set; }
        [JsonProperty("category_id")]
        public int? CategoryId { get; set; }
        [JsonProperty("user_Id")]
        public int UserId { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new RatingInputValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
