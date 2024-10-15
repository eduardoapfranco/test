using Application.AppServices.UserApplication.Input;
using Application.AppServices.UserApplication.ViewModel;
using Bogus;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Controllers;
using Infra.Data.Context;
using Infra.Data.Repository;
using IntegrationTest.Config;
using IntegrationTest.Scenarios.Auth.Faker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest.Scenarios.Auth
{
    public class AuthControllerIntegrationTest
    {
        private readonly TestContext _testContext;


        public AuthControllerIntegrationTest()
        {
            _testContext = new TestContext();
        }

        [Fact(DisplayName = "Should return success when user login")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnSuccessWheUserLogin()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/login"
            }; 

            var input = new UserLoginInput()
            {
                Email = AuthLogin.Email,
                Password = AuthLogin.Password
            };

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);          

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.IsType<UserViewModel>(result.Data);
            Assert.NotNull(result.Data.Token);
            Assert.False(result.Error);
            Assert.Equal("construaapp@gmail.com", result.Data.Email);
        }

        [Fact(DisplayName = "Should return invalid credential when user login")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnInvalidCredentialUserLogin()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/login"
            };

            var input = new UserLoginInput()
            {
                Email = AuthLogin.Email,
                Password = "1234567"
            };

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Null(result.Data);
            Assert.True(result.Error);
            Assert.Contains("E-mail e/ou senha inválidos.", result.Messages[0]);
        }

        [Fact(DisplayName = "Should return invalid required fields when user login")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnInvalidRequiredFieldsUserLogin()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/login"
            };

            var input = new UserLoginInput()
            {
                Email = "string",
                Password = "string"
            };

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Null(result.Data);
            Assert.True(result.Error);
        }

        [Fact(DisplayName = "Should return success when user register")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnSuccessWheUserRegister()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/register"
            };

            var input = new Faker<UserInput>("pt_BR")
                .RuleFor(c => c.Name, (f, c) => f.Internet.UserName(c.Name))
                .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.Name))                
                .Generate();

            input.EmailConfirm = input.Email;
            input.Password = "123456";
            input.PasswordConfirm = input.Password;

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.IsType<UserViewModel>(result.Data);
            Assert.NotNull(result.Data.Token);
            Assert.False(result.Error);
            Assert.Equal(input.Email, result.Data.Email);
        }

        [Fact(DisplayName = "Should return fail when user exists register")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnFailWheUserExistsRegister()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/register"
            };

            var input = new UserInput();
            input.Name = "Rafael Teste";
            input.Email = AuthLogin.Email;
            input.EmailConfirm = input.Email;
            input.Password = "123456";
            input.PasswordConfirm = input.Password;

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result.Error);
            Assert.Contains("O e-mail 'construaapp@gmail.com' já está cadastrado em nosso sistema.", result.Messages[0]);
        }

        [Fact(DisplayName = "Should return invalid fields user register")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldInvalidFieldsUserRegister()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/register"
            };

            var input = new UserInput();
            input.Name = "";
            input.Email = "rafael.apfsan";
            input.EmailConfirm = input.Email;
            input.Password = "123456";
            input.PasswordConfirm = input.Password;

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result.Error);
        }

        [Fact(DisplayName = "Should return invalid fields request password reset")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnInvalidFieldsRequestPasswordReset()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/request-reset-password"
            };

            var input = new UserRequestPasswordResetInput();
            input.Email = "rafael.apfsan";

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserRequestPasswordResetViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result.Error);
        }

        [Fact(DisplayName = "Should return error when user doesent exists request password reset")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnErrorWhenUserDoesntExistsRequestPasswordReset()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/request-reset-password"
            };

            var input = new UserRequestPasswordResetInput();
            input.Email = "rafael.apfsan@gmail.com";

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserRequestPasswordResetViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result.Error);
        }

        [Fact(DisplayName = "Should return success when user exists request password reset")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnSuccessWhenUserExistsRequestPasswordReset()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/request-reset-password"
            };

            var input = new UserRequestPasswordResetInput();
            input.Email = "construaapp@gmail.com";

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserRequestPasswordResetViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
        }

        [Fact(DisplayName = "Should return fail when user e-mail not found reset passowrd")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnFailWheUserEmailNotFoundResetPassword()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/reset-password"
            };

            var input = new UserResetPasswordInput();
            input.Email = "rafael@rafaeleeee.com";
            input.Password = "123456";
            input.PasswordConfirm = "123456";
            input.CheckerNumber = 123456;

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result.Error);
            Assert.Contains("O e-mail 'rafael@rafaeleeee.com' não foi encontrado em nosso sistema.", result.Messages[0]);
        }

        [Fact(DisplayName = "Should return fail when user checker number invalid reset passowrd")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnFailWheUserCheckerNumberInvalidResetPassword()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/reset-password"
            };

            var input = new UserResetPasswordInput();
            input.Email = AuthLogin.Email;
            input.Password = AuthLogin.Password;
            input.PasswordConfirm = AuthLogin.Password;
            input.CheckerNumber = 123456;

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result.Error);
            Assert.Contains("O código de verificação é inválido.", result.Messages[0]);
        }

        [Fact(DisplayName = "Should return success when user reset password")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnSuccessWhenUserResetPassword()
        {
            // arrange
            var requestChekerNumber = new
            {
                Url = "/api/v1/request-reset-password"
            };
            var inputCheckerNumber = new UserRequestPasswordResetInput();
            inputCheckerNumber.Email = "construaapp@gmail.com";
            await _testContext.Client.PostAsync(requestChekerNumber.Url, ContentHelper<object>.FormatStringContent(inputCheckerNumber));
             

            var request = new
            {
                Url = "/api/v1/reset-password"
            };

            var input = new UserResetPasswordInput();
            input.Email = AuthLogin.Email;
            input.Password = AuthLogin.Password;
            input.PasswordConfirm = AuthLogin.Password;
            IServiceCollection services = new ServiceCollection();

            var configuration = ConnectionString.GetConnection();

            // Add a database context (AppDbContext) using an in-memory database for testing.
            services.AddDbContext<MySQLCoreContext>(options => options.UseMySql(configuration["ConnectionStrings:MySqlCore"]));
            services.AddSingleton<IPasswordResetMobileRepository, PasswordResetMobileRepository>();
            services.AddLogging();
            using (ServiceProvider serviceProvider = services.AddEntityFrameworkMySql().BuildServiceProvider())
            {
                var passwordResetMobileRepository = serviceProvider.GetRequiredService<IPasswordResetMobileRepository>();

                var getCheckNumber = await passwordResetMobileRepository.SelectFilterAsync(x =>
                    x.UserEmail.Equals(input.Email) && x.Active.Equals((byte)PasswordResetMobileEnum.YES) &&
                    x.Used.Equals((byte)PasswordResetMobileEnum.NO));

                    input.CheckerNumber = getCheckNumber.OrderByDescending(x => x.Id).FirstOrDefault().CheckerNumber;
            }

            // act
            var response = await _testContext.Client.PostAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<UserViewModel>>(result);
            Assert.NotNull(result.Data.Token);
        }

        [Fact(DisplayName = "Should return the quarterly premium plan when the user has an open plan")]
        [Trait("[IntegrationTest]-AuthController", "GetControlAccessAsync")]
        public async Task ShouldReturnTheQuarterlyPremiumPlanWhenTheUserHasOpenPlan()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/auth/profile"
            };

            var userConstrua = await ClearUserPlansForConstruaAppGmail();
            await InsertUserPlan(UserPlanFaker.CreateUserPlanFreemium(userConstrua.Id));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_MENSAL, userConstrua.Id, DateTime.Now.AddMonths(-2), DateTime.Now.AddMonths(-1), (sbyte)BoolEnum.YES));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_TRIMESTRAL, userConstrua.Id, DateTime.Now, DateTime.Now.AddMonths(3), (sbyte)BoolEnum.YES));

            var user = await AuthLogin.GetUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);

            // act
            var response = await _testContext.Client.GetAsync($"{request.Url}");
            var result = await ContentHelper<UserControlAccessVOViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal((int)PlanWithTypeEnum.PREMIUM_TRIMESTRAL, result.Data.PlanId);
            Assert.Equal(user.Id, result.Data.Id);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Should return the Semiannual premium plan when the user has an open plan")]
        [Trait("[IntegrationTest]-AuthController", "GetControlAccessAsync")]
        public async Task ShouldReturnTheSemiannualPremiumPlanWhenTheUserHasOpenPlan()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/auth/profile"
            };

            var userConstrua = await ClearUserPlansForConstruaAppGmail();
            await InsertUserPlan(UserPlanFaker.CreateUserPlanFreemium(userConstrua.Id));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_MENSAL, userConstrua.Id, DateTime.Now.AddMonths(-5), DateTime.Now.AddMonths(-4), (sbyte)BoolEnum.YES));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_TRIMESTRAL, userConstrua.Id, DateTime.Now.AddMonths(-3), DateTime.Now, (sbyte)BoolEnum.YES));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_SEMESTRAL, userConstrua.Id, DateTime.Now, DateTime.Now.AddMonths(6), (sbyte)BoolEnum.YES));

            var user = await AuthLogin.GetUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);

            // act
            var response = await _testContext.Client.GetAsync($"{request.Url}");
            var result = await ContentHelper<UserControlAccessVOViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal((int)PlanWithTypeEnum.PREMIUM_SEMESTRAL, result.Data.PlanId);
            Assert.Equal(user.Id, result.Data.Id);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Should return the yearly premium plan when the user has an open plan")]
        [Trait("[IntegrationTest]-AuthController", "GetControlAccessAsync")]
        public async Task ShouldReturnTheYearlyPremiumPlanWhenTheUserHasOpenPlan()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/auth/profile"
            };

            var userConstrua = await ClearUserPlansForConstruaAppGmail();
            await InsertUserPlan(UserPlanFaker.CreateUserPlanFreemium(userConstrua.Id));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_MENSAL, userConstrua.Id, DateTime.Now.AddMonths(-12), DateTime.Now.AddMonths(-11), (sbyte)BoolEnum.YES));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_TRIMESTRAL, userConstrua.Id, DateTime.Now.AddMonths(-10), DateTime.Now.AddMonths(-7), (sbyte)BoolEnum.YES));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_SEMESTRAL, userConstrua.Id, DateTime.Now.AddMonths(-6), DateTime.Now, (sbyte)BoolEnum.YES));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_ANUAL, userConstrua.Id, DateTime.Now, DateTime.Now.AddMonths(12), (sbyte)BoolEnum.YES));

            var user = await AuthLogin.GetUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);

            // act
            var response = await _testContext.Client.GetAsync($"{request.Url}");
            var result = await ContentHelper<UserControlAccessVOViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal((int)PlanWithTypeEnum.PREMIUM_ANUAL, result.Data.PlanId);
            Assert.Equal(user.Id, result.Data.Id);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Should return the monthly premium plan when the user has an open plan even with trial plan active")]
        [Trait("[IntegrationTest]-AuthController", "GetControlAccessAsync")]
        public async Task ShouldReturnTheMonthlyPremiumPlanWhenTheUserHasOpenPlanEventWithTrialPlanActive()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/auth/profile"
                };

            var userConstrua = await ClearUserPlansForConstruaAppGmail();
            await InsertUserPlan(UserPlanFaker.CreateUserPlanFreemium(userConstrua.Id));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_MENSAL, userConstrua.Id, DateTime.Now, DateTime.Now.AddMonths(1), (sbyte)BoolEnum.YES));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_TRIAL, userConstrua.Id, DateTime.Now, DateTime.Now.AddDays(15), (sbyte)BoolEnum.YES));

            var user = await AuthLogin.GetUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);

            // act
            var response = await _testContext.Client.GetAsync($"{request.Url}");
            var result = await ContentHelper<UserControlAccessVOViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal((int)PlanWithTypeEnum.PREMIUM_MENSAL, result.Data.PlanId);
            Assert.Equal(user.Id, result.Data.Id);
            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Should return the monthly premium plan when the user has an open plan")]
        [Trait("[IntegrationTest]-AuthController", "GetControlAccessAsync")]
        public async Task ShouldReturnTheMonthlyPremiumPlanWhenTheUserHasOpenPlan()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/auth/profile"
            };

            var userConstrua = await ClearUserPlansForConstruaAppGmail();
            await InsertUserPlan(UserPlanFaker.CreateUserPlanFreemium(userConstrua.Id));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_MENSAL, userConstrua.Id, DateTime.Now, DateTime.Now.AddMonths(1), (sbyte)BoolEnum.YES));

            var user = await AuthLogin.GetUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);

            // act
            var response = await _testContext.Client.GetAsync($"{request.Url}");
            var result = await ContentHelper<UserControlAccessVOViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal((int)PlanWithTypeEnum.PREMIUM_MENSAL, result.Data.PlanId);
            Assert.Equal(user.Id, result.Data.Id);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Should return the trial premium plan when the user has an open plan")]
        [Trait("[IntegrationTest]-AuthController", "GetControlAccessAsync")]
        public async Task ShouldReturnTheTrialPremiumPlanWhenTheUserHasOpenPlan()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/auth/profile"
                };

            var userConstrua = await ClearUserPlansForConstruaAppGmail();
            await InsertUserPlan(UserPlanFaker.CreateUserPlanFreemium(userConstrua.Id));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_TRIAL, userConstrua.Id, DateTime.Now, DateTime.Now.AddMonths(1), (sbyte)BoolEnum.YES));

            var user = await AuthLogin.GetUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);

            // act
            var response = await _testContext.Client.GetAsync($"{request.Url}");
            var result = await ContentHelper<UserControlAccessVOViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal((int)PlanWithTypeEnum.PREMIUM_TRIAL, result.Data.PlanId);
            Assert.Equal(user.Id, result.Data.Id);
            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Should return freemium plan when the user has an closed all plans preemium")]
        [Trait("[IntegrationTest]-AuthController", "GetControlAccessAsync")]
        public async Task ShouldReturnFreemiumPlanWhenTheUserHasClosedAllPlansPreemium()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/auth/profile"
            };

            var userConstrua = await ClearUserPlansForConstruaAppGmail();
            await InsertUserPlan(UserPlanFaker.CreateUserPlanFreemium(userConstrua.Id));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_MENSAL, userConstrua.Id, DateTime.Now.AddMonths(-30), DateTime.Now.AddMonths(-29), (sbyte)BoolEnum.YES));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_TRIMESTRAL, userConstrua.Id, DateTime.Now.AddMonths(-28), DateTime.Now.AddMonths(-25), (sbyte)BoolEnum.YES));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_SEMESTRAL, userConstrua.Id, DateTime.Now.AddMonths(-24), DateTime.Now.AddMonths(-19), (sbyte)BoolEnum.YES));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_ANUAL, userConstrua.Id, DateTime.Now.AddMonths(-18), DateTime.Now.AddMonths(-6), (sbyte)BoolEnum.YES));

            var user = await AuthLogin.GetUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);

            // act
            var response = await _testContext.Client.GetAsync($"{request.Url}");
            var result = await ContentHelper<UserControlAccessVOViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal((int)PlanWithTypeEnum.FREEMIUM_MENSAL, result.Data.PlanId);
            Assert.Equal(user.Id, result.Data.Id);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Should return freemium plan when the user has closed all monthly plans preemium")]
        [Trait("[IntegrationTest]-AuthController", "GetControlAccessAsync")]
        public async Task ShouldReturnFreemiumPlanWhenTheUserHasClosedAllMonthlyPlansPreemium()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/auth/profile"
            };

            var userConstrua = await ClearUserPlansForConstruaAppGmail();
            await InsertUserPlan(UserPlanFaker.CreateUserPlanFreemium(userConstrua.Id));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_MENSAL, userConstrua.Id, DateTime.Now.AddMonths(-30), DateTime.Now.AddMonths(-29), (sbyte)BoolEnum.YES));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_MENSAL, userConstrua.Id, DateTime.Now.AddMonths(-28), DateTime.Now.AddMonths(-27), (sbyte)BoolEnum.YES));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_MENSAL, userConstrua.Id, DateTime.Now.AddMonths(-26), DateTime.Now.AddMonths(-25), (sbyte)BoolEnum.YES));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_MENSAL, userConstrua.Id, DateTime.Now.AddMonths(-24), DateTime.Now.AddMonths(-23), (sbyte)BoolEnum.YES));

            var user = await AuthLogin.GetUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);

            // act
            var response = await _testContext.Client.GetAsync($"{request.Url}");
            var result = await ContentHelper<UserControlAccessVOViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal((int)PlanWithTypeEnum.FREEMIUM_MENSAL, result.Data.PlanId);
            Assert.Equal(user.Id, result.Data.Id);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Should return freemium plan when the user has an closed plan preemium")]
        [Trait("[IntegrationTest]-AuthController", "GetControlAccessAsync")]
        public async Task ShouldReturnFreemiumPlanWhenTheUserHasClosedPlanPreemium()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/auth/profile"
            };

            var userConstrua = await ClearUserPlansForConstruaAppGmail();
            await InsertUserPlan(UserPlanFaker.CreateUserPlanFreemium(userConstrua.Id));
            await InsertUserPlan(UserPlanFaker.CreateUserPlanPremium((int)PlanWithTypeEnum.PREMIUM_MENSAL, userConstrua.Id, DateTime.Now.AddMonths(-2), DateTime.Now.AddMonths(-1), (sbyte)BoolEnum.YES));

            var user = await AuthLogin.GetUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);

            // act
            var response = await _testContext.Client.GetAsync($"{request.Url}");
            var result = await ContentHelper<UserControlAccessVOViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal((int)PlanWithTypeEnum.FREEMIUM_MENSAL, result.Data.PlanId);
            Assert.Equal(user.Id, result.Data.Id);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Should return freemium plan when the user hasnt plans")]
        [Trait("[IntegrationTest]-AuthController", "GetControlAccessAsync")]
        public async Task ShouldReturnFreemiumPlanWhenTheUserHasntPlans()
        {
            // arrange
            var request = new
            {
                Url = "/api/v1/auth/profile"
            };

            var userConstrua = await ClearUserPlansForConstruaAppGmail();
         
            var user = await AuthLogin.GetUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);

            // act
            var response = await _testContext.Client.GetAsync($"{request.Url}");
            var result = await ContentHelper<UserControlAccessVOViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal((int)PlanWithTypeEnum.FREEMIUM_MENSAL, result.Data.PlanId);
            Assert.Equal(user.Id, result.Data.Id);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Should return success when user update password")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnSuccessWhenUserUpdatePassword()
            {
            // arrange
            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var request = new
                {
                Url = "/api/v1/update/password"
                };

            var input = new UserUpdatePasswordInput();
            input.Password = AuthLogin.Password;
            input.PasswordConfirm = AuthLogin.Password;
                

            // act
            var response = await _testContext.Client.PutAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<UserViewModel>>(result);
            Assert.NotNull(result.Data.Token);
            }

        [Fact(DisplayName = "Should return not authorized when try to update password without token")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnNotAuthorizedWhenTryToUpdatePasswordWithoutToken()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/update/password"
                };

            var input = new UserUpdatePasswordInput();
            input.Password = AuthLogin.Password;
            input.PasswordConfirm = AuthLogin.Password;


            // act
            var response = await _testContext.Client.PutAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

        [Fact(DisplayName = "Should return success when user update")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnSuccessWhenUserUpdate()
            {
            // arrange
            var token = await AuthLogin.GetTokenUser(_testContext);
            _testContext.Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var request = new
                {
                Url = "/api/v1/update"
                };

            var input = new UserUpdateInput();
            input.Name = "Teste Integrado " + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo);


            // act
            var response = await _testContext.Client.PutAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result.Error);
            Assert.IsType<Result<UserViewModel>>(result);
            Assert.Equal(input.Name, result.Data.Name);
            }

        [Fact(DisplayName = "Should return not authorized when try to update user without token")]
        [Trait("[IntegrationTest]-AuthController", "AuthController")]
        public async Task ShouldReturnNotAuthorizedWhenTryToUpdateUserWithoutToken()
            {
            // arrange
            var request = new
                {
                Url = "/api/v1/update"
                };

            var input = new UserUpdatePasswordInput();
            input.Password = AuthLogin.Password;
            input.PasswordConfirm = AuthLogin.Password;


            // act
            var response = await _testContext.Client.PutAsync(request.Url, ContentHelper<object>.FormatStringContent(input));
            var result = await ContentHelper<UserViewModel>.GetResponse(response);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

        private async Task<User> ClearUserPlansForConstruaAppGmail()
        {
            IServiceCollection services = new ServiceCollection();
            var configuration = ConnectionString.GetConnection();
            services.AddDbContext<MySQLCoreContext>(options => options.UseMySql(configuration["ConnectionStrings:MySqlCore"]));
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IUserPlansRepository, UserPlansRepository>();
            services.AddLogging();
            using (ServiceProvider serviceProvider = services.AddEntityFrameworkMySql().BuildServiceProvider())
            {
                var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
                var userPlansRepository = serviceProvider.GetRequiredService<IUserPlansRepository>();
                var users = await userRepository.SelectFilterAsync(x => x.Email.Equals("construaapp@gmail.com"));
                var userConstrua = users.FirstOrDefault();
                var userPlans = await userPlansRepository.SelectFilterAsync(x => x.UserId.Equals(userConstrua.Id));
                foreach (var item in userPlans)
                {
                    await userPlansRepository.DeleteAsync(item.Id);
                }
                return userConstrua;
            }
        }

        private async Task InsertUserPlan(UserPlans input)
        {
            IServiceCollection services = new ServiceCollection();
            var configuration = ConnectionString.GetConnection();
            services.AddDbContext<MySQLCoreContext>(options => options.UseMySql(configuration["ConnectionStrings:MySqlCore"]));
            services.AddSingleton<IUserPlansRepository, UserPlansRepository>();
            services.AddLogging();
            using (ServiceProvider serviceProvider = services.AddEntityFrameworkMySql().BuildServiceProvider())
            {
                var userPlansRepository = serviceProvider.GetRequiredService<IUserPlansRepository>();
                await userPlansRepository.InsertAsync(input);
            }
        }
    }
}
