using Application.AppServices.RatingApplication.Input;
using Domain.Messages;
using FluentValidation;

namespace Application.AppServices.RatingApplication.Validators
{
    public class RatingInputValidator : AbstractValidator<RatingInput>
    {
        public RatingInputValidator()
        {
            RuleFor(doc => doc.UserId).NotNull();
            RuleFor(doc => doc.Comment).Length(3,190);
            RuleFor(doc => doc.Title).Length(3, 190);
            RuleFor(doc => doc.ChecklistId).NotEmpty().Unless(doc => doc.CategoryId.HasValue);
            RuleFor(doc => doc.CategoryId).NotEmpty().Unless(doc => doc.ChecklistId.HasValue);
            }
    }
}
