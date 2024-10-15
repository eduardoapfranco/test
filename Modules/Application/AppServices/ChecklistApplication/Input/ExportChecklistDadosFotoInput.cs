using Application.AppServices.ChecklistApplication.Validators;
using Application.AppServices.UserApplication.Validators;
using Domain.Enum;
using Infra.CrossCutting.Validators;

namespace Application.AppServices.ChecklistApplication.Input
{
    public class ExportChecklistDadosFotoInput : ValidationInput
    {
        public string Base64 { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string Nome { get; set; }
        public int TipoMidia { get; set; }
        public string Caption { get; set; }

        public ExportChecklistDadosFotoInput()
        {
            
        }

        public override bool IsValid()
            {
            ValidationResult = new ExportChecklistDadosFotoInputValidator().Validate(this);
            return ValidationResult.IsValid;
            }
        }
}
