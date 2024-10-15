using Infra.CrossCutting.Repository;
using System;

namespace Domain.Entities
{
    public class User : BaseEntityDates<int>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Rg { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Password { get; set; }
        public string IsAdmin { get; set; }
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
        public string IuguCustomerId { get; set; }
        public string WebSite { get; set; }
        public string ActArea { get; set; }
        public string Avatar { get; set; }
        public string Company { get; set; }

        public string RDConversionID { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public User()
        {
                
        }

        public User(string name, string email, string cpf, string rg, DateTime? birthDate, string password, string isAdmin, string status, string phoneNumber1, string phoneNumber2, string address, string addressNumber, string neighborhood, string addressComplement, string zipCode, string city, string state, string tokenNotification, string iuguCustomerId, string webSite, string actArea, string avatar, string company, DateTime createdAt, DateTime updatedAt)
        {
            Name = name;
            Email = email;
            Cpf = cpf;
            Rg = rg;
            BirthDate = birthDate;
            Password = password;
            IsAdmin = isAdmin;
            Status = status;
            PhoneNumber1 = phoneNumber1;
            PhoneNumber2 = phoneNumber2;
            Address = address;
            AddressNumber = addressNumber;
            Neighborhood = neighborhood;
            AddressComplement = addressComplement;
            ZipCode = zipCode;
            City = city;
            State = state;
            TokenNotification = tokenNotification;
            IuguCustomerId = iuguCustomerId;
            WebSite = webSite;
            ActArea = actArea;
            Avatar = avatar;
            Company = company;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}
