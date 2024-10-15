using Application.AppServices.UserApplication.Validators;
using FluentValidation.Results;
using Infra.CrossCutting.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.AppServices.UserApplication.Input
{
    public class UserInput : ValidationInput
    {
        public long Id { get; set; }      
        public string Name { get; set; }      
        public string Email { get; set; }       
        public string EmailConfirm { get; set; }
        public string IsAdmin { get; set; }       
        public string Password { get; set; }      
        public string PasswordConfirm { get; set; }
        public string Status { get; set; }
        public string PhoneNumber1 { get; set; }       
        public string TokenNotification { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new UserInputValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
