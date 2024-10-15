using Application.AppServices.UserApplication.Validators;
using FluentValidation.Results;
using Infra.CrossCutting.Validators;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Application.AppServices.UserApplication.Input
{
    public class UserUpdateInput : ValidationInput
        {
        public string Name { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public string Address { get; set; }
        public string AddressNumber { get; set; }
        public string Neighborhood { get; set; }
        public string AddressComplement { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string WebSite { get; set; }
        public string ActArea { get; set; }
        public string Avatar { get; set; }
        public string Company { get; set; }
        [JsonProperty("areas_ids")]
        public int[] AreasIds { get; set; }

        public UserUpdateInput()
            {
            AreasIds = new int[] { };
            }

        public override bool IsValid()
        {
            ValidationResult = new UserUpdateInputValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
