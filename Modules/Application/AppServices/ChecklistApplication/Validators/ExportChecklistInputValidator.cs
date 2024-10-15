using Application.AppServices.ChecklistApplication.Input;
using Application.AppServices.ChecklistApplication.Validators;
using Domain.Enum;
using Domain.Messages;
using FluentValidation;

namespace Application.AppServices.ChecklistApplication.Validators
{
    public class ExportChecklistInputValidator : AbstractValidator<ExportChecklistInput>
    {
        public ExportChecklistInputValidator()
        {
            RuleFor(doc => doc.CategoryId).NotNull().OverridePropertyName(GenericMessages.CategoryId);
            RuleFor(doc => doc.Ids).NotNull().OverridePropertyName(GenericMessages.FieldRequired);
            RuleFor(doc => doc.CategoryId).GreaterThan(0).OverridePropertyName(GenericMessages.CategoryIdNumber);
            RuleFor(doc => doc.Type).IsInEnum().OverridePropertyName(GenericMessages.EnumExportPDFRequired);
            RuleFor(doc => doc.Dados).SetValidator(new ExportChecklistDadosInputValidator());
            }
    }
}
