using Application.AppServices.UserApplication.Input;
using Domain.Messages;
using FluentValidation;

namespace Application.AppServices.UserApplication.Validators
{
    public class UserUpdatePasswordInputValidator : AbstractValidator<UserUpdatePasswordInput>
    {
        public UserUpdatePasswordInputValidator()
        {
            RuleFor(doc => doc.Password).NotNull().OverridePropertyName(UserMessages.PasswordRequired);
            RuleFor(doc => doc.Password).Length(6, 20).OverridePropertyName(UserMessages.PasswordLength);
            RuleFor(doc => doc.Password).Equal(x => x.PasswordConfirm).OverridePropertyName(UserMessages.PasswordEqual);
        }
    }
}
