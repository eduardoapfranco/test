using Application.AppServices.OrderApplication.Input;
using Domain.Messages;
using FluentValidation;

namespace Application.AppServices.OrderApplication.Validators
    {
    public class PagSeguroNotificationInputValidator : AbstractValidator<WebhookPagSeguroNotificationInput>
        {
            public PagSeguroNotificationInputValidator()
            {
            RuleFor(doc => doc.NotificationCode).NotNull();
            RuleFor(doc => doc.NotificationCode).Length(39, 39);
            RuleFor(doc => doc.NotificationType).NotNull();
            RuleFor(doc => doc.NotificationType).Equals("transaction");
            }
        }
    }
