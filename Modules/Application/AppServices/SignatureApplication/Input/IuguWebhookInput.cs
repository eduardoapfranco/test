using Infra.CrossCutting.Validators;

namespace Application.AppServices.SignatureApplication.Input
    {
    public class IuguWebhookInput : ValidationInput
        {
        public string Event { get; set; }
        public IuguWebhookDataInput Data { get; set; }

        //public override bool IsValid()
        //    {
        //    ValidationResult = new PagSeguroNotificationInputValidator().Validate(this);
        //    return ValidationResult.IsValid;
        //    }

        }

    }
