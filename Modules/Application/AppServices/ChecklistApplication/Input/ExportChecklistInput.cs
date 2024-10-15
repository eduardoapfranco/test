using Application.AppServices.ChecklistApplication.Validators;
using Domain.Enum;
using Infra.CrossCutting.Validators;

namespace Application.AppServices.ChecklistApplication.Input
{
    public class ExportChecklistInput : ValidationInput
    {
        public string ConstructionAppId { get; set; }

        public string Title { get; set; }

        public ExportTypeChecklistEnum Type { get; set; }
        public int[] Ids { get; set; }
        public int CategoryId { get; set; }

        public ExportChecklistDadosInput Dados { get; set; }

        public ExportChecklistInput()
        {
            Ids = new int[] { };
            Type = ExportTypeChecklistEnum.ALL;
        }

        public override bool IsValid()
        {
            ValidationResult = new ExportChecklistInputValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
