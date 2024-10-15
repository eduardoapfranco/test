using Application.AppServices.ConstructionApplication.Validators;
using FluentValidation.Results;
using Infra.CrossCutting.Validators;
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Application.AppServices.ConstructionApplication.Input
    {
    public class ConstructionInput : ValidationInput
        {
        public int Id { get; set; }
        [JsonProperty("app_id")]
        public string AppId { get; set; }
        public int UserId { get; set; }
        public string Nome { get; set; }
        public string Responsavel { get; set; }
        public string Contratante { get; set; }
        public string Endereco { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Termino { get; set; }
        public decimal Metragem { get; set; }
        [JsonProperty("valor_estimado")]
        public decimal ValorEstimado { get; set; }
        public string Status { get; set; }
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public bool Deleted { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
        public override bool IsValid()
        {
            ValidationResult = new ConstructionInputValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
