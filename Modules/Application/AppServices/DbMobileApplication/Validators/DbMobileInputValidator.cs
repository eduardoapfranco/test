using Application.AppServices.DbMobileApplication.Input;
using Domain.Messages;
using FluentValidation;

namespace Application.AppServices.UserApplication.Validators
{
    public class DbMobileInputValidator : AbstractValidator<DbMobileInput>
    {
        public DbMobileInputValidator()
        {
            RuleFor(doc => doc.Secret).NotNull().OverridePropertyName(DbMobileMessages.SecredRequired);
        }
    }
}
