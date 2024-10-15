using Application.AppServices.RatingApplication.Validators;
using FluentValidation.Results;
using Infra.CrossCutting.Validators;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Application.AppServices.SignatureApplication.Input
    {
    public class SignatureInput : ValidationInput
    {
        [JsonProperty("user_Id")]
        public int UserId { get; set; }
        [JsonProperty("plan_id")]
        public int PlanId { get; set; }
        [JsonProperty("payment_method_token")]
        public string PaymentMethodToken { get; set; }

        public string Flag { get; set; }
        [JsonProperty("last_four_digits")]
        public int LastFourDigits { get; set; }
        public int Type { get; set; }

        public string UrlBase { get; set; }
        public string UrlBaseOrderForm { get; set; }
        public string UrlRedirectOrderForm { get; set; }
        public string URLNotifications { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }

        //public override bool IsValid()
        //{
        //    ValidationResult = new RatingInputValidator().Validate(this);
        //    return ValidationResult.IsValid;
        //}
    }
}
