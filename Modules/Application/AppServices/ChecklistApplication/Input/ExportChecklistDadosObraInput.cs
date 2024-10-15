
using Application.AppServices.ChecklistApplication.Validators;
using Infra.CrossCutting.Validators;
using System;

namespace Application.AppServices.ChecklistApplication.Input
{
    public class ExportChecklistDadosObraInput : ValidationInput
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Nome { get; set; }
        public string Responsavel { get; set; }
        public string Contratante { get; set; }
        public string Endereco { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Termino { get; set; }
        public decimal? Metragem { get; set; }
        public decimal? ValorEstimado { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ExportChecklistDadosObraInput()
        {
            Status = "Não iniciada";
        }

        public override bool IsValid()
            {
            ValidationResult = new ExportChecklistDadosObraInputValidator().Validate(this);
            return ValidationResult.IsValid;
            }
        }
}
