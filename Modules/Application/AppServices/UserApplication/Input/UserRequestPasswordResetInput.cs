using Application.AppServices.UserApplication.Validators;
using Infra.CrossCutting.Validators;
using System.ComponentModel.DataAnnotations;

namespace Application.AppServices.UserApplication.Input
{
    public class UserRequestPasswordResetInput : ValidationInput
    {
        public string Email { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new UserRequestPasswordResetInputValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
