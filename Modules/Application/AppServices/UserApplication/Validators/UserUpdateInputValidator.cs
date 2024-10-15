using Application.AppServices.UserApplication.Input;
using Domain.Messages;
using FluentValidation;

namespace Application.AppServices.UserApplication.Validators
{
    public class UserUpdateInputValidator : AbstractValidator<UserUpdateInput>
    {
        public UserUpdateInputValidator()
        {
            RuleFor(doc => doc.Name).NotNull().OverridePropertyName(UserMessages.NameRequired);
            RuleFor(doc => doc.Name).Length(3,190).OverridePropertyName(UserMessages.NameLength);
        }
    }
}
