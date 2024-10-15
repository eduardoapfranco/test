using Application.AppServices.UserApplication.Validators;
using Infra.CrossCutting.Validators;

namespace Application.AppServices.DbMobileApplication.Input
{
    public class DbMobileInput : ValidationInput
    {
        public string Secret { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new DbMobileInputValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
