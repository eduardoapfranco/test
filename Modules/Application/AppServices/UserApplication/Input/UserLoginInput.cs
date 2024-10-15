using Application.AppServices.UserApplication.Validators;
using Infra.CrossCutting.Validators;
using System.ComponentModel.DataAnnotations;

namespace Application.AppServices.UserApplication.Input
{
    public class UserLoginInput : ValidationInput
    {
        public string Email { get; set; }        
        public string Password { get; set; }
        public string TokenNotification { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new UserLoginInputValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
