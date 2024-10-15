using Application.AppServices.ContentSugestionApplication.Input;
using Domain.Messages;
using FluentValidation;

namespace Application.AppServices.ContentSugestionApplication.Validators
{
    public class ContentSugestionInputValidator : AbstractValidator<ContentSugestionInput>
    {
        public ContentSugestionInputValidator()
        {
            RuleFor(doc => doc.UserId).NotNull();
            RuleFor(doc => doc.Content).Length(3,190);
            }
    }
}
