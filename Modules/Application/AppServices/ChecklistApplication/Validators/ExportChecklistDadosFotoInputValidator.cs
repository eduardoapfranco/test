using Application.AppServices.ChecklistApplication.Input;
using Domain.Enum;
using Domain.Messages;
using FluentValidation;

namespace Application.AppServices.ChecklistApplication.Validators
{
    public class ExportChecklistDadosFotoInputValidator : AbstractValidator<ExportChecklistDadosFotoInput>
    {
        public ExportChecklistDadosFotoInputValidator()
        {
            RuleFor(doc => doc.Base64).NotNull();
            RuleFor(doc => doc.Base64).NotEmpty();
            }
    }
}
