using Application.AppServices.UserApplication.Validators;
using Application.AppServices.OrderApplication.Validators;
using FluentValidation.Results;
using Infra.CrossCutting.Validators;

namespace Application.AppServices.OrderApplication.Input
    {
    public class WebhookPagSeguroNotificationInput : ValidationInput
        {
        public string NotificationCode { get; set; }
        public string NotificationType { get; set; }
        public string UrlBase { get; set; }
        public string UrlBaseOrderForm { get; set; }
        public string UrlRedirectOrderForm { get; set; }
        public string URLNotifications { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }

        public override bool IsValid()
            {
            ValidationResult = new PagSeguroNotificationInputValidator().Validate(this);
            return ValidationResult.IsValid;
            }

        }

    }
