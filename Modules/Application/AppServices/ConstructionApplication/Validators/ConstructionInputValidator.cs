using Application.AppServices.ConstructionApplication.Input;
using Domain.Messages;
using FluentValidation;

namespace Application.AppServices.ConstructionApplication.Validators
{
    public class ConstructionInputValidator : AbstractValidator<ConstructionInput>
    {
        public ConstructionInputValidator()
        {
            RuleFor(doc => doc.UserId).NotNull();
            RuleFor(doc => doc.Nome).NotEmpty();
            RuleFor(doc => doc.Nome).NotNull();
            RuleFor(doc => doc.Nome).Length(3, 256);
            RuleFor(doc => doc.Responsavel).NotEmpty();
            RuleFor(doc => doc.Contratante).NotEmpty();
            RuleFor(doc => doc.Responsavel).Length(3, 256);
            RuleFor(doc => doc.Contratante).Length(3, 256);
            RuleFor(doc => doc.Inicio).NotNull();
            RuleFor(doc => doc.Termino).NotNull();
            }
    }
}
