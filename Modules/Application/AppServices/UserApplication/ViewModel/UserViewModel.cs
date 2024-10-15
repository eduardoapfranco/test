using Infra.CrossCutting.UoW.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Domain.Entities;
using Domain.Input.Iugu;
using System.Collections.Generic;

namespace Application.AppServices.UserApplication.ViewModel
{
    public class UserViewModel : BaseResult
    { 
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Cpf { get; set; }
        [JsonIgnore]
        public string Rg { get; set; }
        [JsonIgnore]
        public DateTime? BirthDate { get; set; }
        [JsonIgnore]
        public string IsAdmin { get; set; }
        [JsonIgnore]
        public string Status { get; set; }
        public string PhoneNumber1 { get; set; }

        public string PhoneNumber2 { get; set; }

        public string Address { get; set; }

        public string AddressNumber { get; set; }

        public string Neighborhood { get; set; }

        public string AddressComplement { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public string State { get; set; }
        public string TokenNotification { get; set; }
        public string Token { get; set; }
        public string WebSite { get; set; }
        public IEnumerable<Area> Areas { get; set; }
        public string Avatar { get; set; }
        public string Company { get; set; }
        public UserPaymentMethod PaymentDefaultMethod { get; set; }

        public UserViewModel()
            {
            Areas = new List<Area>();
            }
        }
}
