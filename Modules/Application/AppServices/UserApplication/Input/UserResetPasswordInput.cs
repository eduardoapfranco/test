using Application.AppServices.UserApplication.Validators;
using FluentValidation.Results;
using Infra.CrossCutting.Validators;

namespace Application.AppServices.UserApplication.Input
{
    public class UserResetPasswordInput : ValidationInput
    {
        public string Email { get; set; }
        public int CheckerNumber { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new UserResetPasswordInputValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
