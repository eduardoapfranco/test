using Application.AppServices.UserApplication.Validators;
using FluentValidation.Results;
using Infra.CrossCutting.Validators;

namespace Application.AppServices.UserApplication.Input
{
    public class UserUpdatePasswordInput : ValidationInput
    {
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new UserUpdatePasswordInputValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
