using Application.AppServices.UserApplication.Input;
using Domain.Messages;
using FluentValidation;

namespace Application.AppServices.UserApplication.Validators
{
    public class UserLoginInputValidator : AbstractValidator<UserLoginInput>
    {
        public UserLoginInputValidator()
        {
            RuleFor(doc => doc.Email).NotNull().OverridePropertyName(UserMessages.EmailRequired);
            RuleFor(doc => doc.Email).EmailAddress().OverridePropertyName(UserMessages.EmailInvalid);
            RuleFor(doc => doc.Email).Length(3, 190).OverridePropertyName(UserMessages.EmailLength);      
            RuleFor(doc => doc.Password).NotNull().OverridePropertyName(UserMessages.PasswordRequired);
            RuleFor(doc => doc.Password).Length(6, 20).OverridePropertyName(UserMessages.PasswordLength);
        }
    }
}
