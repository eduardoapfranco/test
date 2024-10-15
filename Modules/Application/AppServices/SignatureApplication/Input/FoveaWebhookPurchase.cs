using Infra.CrossCutting.Validators;
using System;
using Newtonsoft.Json;

namespace Application.AppServices.SignatureApplication.Input
    {
    public class FoveaWebhookPurchase : ValidationInput
        {
        [JsonProperty("productId")]
        public string ProductId { get; set; }
        [JsonProperty("platform")]
        public string Platform { get; set; }
        public bool Sandbox { get; set; }
        [JsonProperty("purchaseId")]
        public string PurchaseId { get; set; }
        [JsonProperty("purchaseDate")]
        public DateTime? PurchaseDate { get; set; }
        [JsonProperty("lastRenewalDate")]
        public DateTime? LastRenewalDate { get; set; }
        [JsonProperty("expirationDate")]
        public string ExpirationDate { get; set; }
        [JsonProperty("cancelationReason")]
        public string CancelationReason { get; set; }
        [JsonProperty("renewalIntent")]
        public string RenewalIntent { get; set; }
        [JsonProperty("renewalIntentChangeDate")]
        public DateTime? RenewalIntentChangeDate { get; set; }
        [JsonProperty("isExpired")]
        public bool IsExpired { get; set; }
        [JsonProperty("isIntroPeriod")]
        public bool IsIntroPeriod { get; set; }
        [JsonProperty("isBillingRetryPeriod")]
        public bool IsBillingRetryPeriod { get; set; }
        [JsonProperty("isTrialPeriod")]
        public bool IsTrialPeriod { get; set; }
        [JsonProperty("discountId")]
        public string DiscountId { get; set; }
        [JsonProperty("priceConsentStatus")]
        public string PriceConsentStatus { get; set; }




        //"cancelationReason": "Customer",
        //"renewalIntent": "Lapse",
        //"isExpired": true

        //public override bool IsValid()
        //    {
        //    ValidationResult = new PagSeguroNotificationInputValidator().Validate(this);
        //    return ValidationResult.IsValid;
        //    }

        }
    }
