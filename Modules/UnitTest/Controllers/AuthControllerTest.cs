using Application.AppServices.UserApplication.Input;
using Application.AppServices.UserApplication.ViewModel;
using Application.Interfaces;
using ConstruaApp.Api.Controllers;
using Domain.Entities;
using Domain.ValueObjects;
using FizzWare.NBuilder;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using UnitTest.Application.UserApplication.Faker;
using Xunit;

namespace UnitTest.Controllers
{
    public class AuthControllerTest
    {
        private DomainNotificationHandler _notificationHandler;
        private Mock<IUserApplication> _userApplicationMock;
        private Mock<ILogger<AuthController>> _loggerMock;
        private AuthController _controller;

        public AuthControllerTest()
        {
            _userApplicationMock = new Mock<IUserApplication>();
            _notificationHandler = new DomainNotificationHandler();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_notificationHandler, _userApplicationMock.Object, _loggerMock.Object);
        }

        [Fact(DisplayName = "Should return register user with success post async")]
        [Trait("[WebApi.Controllers]-AuthController", "Controllers-PostAsync")]
        public async Task ShouldReturnRegisterUserWithSuccessPostAsync()
        {
            var userViewModel = Builder<UserViewModel>.CreateNew().Build();
            var input = UserFaker.CreateUserInput();

            _userApplicationMock.Setup(x => x.InsertMobileAsync(input)).ReturnsAsync(userViewModel);

            var result = await _controller.PostAsync(input);

            Assert.NotNull(input);
            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<UserViewModel>>(okObjectResult.Value);

            var resultVerify = (Result<UserViewModel>)okObjectResult.Value;

            Assert.NotNull(result);
            Assert.Equal(userViewModel, resultVerify.Data);

            _userApplicationMock.Verify(x => x.InsertMobileAsync(input), Times.Once);
        }

        [Fact(DisplayName = "Should return user login with success login async")]
        [Trait("[WebApi.Controllers]-AuthController", "Controllers-LoginAsync")]
        public async Task ShouldReturnUserLoginWithSuccessLoginAsync()
        {
            var userViewModel = Builder<UserViewModel>.CreateNew().Build();
            var input = UserFaker.CreateUserLoginInput();

            _userApplicationMock.Setup(x => x.LoginAsync(input)).ReturnsAsync(userViewModel);

            var result = await _controller.LoginAsync(input);

            Assert.NotNull(input);
            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<UserViewModel>>(okObjectResult.Value);

            var resultVerify = (Result<UserViewModel>)okObjectResult.Value;

            Assert.NotNull(result);
            Assert.Equal(userViewModel, resultVerify.Data);

            _userApplicationMock.Verify(x => x.LoginAsync(input), Times.Once);
        }

        [Fact(DisplayName = "Should return success when user request password reset")]
        [Trait("[WebApi.Controllers]-AuthController", "Controllers-RequestPasswordResetAsync")]
        public async Task ShouldReturnSuccessWhenUserRequestPasswordReset()
        {
            var userRequestPasswordResetViewModel = Builder<UserRequestPasswordResetViewModel>.CreateNew().Build();
            var input = Builder<UserRequestPasswordResetInput>.CreateNew().Build();

            _userApplicationMock.Setup(x => x.RequestPasswordResetAsync(input)).ReturnsAsync(userRequestPasswordResetViewModel);

            var result = await _controller.RequestPasswordResetAsync(input);

            Assert.NotNull(input);
            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<UserRequestPasswordResetViewModel>>(okObjectResult.Value);

            var resultVerify = (Result<UserRequestPasswordResetViewModel>)okObjectResult.Value;

            Assert.NotNull(result);
            Assert.Equal(userRequestPasswordResetViewModel, resultVerify.Data);

            _userApplicationMock.Verify(x => x.RequestPasswordResetAsync(input), Times.Once);
        }

        [Fact(DisplayName = "Should return success when user reset password")]
        [Trait("[WebApi.Controllers]-AuthController", "Controllers-ResetPasswordAsync")]
        public async Task ShouldReturnSuccessWhenUserResetPassword()
        {
            var userViewModel = Builder<UserViewModel>.CreateNew().Build();
            var input = Builder<UserResetPasswordInput>.CreateNew().Build();

            _userApplicationMock.Setup(x => x.ResetPasswordAsync(input)).ReturnsAsync(userViewModel);

            var result = await _controller.ResetPasswordAsync(input);

            Assert.NotNull(input);
            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<UserViewModel>>(okObjectResult.Value);

            var resultVerify = (Result<UserViewModel>)okObjectResult.Value;

            Assert.NotNull(result);
            Assert.Equal(userViewModel, resultVerify.Data);

            _userApplicationMock.Verify(x => x.ResetPasswordAsync(input), Times.Once);
        }

        [Fact(DisplayName = "Should return success when user get auth profile")]
        [Trait("[WebApi.Controllers]-AuthController", "Controllers-GetControlAccessAsync")]
        public async Task ShouldReturnSuccessWhenUserGetAuthProfile()
        {
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            var plan = Builder<Plan>.CreateNew().Build();
            var user = Builder<User>.CreateNew().Build();
            plan.PlanType = Builder<PlanType>.CreateNew().Build();
            var profile = Builder<Profile>.CreateNew().Build();
            var profileCategories = Builder<Category>.CreateListOfSize(10).Build();
            var profileFunctionalities = Builder<Functionality>.CreateListOfSize(10).Build();
            var userControlAccess = new UserControlAccessVO(userPlan, plan, profile, profileCategories, profileFunctionalities);

            var userViewModel = new UserControlAccessVOViewModel(userControlAccess, user, null);

            _userApplicationMock.Setup(x => x.GetControlAccessAsync(It.IsAny<int>())).ReturnsAsync(userViewModel);

            var result = await _controller.GetControlAccessAsync();

            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<UserControlAccessVOViewModel>>(okObjectResult.Value);

            var resultVerify = (Result<UserControlAccessVOViewModel>)okObjectResult.Value;

            Assert.NotNull(result);
            Assert.Equal(userViewModel, resultVerify.Data);

            _userApplicationMock.Verify(x => x.GetControlAccessAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Should return internal server error when user get auth profile")]
        [Trait("[WebApi.Controllers]-AuthController", "Controllers-GetControlAccessAsync")]
        public async Task ShouldReturnInternalServerErrorWhenUserGetAuthProfile()
        {
            _userApplicationMock.Setup(x => x.GetControlAccessAsync(It.IsAny<int>())).ThrowsAsync(new Exception("teste"));

            var result = await _controller.GetControlAccessAsync();

            Assert.IsType<ObjectResult>(result);

            var objectResult = (ObjectResult)result;

            Assert.Equal(500, objectResult.StatusCode);
            Assert.NotNull(result);
            _userApplicationMock.Verify(x => x.GetControlAccessAsync(It.IsAny<int>()), Times.Once);
        }
    }
}
