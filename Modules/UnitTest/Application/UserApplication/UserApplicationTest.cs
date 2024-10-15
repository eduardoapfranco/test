using System;
using Application.AppServices.UserApplication.ViewModel;
using AutoMapper;
using Domain.Core;
using Domain.Entities;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Auth.Intefaces;
using Infra.CrossCutting.Email.Interfaces;
using Infra.CrossCutting.Notification.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.AppServices.UserApplication.Input;
using AutoFixture;
using AutoFixture.AutoMoq;
using Domain.Input;
using FizzWare.NBuilder;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using UnitTest.Application.UserApplication.Faker;
using Xunit;
using App = Application.AppServices;
using Domain.ValueObjects;
using ProfileDomain = Domain.Entities.Profile;

namespace UnitTest.Application.UserApplication
{
    public class UserApplicationTest
    {
        private Mock<IUserDomainService> _userDomainServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IUserPaymentMethodDomainService> _userPaymentMethodDomainServiceMock;
        private Mock<IEmailSendService> _emailSendServiceMock;
        //private Mock<IAuthService<User>> _authServiceMock;
        private Mock<ILogger<App.UserApplication.UserApplication>> _loggerMock;
        private App.UserApplication.UserApplication _userApplication;
        private AuthService _authService;
        private readonly Fixture _fixture;

        public UserApplicationTest()
        {
            // configure
            _userDomainServiceMock = new Mock<IUserDomainService>();
            _mapperMock = new Mock<IMapper>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _emailSendServiceMock = new Mock<IEmailSendService>();
            _loggerMock = new Mock<ILogger<App.UserApplication.UserApplication>>();
            _authService = new AuthService();
            _userPaymentMethodDomainServiceMock = new Mock<IUserPaymentMethodDomainService>();

            //_authServiceMock = new Mock<IAuthService<User>>();
            _userApplication = new App.UserApplication.UserApplication(_userDomainServiceMock.Object, _smartNotificationMock.Object, _mapperMock.Object, _emailSendServiceMock.Object, _authService, _loggerMock.Object, _userPaymentMethodDomainServiceMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }

        [Fact(DisplayName = "Shoud return fields invalids insert mobile async")]
        [Trait("[Application.AppServices]-UserApplication", "Application-InsertMobileAsync")]
        public async Task ShouldReturnFieldsInvalidsInsertMobileAsync()
        {
            // arrange
            var userInput = UserFaker.CreateUserInputInvalids();           
            var user = UserFaker.CreateUser;           
            var userViewModel = UserFaker.CreateUserViewModelInvalids();          
            _mapperMock.Setup(x => x.Map<UserViewModel>(userInput)).Returns(userViewModel);

            // act
            var result = await _userApplication.InsertMobileAsync(userInput);

            // assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "Shoud return user view model with success after insert mobile async")]
        [Trait("[Application.AppServices]-UserApplication", "Application-InsertMobileAsync")]
        public async Task ShouldReturnUserViewModelWithSuccessAfterInsertMobileAsync()
        {
            // arrange
            var userInput = UserFaker.CreateUserInput();
            userInput.Email = "construaapp@gmail.com";
            userInput.EmailConfirm = "construaapp@gmail.com";
            var user = UserFaker.CreateUser;
            user.Email = "construaapp@gmail.com";
            var userViewModel = UserFaker.UserViewModel;
            userViewModel.Email = "construaapp@gmail.com";
            _mapperMock.Setup(x => x.Map<UserViewModel>(userInput)).Returns(userViewModel);
            _mapperMock.Setup(x => x.Map<UserViewModel>(user)).Returns(userViewModel);
            _mapperMock.Setup(x => x.Map<User>(userInput)).Returns(user);
            _userDomainServiceMock.Setup(x => x.InsertAsync(user)).ReturnsAsync(user);

            // act
            var result = await _userApplication.InsertMobileAsync(userInput);

            // assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            _userDomainServiceMock.Verify(x => x.InsertAsync(user), Times.Once);
        }


        [Fact(DisplayName = "Shoud return null when email exists in database insert mobile async")]
        [Trait("[Application.AppServices]-UserApplication", "Application-InsertMobileAsync")]
        public async Task ShouldReturnNullWhenEmailExistsInDatabaseInsertMobileAsync()
        {
            // arrange
            var userInput = UserFaker.CreateUserInput();
            var user = UserFaker.CreateUser;
            var userViewModel = UserFaker.UserViewModel;
            var userList = UserFaker.CreateListUser();
            _mapperMock.Setup(x => x.Map<UserViewModel>(userInput)).Returns(userViewModel);
            _mapperMock.Setup(x => x.Map<UserViewModel>(user)).Returns(userViewModel);
            _mapperMock.Setup(x => x.Map<User>(userInput)).Returns(user);
            _userDomainServiceMock.Setup(x => x.InsertAsync(user)).ReturnsAsync(user);
            _userDomainServiceMock.Setup(x => x.SelectFilterAsync(x => x.Email.Equals(userInput.Email))).ReturnsAsync(userList);

            // act
            var result = await _userApplication.InsertMobileAsync(userInput);

            // assert
            Assert.Null(result);
            _userDomainServiceMock.Verify(x => x.SelectFilterAsync(x => x.Email.Equals(userInput.Email)), Times.Once);
        }

        [Fact(DisplayName = "Shoud return null when email not exists in database login async")]
        [Trait("[Application.AppServices]-UserApplication", "Application-LoginAsync")]
        public async Task ShouldReturnNullWhenEmailNotExistsInDatabaseLoginAsync()
        {
            // arrange
            var userLoginInput = UserFaker.CreateUserLoginInput();
            var user = UserFaker.CreateUser;
            var userViewModel = UserFaker.UserViewModel;
            var userList = new List<User>();
            _mapperMock.Setup(x => x.Map<UserViewModel>(user)).Returns(userViewModel);
            _userDomainServiceMock.Setup(x => x.SelectFilterAsync(x => x.Email.Equals(userLoginInput.Email))).ReturnsAsync(userList);

            // act
            var result = await _userApplication.LoginAsync(userLoginInput);

            // assert
            Assert.Null(result);
            _userDomainServiceMock.Verify(x => x.SelectFilterAsync(x => x.Email.Equals(userLoginInput.Email)), Times.Once);
        }

        [Fact(DisplayName = "Shoud return user view model when email and password valid login async")]
        [Trait("[Application.AppServices]-UserApplication", "Application-LoginAsync")]
        public async Task ShouldReturnUserViewModelWhenEmailAndPasswordValidLoginAsync()
        {
            // arrange
            var userLoginInput = UserFaker.CreateUserLoginInput();
            var user = UserFaker.CreateUser;
            var userViewModel = UserFaker.UserViewModel;
            var userList = UserFaker.CreateListUserLogin();
            _mapperMock.Setup(x => x.Map<UserViewModel>(userList.FirstOrDefault())).Returns(userViewModel);
            //_authServiceMock.Setup(x => x.GenerateToken(user)).Returns("fedaf7d8863b48e197b9287d492b708e");
            _userDomainServiceMock.Setup(x => x.SelectFilterAsync(x => x.Email.Equals(userLoginInput.Email))).ReturnsAsync(userList);

            // act
            var result = await _userApplication.LoginAsync(userLoginInput);

            // assert
            Assert.IsType<UserViewModel>(result);
            _userDomainServiceMock.Verify(x => x.SelectFilterAsync(x => x.Email.Equals(userLoginInput.Email)), Times.Once);
        }

        [Fact(DisplayName = "Should return null when email and password invalid login async")]
        [Trait("[Application.AppServices]-UserApplication", "Application-LoginAsync")]
        public async Task ShouldReturnNullWhenEmailAndPasswordInvalidLoginAsync()
        {
            // arrange
            var userLoginInput = UserFaker.CreateUserLoginInput();
            userLoginInput.Password = "111111";
            var user = UserFaker.CreateUser;
            var userViewModel = UserFaker.UserViewModel;
            var userList = UserFaker.CreateListUserLogin();
            _mapperMock.Setup(x => x.Map<UserViewModel>(userList.FirstOrDefault())).Returns(userViewModel);
            _userDomainServiceMock.Setup(x => x.SelectFilterAsync(x => x.Email.Equals(userLoginInput.Email))).ReturnsAsync(userList);

            // act
            var result = await _userApplication.LoginAsync(userLoginInput);

            // assert
            Assert.Null(result);
            _userDomainServiceMock.Verify(x => x.SelectFilterAsync(x => x.Email.Equals(userLoginInput.Email)), Times.Once);
        }

        [Fact(DisplayName = "Should return null when request password reset invalid params")]
        [Trait("[Application.AppServices]-UserApplication", "Application-RequestPasswordResetAsync")]
        public async Task ShouldReturnNullWhenRequestPasswordResetInvalidParams()
        {
            // arrange
            var input = Builder<UserRequestPasswordResetInput>.CreateNew().Build();
            input.Email = null;

            // act
            var result = await _userApplication.RequestPasswordResetAsync(input);

            // assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "Should return success request password reset")]
        [Trait("[Application.AppServices]-UserApplication", "Application-RequestPasswordResetAsync")]
        public async Task ShouldReturnSuccessRequestPasswordReset()
        {
            // arrange
            var input = Builder<UserRequestPasswordResetInput>.CreateNew().Build();
            input.Email = "teste@teste.com";
            _userDomainServiceMock.Setup(x => x.RequestPasswordResetAsync(It.IsAny<RequestPasswordResetInput>())).ReturnsAsync(123456);

            // act
            var result = await _userApplication.RequestPasswordResetAsync(input);

            // assert
            Assert.NotNull(result);
            Assert.IsType<UserRequestPasswordResetViewModel>(result);
        }

        [Fact(DisplayName = "Should return null when request password reset when dont generate checker number")]
        [Trait("[Application.AppServices]-UserApplication", "Application-RequestPasswordResetAsync")]
        public async Task ShouldReturnNullRequestPasswordResetWhenDontGenerateCheckerNumber()
        {
            // arrange
            var input = Builder<UserRequestPasswordResetInput>.CreateNew().Build();
            input.Email = "teste@teste.com";
            _userDomainServiceMock.Setup(x => x.RequestPasswordResetAsync(It.IsAny<RequestPasswordResetInput>())).ReturnsAsync(0);

            // act
            var result = await _userApplication.RequestPasswordResetAsync(input);

            // assert
            Assert.Null(result);
        }


        [Fact(DisplayName = "Should return null when reset password invalid params")]
        [Trait("[Application.AppServices]-UserApplication", "Application-ResetPasswordAsync")]
        public async Task ShouldReturnNullWhenResetPasswordInvalidParams()
        {
            // arrange
            var input = _fixture.Build<UserResetPasswordInput>().With(x => x.ValidationResult, new ValidationResult()).Create();
            input.Email = null;

            // act
            var result = await _userApplication.ResetPasswordAsync(input);

            // assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "Should return null when reset password fail")]
        [Trait("[Application.AppServices]-UserApplication", "Application-ResetPasswordAsync")]
        public async Task ShouldReturnNullWhenResetPasswordFail()
        { 
            // arrange
            var input = _fixture.Build<UserResetPasswordInput>().With(x => x.ValidationResult, new ValidationResult()).Create();
            _userDomainServiceMock.Setup(x => x.ResetPasswordAsync(It.IsAny<ResetPasswordInput>())).ReturnsAsync(false);
            input.Email = "teste@teste.com";
            input.Password = "123456";
            input.PasswordConfirm = "123456";

            // act
            var result = await _userApplication.ResetPasswordAsync(input);

            // assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "Should return user view model when reset password successfully")]
        [Trait("[Application.AppServices]-UserApplication", "Application-ResetPasswordAsync")]
        public async Task ShouldReturnUserViewModelWhenResetPasswordSuccessfully()
        {
            // arrange
            var input = _fixture.Build<UserResetPasswordInput>().With(x => x.ValidationResult, new ValidationResult()).Create();
            _userDomainServiceMock.Setup(x => x.ResetPasswordAsync(It.IsAny<ResetPasswordInput>())).ReturnsAsync(true);
            input.Email = "teste@teste.com";
            input.Password = "123456";
            input.PasswordConfirm = "123456";
            var userLoginInput = UserFaker.CreateUserLoginInput();
            var userViewModel = UserFaker.UserViewModel;
            var userList = UserFaker.CreateListUserLogin();
            _mapperMock.Setup(x => x.Map<UserViewModel>(userList.FirstOrDefault())).Returns(userViewModel);
            _userDomainServiceMock.Setup(x => x.SelectFilterAsync( It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(userList);

            // act
            var result = await _userApplication.ResetPasswordAsync(input);

            // assert
            Assert.IsType<UserViewModel>(result);
            Assert.NotNull(result);
            _userDomainServiceMock.Verify(x => x.ResetPasswordAsync(It.IsAny<ResetPasswordInput>()), Times.Once);
        }

        [Fact(DisplayName = "Shoud return control access for user logged")]
        [Trait("[Domain.AppServices]-UserApplication", "Application-GetControlAccessAsync")]
        public async Task ShouldReturnControlAccessForUserLogged()
        {
            //arrange
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            var user = Builder<User>.CreateNew().Build();
            var plan = Builder<Plan>.CreateNew().Build();
            plan.PlanType = Builder<PlanType>.CreateNew().Build();
            var profile = Builder<ProfileDomain>.CreateNew().Build();
            var profileCategories = Builder<Category>.CreateListOfSize(10).Build();
            var profileFunctionalities = Builder<Functionality>.CreateListOfSize(10).Build();
            var userControlAccess = new UserControlAccessVO(userPlan, plan, profile, profileCategories, profileFunctionalities, null);
            _userDomainServiceMock.Setup(x => x.GetControlAccessAsync(1)).ReturnsAsync(userControlAccess);
            _userDomainServiceMock.Setup(x => x.SelectByIdAsync(It.IsAny<int>())).ReturnsAsync(user);

            //act
            var result = await _userApplication.GetControlAccessAsync(1);

            //assert
            Assert.NotNull(result);
            Assert.IsType<UserControlAccessVOViewModel>(result);
        }

        [Fact(DisplayName = "Should return user view model when update password successfully")]
        [Trait("[Application.AppServices]-UserApplication", "Application-UpdatePasswordAsync")]
        public async Task ShouldReturnUserViewModelWhenUpdatePasswordSuccessfully()
            {
            // arrange
            var user = UserFaker.CreateUser;
            var input = _fixture.Build<UserUpdatePasswordInput>().With(x => x.ValidationResult, new ValidationResult()).Create();
            user.Email = "teste@teste.com";
            input.Password = "123456";
            input.PasswordConfirm = "123456";
            var userViewModel = UserFaker.UserViewModel;
            var userList = UserFaker.CreateListUserLogin();

            _userDomainServiceMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);
            _userDomainServiceMock.Setup(x => x.SelectByIdAsync(It.IsAny<int>())).ReturnsAsync(user);
            _mapperMock.Setup(x => x.Map<UserViewModel>(userList.FirstOrDefault())).Returns(userViewModel);
            _userDomainServiceMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(userList);

            // act
            var result = await _userApplication.UpdatePasswordAsync(user.Id, input);

            // assert
            Assert.IsType<UserViewModel>(result);
            Assert.NotNull(result);
            _userDomainServiceMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Exactly(2));
            }


        [Fact(DisplayName = "Should return null with invalid input when update password async")]
        [Trait("[Application.AppServices]-UserApplication", "Application-UpdatePasswordAsync")]
        public async Task ShouldReturnNullWithInvalidInputWhenUpdatePassword()
            {
            // arrange
            var user = UserFaker.CreateUser;
            var input = _fixture.Build<UserUpdatePasswordInput>().With(x => x.ValidationResult, new ValidationResult()).Create();
            user.Email = "teste@teste.com";
            input.PasswordConfirm = "123456";
            var userViewModel = UserFaker.UserViewModel;
            var userList = UserFaker.CreateListUserLogin();

            _userDomainServiceMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);
            _userDomainServiceMock.Setup(x => x.SelectByIdAsync(It.IsAny<int>())).ReturnsAsync(user);
            _mapperMock.Setup(x => x.Map<UserViewModel>(userList.FirstOrDefault())).Returns(userViewModel);
            _userDomainServiceMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(userList);

            // act
            var result = await _userApplication.UpdatePasswordAsync(user.Id, input);

            // assert
            Assert.Null(result);
            }

        [Fact(DisplayName = "Should return null when can't update user in update password async")]
        [Trait("[Application.AppServices]-UserApplication", "Application-UpdatePasswordAsync")]
        public async Task ShouldReturnNullWhenCantUpdateUserInUpdatePassword()
            {
            // arrange
            var user = UserFaker.CreateUser;
            var input = _fixture.Build<UserUpdatePasswordInput>().With(x => x.ValidationResult, new ValidationResult()).Create();
            user.Email = "teste@teste.com";
            input.PasswordConfirm = "123456";
            var userViewModel = UserFaker.UserViewModel;
            var userList = UserFaker.CreateListUserLogin();

            _userDomainServiceMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User)null);
            _userDomainServiceMock.Setup(x => x.SelectByIdAsync(It.IsAny<int>())).ReturnsAsync(user);
            _mapperMock.Setup(x => x.Map<UserViewModel>(userList.FirstOrDefault())).Returns(userViewModel);
            _userDomainServiceMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(userList);

            // act
            var result = await _userApplication.UpdatePasswordAsync(user.Id, input);

            // assert
            Assert.Null(result);
            }
        }
}
