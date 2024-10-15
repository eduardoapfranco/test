using Domain.Entities;
using Domain.Enum;
using Domain.Input;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;
using FizzWare.NBuilder;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnitTest.Application.UserApplication.Faker;
using UnitTest.Domain.Faker;
using Xunit;
using DomainTest = Domain.Services;

namespace UnitTest.Domain
    {
    public class UserDomainTest
        {
        private Mock<IUserPlansRepository> _userPlansRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IPlanRepository> _planRepositoryMock;
        private Mock<IPasswordResetMobileRepository> _passwordResetMobileRepositoryMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private DomainTest.UserDomainService _userDomainService;
        private Mock<ILogger<DomainTest.UserDomainService>> _loggerMock;
        private Mock<IPlanDomainService> _planDomainServiceMock;
        private Mock<IProfileRepository> _profileRepositoryMock;
        private Mock<IProfileCategoryRepository> _profileCategoryRepositoryMock;
        private Mock<IProfileFunctionalityRepository> _profileFunctionalityRepositoryMock;
        private Mock<ISignatureDomainService> _signatureDomainServiceMock;
        private Mock<IUserAreaRepository> _userAreaRepositoryMock;
        

        public UserDomainTest()
            {
            _userPlansRepositoryMock = new Mock<IUserPlansRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _planRepositoryMock = new Mock<IPlanRepository>();
            _passwordResetMobileRepositoryMock = new Mock<IPasswordResetMobileRepository>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DomainTest.UserDomainService>>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _planDomainServiceMock = new Mock<IPlanDomainService>();
            _profileRepositoryMock = new Mock<IProfileRepository>();
            _profileCategoryRepositoryMock = new Mock<IProfileCategoryRepository>();
            _profileFunctionalityRepositoryMock = new Mock<IProfileFunctionalityRepository>();
            _signatureDomainServiceMock = new Mock<ISignatureDomainService>();
            _signatureDomainServiceMock = new Mock<ISignatureDomainService>();
            _userAreaRepositoryMock = new Mock<IUserAreaRepository>();
            _userDomainService = new DomainTest.UserDomainService(_userRepositoryMock.Object, _planRepositoryMock.Object, _userPlansRepositoryMock.Object, _passwordResetMobileRepositoryMock.Object, _smartNotificationMock.Object, _unitOfWorkMock.Object, new DomainNotificationHandler(), _loggerMock.Object, _planDomainServiceMock.Object,
                _profileRepositoryMock.Object, _profileCategoryRepositoryMock.Object, _profileFunctionalityRepositoryMock.Object, _signatureDomainServiceMock.Object, _userAreaRepositoryMock.Object);

            }

        [Fact(DisplayName = "Shoud return null user when not exists freemium plan override insert async")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-InsertAsync")]
        public async Task ShouldNullUserWhenNotExistsFreemiumPlanOverrideInsertAsync()
            {
            // arrange
            var user = UserFaker.CreateUser;
            var listPlan = new List<Plan>();
            _planRepositoryMock.Setup(x => x.SelectFilterAsync(x => x.Title.Equals("Freemium") && x.Active.Equals(1)))
                .ReturnsAsync(listPlan);

            // act
            var result = await _userDomainService.InsertAsync(user);

            // assert
            Assert.Null(result);
            }

        [Fact(DisplayName = "Shoud return null user when not insert user override insert async")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-InsertAsync")]
        public async Task ShouldNullUserWhenNotInsertUserOverrideInsertAsync()
            {
            // arrange
            User user = null;
            var listPlan = PlanFaker.CreateListPlansFreemium();
            _planRepositoryMock.Setup(x => x.SelectFilterAsync(x => x.Title.Equals("Freemium") && x.Active.Equals(1)))
                .ReturnsAsync(listPlan);
            _unitOfWorkMock.Setup(x => x.User.InsertAsync(user)).ReturnsAsync(user);

            // act
            var result = await _userDomainService.InsertAsync(user);

            // assert
            Assert.Null(result);
            }

        [Fact(DisplayName = "Shoud return null user when not insert user plans override insert async")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-InsertAsync")]
        public async Task ShouldNullUserWhenNotInsertUserPlansOverrideInsertAsync()
            {
            // arrange
            var user = UserFaker.CreateUser;
            var listPlan = PlanFaker.CreateListPlansFreemium();
            UserPlans userPlan = null;
            _planRepositoryMock.Setup(x => x.SelectFilterAsync(x => x.Title.Equals("Freemium") && x.Active.Equals(1)))
                .ReturnsAsync(listPlan);
            _unitOfWorkMock.Setup(x => x.User.InsertAsync(user)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(x => x.UserPlans.InsertAsync(userPlan)).ReturnsAsync(userPlan);

            // act
            var result = await _userDomainService.InsertAsync(user);

            // assert
            Assert.Null(result);
            }

        [Fact(DisplayName = "Shoud return return user with sucess override insert async")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-InsertAsync")]
        public async Task ShouldReturnUserWithSuccessOverrideInsertAsync()
            {
            // arrange
            var user = UserFaker.CreateUser;
            var listPlan = PlanFaker.CreateListPlansFreemium();
            var userPlan = UserPlanFaker.CreateUserPlan();
            _planRepositoryMock.Setup(x => x.GetWithTypeByTitle("Freemium")).ReturnsAsync(listPlan.FirstOrDefault);
            _unitOfWorkMock.Setup(x => x.User.InsertAsync(user)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(x => x.UserPlans.InsertAsync(It.IsAny<UserPlans>())).ReturnsAsync(userPlan);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(new CommandResponse(true));

            // act
            var result = await _userDomainService.InsertAsync(user);

            // assert
            Assert.IsType<User>(result);
            }

        [Fact(DisplayName = "Shoud return return user with trial plan sucess override insert async")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-InsertAsync")]
        public async Task ShouldReturnUserWithTrialPlanWithSuccessOverrideInsertAsync()
            {
            // arrange
            var user = UserFaker.CreateUser;
            var listPlan = PlanFaker.CreateListPlansFreemium();
            var listPlanTrial = PlanFaker.CreateListPlansPreemiumTrial();
            var userPlan = UserPlanFaker.CreateUserPlan();
            var userPlanTrial = UserPlanFaker.CreateUserPlan();
            userPlanTrial.PlanId = 6;
            _planRepositoryMock.Setup(x => x.GetWithTypeByTitle("Freemium"))
                .ReturnsAsync(listPlan.FirstOrDefault);
            _planRepositoryMock.Setup(x => x.GetWithTypeByTitle("Trial"))
                .ReturnsAsync(listPlanTrial.FirstOrDefault);
            _unitOfWorkMock.Setup(x => x.User.InsertAsync(user)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(x => x.UserPlans.InsertAsync(It.Is<UserPlans>(x => x.PlanId == 1))).ReturnsAsync(userPlan);
            _unitOfWorkMock.Setup(x => x.UserPlans.InsertAsync(It.Is<UserPlans>(x => x.PlanId != 1))).ReturnsAsync(userPlanTrial);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(new CommandResponse(true));
            _signatureDomainServiceMock.Setup(x => x.UpdateUserToPremiumPlanAsync(It.IsAny<User>(), It.IsAny<Plan>(), It.IsAny<string>())).ReturnsAsync(user);
            // act
            var result = await _userDomainService.InsertAsync(user);

            // assert
            Assert.IsType<User>(result);
            }

        [Fact(DisplayName = "Shoud return return 0 when user not exists")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-RequestPasswordResetAsync")]
        public async Task ShouldReturnZeroWhenUserNotExists()
            {
            // arrange
            var input = Builder<RequestPasswordResetInput>.CreateNew().Build();
            _userRepositoryMock.Setup(x => x.SelectFilterAsync(x => x.Email.Equals(input.Email)))
                .ReturnsAsync(new List<User>());

            // act
            var result = await _userDomainService.RequestPasswordResetAsync(input);

            // assert
            Assert.IsType<int>(result);
            Assert.Equal(0, result);
            }

        [Fact(DisplayName = "Shoud return return checker number when delete number actives")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-RequestPasswordResetAsync")]
        public async Task ShouldReturnCheckerNumberWhenDeleteNumbersActives()
            {
            // arrange
            var input = Builder<RequestPasswordResetInput>.CreateNew().Build();
            input.Email = "teste@teste.com";
            var listUser = UserFaker.CreateListUser();
            var user = listUser.FirstOrDefault();
            user.Email = input.Email;
            var listNumbersActives = PasswordResetMobileFaker.CreateListPasswordResetMobile();
            var resultInsert = PasswordResetMobileFaker.CreateListPasswordResetMobile().FirstOrDefault();
            var numberActive = listNumbersActives.FirstOrDefault();
            _userRepositoryMock.Setup(x => x.SelectFilterAsync(x => x.Email.Equals(input.Email)))
                .ReturnsAsync(listUser);
            _unitOfWorkMock.Setup(y => y.PasswordResetMobile.SelectFilterAsync(x => x.UserId == user.Id && x.Used == (byte)PasswordResetMobileEnum.NO)).ReturnsAsync(listNumbersActives);
            _unitOfWorkMock.Setup(x => x.PasswordResetMobile.DeleteAsync(numberActive.Id)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.PasswordResetMobile.InsertAsync(It.IsAny<PasswordReset>())).ReturnsAsync(resultInsert);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(new CommandResponse(true));

            // act
            var result = await _userDomainService.RequestPasswordResetAsync(input);

            // assert
            Assert.IsType<int>(result);
            Assert.Equal(6, result.ToString().Length);
            }

        [Fact(DisplayName = "Shoud return return zero when dont generate checker number")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-RequestPasswordResetAsync")]
        public async Task ShouldReturnZeroWhenDontGenerateCheckerNumber()
            {
            // arrange
            var input = Builder<RequestPasswordResetInput>.CreateNew().Build();
            input.Email = "teste@teste.com";
            var listUser = UserFaker.CreateListUser();
            var user = listUser.FirstOrDefault();
            user.Email = input.Email;
            PasswordReset resultInsert = null;
            var listNumbersActives = PasswordResetMobileFaker.CreateListPasswordResetMobile();
            var numberActive = listNumbersActives.FirstOrDefault();
            _userRepositoryMock.Setup(x => x.SelectFilterAsync(x => x.Email.Equals(input.Email)))
                .ReturnsAsync(listUser);
            _unitOfWorkMock.Setup(y => y.PasswordResetMobile.SelectFilterAsync(x => x.UserId == user.Id && x.Used == (byte)PasswordResetMobileEnum.NO)).ReturnsAsync(new List<PasswordReset>());
            _unitOfWorkMock.Setup(x => x.PasswordResetMobile.DeleteAsync(numberActive.Id)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.PasswordResetMobile.InsertAsync(It.IsAny<PasswordReset>())).ReturnsAsync(resultInsert);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(new CommandResponse(true));

            // act
            var result = await _userDomainService.RequestPasswordResetAsync(input);

            // assert
            Assert.IsType<int>(result);
            Assert.Equal(0, result);
            }

        [Fact(DisplayName = "Shoud return return false when dont found user for reset password")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-ResetPasswordAsync")]
        public async Task ShouldReturnFalseWhenDontFoundUserForResetPassword()
            {
            // arrange
            var input = Builder<ResetPasswordInput>.CreateNew().Build();
            _userRepositoryMock.Setup(x => x.SelectFilterAsync(x => x.Email.Equals(input.Email)))
                .ReturnsAsync(new List<User>());

            // act
            var result = await _userDomainService.ResetPasswordAsync(input);

            // assert
            Assert.IsType<bool>(result);
            Assert.False(result);
            }

        [Fact(DisplayName = "Shoud return return false when checker number invalid for reset password")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-ResetPasswordAsync")]
        public async Task ShouldReturnFalseWhenCheckerNumberInvalidForResetPassword()
            {
            // arrange
            var input = Builder<ResetPasswordInput>.CreateNew().Build();
            var users = UserFaker.CreateListUser();
            _userRepositoryMock.Setup(x => x.SelectFilterAsync(x => x.Email.Equals(input.Email)))
                .ReturnsAsync(users);

            _unitOfWorkMock.Setup(y => y.PasswordResetMobile.SelectFilterAsync(x => x.UserId == 1 && x.Used == (byte)PasswordResetMobileEnum.NO)).ReturnsAsync(new List<PasswordReset>());

            // act
            var result = await _userDomainService.ResetPasswordAsync(input);

            // assert
            Assert.IsType<bool>(result);
            Assert.False(result);
            }

        [Fact(DisplayName = "Shoud return return false when checker number expired for reset password")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-ResetPasswordAsync")]
        public async Task ShouldReturnFalseWhenCheckerNumberExpiredForResetPassword()
            {
            // arrange
            var input = Builder<ResetPasswordInput>.CreateNew().Build();
            var users = UserFaker.CreateListUser();
            var passwordsReset = PasswordResetMobileFaker.CreateListPasswordResetMobile();
            var checkerNumber = passwordsReset.FirstOrDefault();
            checkerNumber.CreatedAt = DateTime.Now.AddHours(-10);
            _userRepositoryMock.Setup(x => x.SelectFilterAsync(x => x.Email.Equals(input.Email)))
                .ReturnsAsync(users);

            _unitOfWorkMock.Setup(y => y.PasswordResetMobile.SelectFilterAsync(It.IsAny<Expression<Func<PasswordReset, bool>>>())).ReturnsAsync(passwordsReset);

            // act
            var result = await _userDomainService.ResetPasswordAsync(input);

            // assert
            Assert.IsType<bool>(result);
            Assert.False(result);
            }

        [Fact(DisplayName = "Shoud return return false when not change user password for reset password")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-ResetPasswordAsync")]
        public async Task ShouldReturnFalseWhenNotChangeUserPasswordForResetPassword()
            {
            // arrange
            var input = Builder<ResetPasswordInput>.CreateNew().Build();
            var users = UserFaker.CreateListUser();
            User user = null;
            PasswordReset passwordReset = null;
            var passwordsReset = PasswordResetMobileFaker.CreateListPasswordResetMobile();
            _userRepositoryMock.Setup(x => x.SelectFilterAsync(x => x.Email.Equals(input.Email)))
                .ReturnsAsync(users);
            _unitOfWorkMock.Setup(y => y.PasswordResetMobile.SelectFilterAsync(It.IsAny<Expression<Func<PasswordReset, bool>>>())).ReturnsAsync(passwordsReset);
            _unitOfWorkMock.Setup(x => x.User.UpdateAsync(new User())).ReturnsAsync(user);
            _unitOfWorkMock.Setup(x => x.PasswordResetMobile.UpdateAsync(new PasswordReset(1, "teste@teste.com"))).ReturnsAsync(passwordReset);
            // act
            var result = await _userDomainService.ResetPasswordAsync(input);

            // assert
            Assert.IsType<bool>(result);
            Assert.False(result);
            }

        [Fact(DisplayName = "Shoud return return false when not change password reset for used for reset password")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-ResetPasswordAsync")]
        public async Task ShouldReturnFalseWhenNotChangePasswordResetForUsedForResetPassword()
            {
            // arrange
            var input = Builder<ResetPasswordInput>.CreateNew().Build();
            var users = UserFaker.CreateListUser();
            var user = UserFaker.CreateUser;
            PasswordReset passwordReset = null;
            var passwordsReset = PasswordResetMobileFaker.CreateListPasswordResetMobile();
            _userRepositoryMock.Setup(x => x.SelectFilterAsync(x => x.Email.Equals(input.Email)))
                .ReturnsAsync(users);
            _unitOfWorkMock.Setup(y => y.PasswordResetMobile.SelectFilterAsync(It.IsAny<Expression<Func<PasswordReset, bool>>>())).ReturnsAsync(passwordsReset);
            _unitOfWorkMock.Setup(x => x.User.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);
            _unitOfWorkMock.Setup(x => x.PasswordResetMobile.UpdateAsync(new PasswordReset(1, "teste@teste.com"))).ReturnsAsync(passwordReset);
            // act
            var result = await _userDomainService.ResetPasswordAsync(input);

            // assert
            Assert.IsType<bool>(result);
            Assert.False(result);
            }


        [Fact(DisplayName = "Shoud return success change password for reset password")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-ResetPasswordAsync")]
        public async Task ShouldSuccessChangePasswordForResetPassword()
            {
            // arrange
            var input = Builder<ResetPasswordInput>.CreateNew().Build();
            var users = UserFaker.CreateListUser();
            var user = UserFaker.CreateUser;
            var passwordReset = PasswordResetMobileFaker.CreatePasswordResetMobile();
            var passwordsReset = PasswordResetMobileFaker.CreateListPasswordResetMobile();
            _userRepositoryMock.Setup(x => x.SelectFilterAsync(x => x.Email.Equals(input.Email)))
                .ReturnsAsync(users);
            _unitOfWorkMock.Setup(y => y.PasswordResetMobile.SelectFilterAsync(It.IsAny<Expression<Func<PasswordReset, bool>>>())).ReturnsAsync(passwordsReset);
            _unitOfWorkMock.Setup(x => x.User.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);
            _unitOfWorkMock.Setup(x => x.PasswordResetMobile.UpdateAsync(It.IsAny<PasswordReset>())).ReturnsAsync(passwordReset);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(new CommandResponse(true));

            // act
            var result = await _userDomainService.ResetPasswordAsync(input);

            // assert
            Assert.IsType<bool>(result);
            Assert.True(result);
            }

        [Fact(DisplayName = "Shoud return control access for user logged")]
        [Trait("[Domain.Services]-UserDomainService", "Domain-GetControlAccessAsync")]
        public async Task ShouldReturnControlAccessForUserLogged()
            {
            // arrange 
            var userPlan = Builder<UserPlans>.CreateNew().Build();
            var plan = Builder<Plan>.CreateNew().Build();
            var profile = Builder<Profile>.CreateNew().Build();
            var profileCategories = Builder<Category>.CreateListOfSize(10).Build();
            var profileFunctionalities = Builder<Functionality>.CreateListOfSize(10).Build();
            _userPlansRepositoryMock.Setup(x => x.GetPlanUserTerm(It.IsAny<int>())).ReturnsAsync(userPlan);
            _planRepositoryMock.Setup(x => x.GetWithType(It.IsAny<int>())).ReturnsAsync(plan);
            _profileRepositoryMock.Setup(x => x.GetProfileAsync(It.IsAny<int>())).ReturnsAsync(profile);
            _profileCategoryRepositoryMock.Setup(x => x.GetCategoriesProfile(It.IsAny<int>())).ReturnsAsync(profileCategories);
            _profileFunctionalityRepositoryMock.Setup(x => x.GetFunctionalitiesProfile(It.IsAny<int>())).ReturnsAsync(profileFunctionalities);

            // act 
            var result = await _userDomainService.GetControlAccessAsync(It.IsAny<int>());

            // assert
            Assert.NotNull(result);
            }
        }
    }
