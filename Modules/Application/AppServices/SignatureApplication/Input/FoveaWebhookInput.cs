using Infra.CrossCutting.Validators;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Application.AppServices.SignatureApplication.Input
    {
    public class FoveaWebhookInput : ValidationInput
        {
        public string Type { get; set; }
        public string ApplicationUsername { get; set; }
        public string Password { get; set; }
        public Dictionary<string, FoveaWebhookPurchase> Purchases { get; set; }

        //public override bool IsValid()
        //    {
        //    ValidationResult = new PagSeguroNotificationInputValidator().Validate(this);
        //    return ValidationResult.IsValid;
        //    }

        }
    }
