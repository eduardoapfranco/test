using Application.AppServices.UserApplication.Input;
using Domain.Messages;
using FluentValidation;

namespace Application.AppServices.UserApplication.Validators
{
    public class UserRequestPasswordResetInputValidator : AbstractValidator<UserRequestPasswordResetInput>
    {
        public UserRequestPasswordResetInputValidator()
        {
            RuleFor(doc => doc.Email).NotNull().OverridePropertyName(UserMessages.EmailRequired);
            RuleFor(doc => doc.Email).EmailAddress().OverridePropertyName(UserMessages.EmailInvalid);
            RuleFor(doc => doc.Email).Length(3, 190).OverridePropertyName(UserMessages.EmailLength);
        }
    }
}
