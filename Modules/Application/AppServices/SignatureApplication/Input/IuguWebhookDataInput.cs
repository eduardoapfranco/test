using Application.AppServices.UserApplication.Validators;
using Application.AppServices.OrderApplication.Validators;
using FluentValidation.Results;
using Infra.CrossCutting.Validators;
using Newtonsoft.Json;

namespace Application.AppServices.SignatureApplication.Input
    {
    public class IuguWebhookDataInput : ValidationInput
        {
        public string Id { get; set; }
        [JsonProperty("account_id")]
        [Microsoft.AspNetCore.Mvc.FromForm(Name = "account_id")]
        public string AccountId { get; set; }
        public string Status { get; set; }
        [JsonProperty("subscription_id")]
        [Microsoft.AspNetCore.Mvc.FromForm(Name = "subscription_id")]
        public string SubscriptionId { get; set; }

        //public override bool IsValid()
        //    {
        //    ValidationResult = new PagSeguroNotificationInputValidator().Validate(this);
        //    return ValidationResult.IsValid;
        //    }

        }
    }
