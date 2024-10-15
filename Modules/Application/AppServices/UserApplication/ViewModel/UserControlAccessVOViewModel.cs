using System;
using Application.AppServices.UserApplication.Mappers;
using Domain.Core;
using Domain.ValueObjects;
using System.Collections.Generic;
using Newtonsoft.Json;
using Domain.Entities;
using Infra.CrossCutting.UoW.Models;
using Application.AppServices.AreaApplication.ViewModel;

namespace Application.AppServices.UserApplication.ViewModel
{
    public class UserControlAccessVOViewModel: BaseResult
    {
        public UserControlAccessVOViewModel()
        {
                
        }

        public UserControlAccessVOViewModel(UserControlAccessVO valueObject, User user, UserPaymentMethod paymentDefaultMethod)
        {
            PlanId = valueObject.Plan.Id;
            Title = valueObject.Plan.Title;
            Content = valueObject.Plan.Content;
            PlanTypeId = valueObject.Plan.PlanType.Id;
            PlanType = valueObject.Plan.PlanType.Title;
            Categories = UserControlAccessCategoriesViewModelMapper.ToViewModel(valueObject);
            Functionalities = UserControlAccessFunctionalitiesViewModelMapper.ToViewModel(valueObject);
            Areas = valueObject.Areas;
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            PaymentDefaultMethod = paymentDefaultMethod;
            PhoneNumber1 = user.PhoneNumber1;
            PhoneNumber2 = user.PhoneNumber2;
            Address = user.Address;
            AddressNumber = user.AddressNumber;
            Neighborhood = user.Neighborhood;
            AddressComplement = user.AddressComplement;
            ZipCode = user.ZipCode;
            City = user.City;
            State = user.State;
            WebSite = user.WebSite;
            Avatar = user.Avatar;
            Company = user.Company;
            StartDatePlan = valueObject.UserPlans.CreatedAt.ToString("dd/MM/yyyy");
            EndDatePlan = valueObject.UserPlans.DueDateAt.ToString("dd/MM/yyyy");
            UserPlan = valueObject.UserPlans;
            DaysToDue = (valueObject.UserPlans.DueDateAt.Date.Subtract(DateTime.Now.Date)).Days;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonProperty("plan_id")]
        public int PlanId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        [JsonProperty("plan_type_id")]
        public int PlanTypeId { get; set; }
        [JsonProperty("plan_type")]
        public string PlanType { get; set; }
        public IEnumerable<UserControlAccessCategoriesViewModel> Categories { get; set; }
        public IEnumerable<UserControlAccessFunctionalitiesViewModel> Functionalities { get; set; }

        public IEnumerable<Area> Areas { get; set; }
        public UserPaymentMethod PaymentDefaultMethod { get; set; }
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
        public string Avatar { get; set; }
        public string Company { get; set; }
        public string StartDatePlan { get; set; }
        public string EndDatePlan { get; set; }
        public UserPlans UserPlan { get; set; }
        public int DaysToDue { get; set; }
    }
}
