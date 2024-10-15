using Application.AppServices.UserApplication.Input;
using Application.AppServices.UserApplication.ViewModel;
using Domain.Entities;
using FizzWare.NBuilder;
using FluentValidation.Results;
using System.Collections.Generic;

namespace UnitTest.Application.UserApplication.Faker
{
    public static class UserFaker
    {
        public static User CreateUser => Builder<User>.CreateNew().Build();
        public static UserViewModel UserViewModel => Builder<UserViewModel>.CreateNew().Build();
        public static ValidationResult ValidationsResult =>  new ValidationResult(new List<ValidationFailure>() { new ValidationFailure("Nome", "Nome obrigatório"), new ValidationFailure("Email", "Email obrigatório") });

        public static UserInput CreateUserInput()
        {
            var userInput = Builder<UserInput>.CreateNew().Build(); 
            userInput.Email = "teste@teste.com";
            userInput.EmailConfirm = "teste@teste.com";
            userInput.Password = "123456";
            userInput.PasswordConfirm = "123456";
            return userInput;
        }

        public static UserLoginInput CreateUserLoginInput()
        {
            return new UserLoginInput()
            {
                Email = "teste@teste.com",
                Password = "123456"
            };
        }

        public static UserInput CreateUserInputInvalids()
        {
            var userInput = CreateUserInput();
            userInput.Name = null;
            userInput.Email = null;
            userInput.Password = null;
            return userInput;
        }

        public static UserInput CreateUserInputWithPasswordInvalid()
            {
            var userInput = CreateUserInput();
            userInput.Name = "Teste de usuário";
            userInput.Email = "teste@teste.com";
            userInput.EmailConfirm = "teste@teste.com";
            userInput.Password = "123456789012345678900";
            userInput.PasswordConfirm = "123456789012345678900";
            return userInput;
            }
        

        public static UserViewModel CreateUserViewModelInvalids()
        {
            var userViewModel = UserViewModel;
            userViewModel.Name = null;
            userViewModel.Email = null;
            userViewModel.ValidationResult = ValidationsResult;
            return userViewModel;
        }

        public static IEnumerable<User> CreateListUser()
        {
            var list = new List<User>()
            {
                CreateUser
            };
            return list;
        }

        public static IEnumerable<User> CreateListUserLogin()
        {
            var user = CreateUser;
            user.Password = "$2a$11$Hd6sFdBwazhRH3jyr4.e2.vupiPJnFdB6UiNdEC4JfJ5MUneDE1GG";
            var list = new List<User>()
            {
                user
            };
            return list;
        }
    }
}
