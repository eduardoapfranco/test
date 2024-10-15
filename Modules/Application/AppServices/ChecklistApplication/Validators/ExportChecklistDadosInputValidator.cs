using Application.AppServices.ChecklistApplication.Input;
using Domain.Enum;
using Domain.Messages;
using FluentValidation;

namespace Application.AppServices.ChecklistApplication.Validators
{
    public class ExportChecklistDadosInputValidator : AbstractValidator<ExportChecklistDadosInput>
    {
        public ExportChecklistDadosInputValidator()
        {
            RuleFor(doc => doc.DataComprovante).NotNull();
            RuleFor(doc => doc.TipoExportacao).IsInEnum().OverridePropertyName(GenericMessages.EnumExportPDFRequired);
            RuleForEach(doc => doc.Fotos).SetValidator(new ExportChecklistDadosFotoInputValidator());
            RuleFor(doc => doc.Obra).SetValidator(new ExportChecklistDadosObraInputValidator());
            }
    }
}
