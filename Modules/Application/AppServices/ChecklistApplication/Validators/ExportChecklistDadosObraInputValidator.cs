using Application.AppServices.ChecklistApplication.Input;
using Domain.Enum;
using Domain.Messages;
using FluentValidation;

namespace Application.AppServices.ChecklistApplication.Validators
{
    public class ExportChecklistDadosObraInputValidator : AbstractValidator<ExportChecklistDadosObraInput>
    {
        public ExportChecklistDadosObraInputValidator()
        {
            RuleFor(doc => doc.Id).NotNull();
            RuleFor(doc => doc.UserId).NotNull();
            RuleFor(doc => doc.Nome).NotEmpty();
            RuleFor(doc => doc.Inicio).NotNull();
            RuleFor(doc => doc.Termino).NotNull();
        }
    }
}
