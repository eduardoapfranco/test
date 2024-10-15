using Domain.Entities;
using Domain.Enum;
using Domain.Input;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;
using Domain.ValueObjects;
using Infra.CrossCutting.Domain.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class UserDomainService : DomainService<User, int, IUnitOfWork>, IUserDomainService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IUserPlansRepository _userPlansRepository;
        private readonly IPasswordResetMobileRepository _passwordResetMobileRepository;
        private ISmartNotification _notification;
        ILogger<UserDomainService> _logger;
        private readonly IPlanDomainService _planDomainService;
        private readonly ISignatureDomainService _signatureDomainService;
        private readonly IProfileRepository _profileRepository;
        private readonly IProfileCategoryRepository _profileCategoryRepository;
        private readonly IProfileFunctionalityRepository _profileFunctionalityRepository;
        private readonly IUserAreaRepository _userAreaRepository;

        public UserDomainService(
            IUserRepository userRepository,
            IPlanRepository planRepository,
            IUserPlansRepository userPlansRepository,
            IPasswordResetMobileRepository passwordResetMobileRepository,
            ISmartNotification notification,
            IUnitOfWork unitOfWork,
            INotificationHandler<DomainNotification> messageHandler,
            ILogger<UserDomainService> logger,
            IPlanDomainService planDomainService,
            IProfileRepository profileRepository,
            IProfileCategoryRepository profileCategoryRepository,
            IProfileFunctionalityRepository profileFunctionalityRepository,
            ISignatureDomainService signatureDomainService,
            IUserAreaRepository userAreaRepository
        ) : base(userRepository, unitOfWork, messageHandler)
        {
            _userRepository = userRepository;
            _planRepository = planRepository;
            _userPlansRepository = userPlansRepository;
            _passwordResetMobileRepository = passwordResetMobileRepository;
            _notification = notification;
            _logger = logger;
            _planDomainService = planDomainService;
            _profileRepository = profileRepository;
            _profileCategoryRepository = profileCategoryRepository;
            _profileFunctionalityRepository = profileFunctionalityRepository;
            _signatureDomainService = signatureDomainService;
            _userAreaRepository = userAreaRepository;
        }

        public override async Task<User> InsertAsync(User item)
        {
            _logger.LogInformation($"Init register user {nameof(InsertAsync)}");
            try
            {
                using (_unitOfWork.BeginTransaction())
                {
                    _logger.LogInformation($"Get freemiun plan {nameof(InsertAsync)}");
                    var planFreemiun =
                        await _planRepository.GetWithTypeByTitle("Freemium");

                    if (null == planFreemiun)
                    {
                        _notification.NewNotificationBadRequest(_notification.EmptyPositions(),
                            "Falha ao vincular usuário com plano freemium!");
                        _logger.LogError($"Get freemiun plan not found {nameof(InsertAsync)} for UserInput: {item}");
                        return default;
                    }

                    var user = await _unitOfWork.User.InsertAsync(item);
                    await _unitOfWork.SaveChangesAsync();

                    if (user == null)
                    {
                        _logger.LogError($"Fail insert user {nameof(InsertAsync)} UserInput: {item}");
                        _notification.NewNotificationBadRequest(_notification.EmptyPositions(),
                            "Falha ao cadastrar usuário, tente novamente!");
                        return default;
                    }

                    _logger.LogInformation($"Insert user {nameof(InsertAsync)} User: {JsonConvert.SerializeObject(user)}");

                    var userPlan = new UserPlans()
                    {
                        UserId = user.Id,
                        PlanId = planFreemiun.Id,
                        DueDateAt = DateTime.Now,
                        ValueDebit = 0,
                        StatusPayment = (sbyte)BoolEnum.NO,
                        Deleted = (sbyte)BoolEnum.NO,
                        DueInstallment = 0
                    };

                    var insertUserPlan = await _unitOfWork.UserPlans.InsertAsync(userPlan);
                    await _unitOfWork.SaveChangesAsync();

                    if (insertUserPlan == null)
                    {
                        _notification.NewNotificationBadRequest(_notification.EmptyPositions(),
                            "Falha ao vincular usuário com plano freemium!");
                        _logger.LogError($"Fail to vinculate user with plan {nameof(InsertAsync)} UserPlan: {userPlan}");

                        return default;
                    }
                    _logger.LogInformation($"Insert user plan {nameof(InsertAsync)} User: {JsonConvert.SerializeObject(insertUserPlan)}");

                    Commit();

                    var trialPlan =
                       await _planRepository.GetWithTypeByTitle("Trial");

                    if (null == trialPlan)
                    {
                        _notification.NewNotificationBadRequest(_notification.EmptyPositions(),
                            "Falha ao vincular usuário com plano Trial!");
                        _logger.LogError($"Get trial plan not found {nameof(InsertAsync)} for UserInput: {item}");
                    }
                    else
                    {
                        user = await _signatureDomainService.UpdateUserToPremiumPlanAsync(user, trialPlan);
                    }
                    return user;
                }
            }
            catch (Exception ex)
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(),
                            "Falha ao inserir o usuário!");
                _logger.LogError($"Fail when insert user async {nameof(InsertAsync)} for UserInput: {item} with error: {ex.Message}");
            }

            return default;
        }

        public async Task<int> RequestPasswordResetAsync(RequestPasswordResetInput input)
        {
            _logger.LogInformation($"Init request password reset {nameof(RequestPasswordResetAsync)} input: {JsonConvert.SerializeObject(input)}");
            using (_unitOfWork.BeginTransaction())
            {
                var searchUser = await _repository.SelectFilterAsync(x => x.Email.Equals(input.Email));
                var user = searchUser.FirstOrDefault();

                if (user == null)
                {
                    _notification.NewNotificationBadRequest(new string[] { input.Email },
                        "O e-mail '{0}' não foi encontrado em nosso sistema.");
                    _logger.LogInformation($"E-mail {input.Email} not found {nameof(RequestPasswordResetAsync)}");
                    return 0;
                }

                var numbersActives = await _unitOfWork.PasswordResetMobile.SelectFilterAsync(x =>
                    x.UserId == user.Id && x.Used == (byte)PasswordResetMobileEnum.NO);
                _logger.LogInformation($"Get number actives for {nameof(RequestPasswordResetAsync)} user: {JsonConvert.SerializeObject(user)}");
                if (numbersActives.Any())
                {
                    foreach (var numberActive in numbersActives)
                    {
                        await _unitOfWork.PasswordResetMobile.DeleteAsync(numberActive.Id);
                        await _unitOfWork.SaveChangesAsync();
                        _logger.LogInformation($"Delete number active for {nameof(RequestPasswordResetAsync)} NumberActive: {JsonConvert.SerializeObject(numberActive)}");
                    }
                }

                var passordResetMobile =
                    await _unitOfWork.PasswordResetMobile.InsertAsync(new PasswordReset(user.Id, user.Email));
                await _unitOfWork.SaveChangesAsync();

                if (passordResetMobile == null)
                {
                    _notification.NewNotificationBadRequest(_notification.EmptyPositions(),
                        "Falha ao gerar número para resetar a senha.");
                    _logger.LogError($"Error for generate number password reset mobile {nameof(RequestPasswordResetAsync)}");
                    return 0;
                }

                _logger.LogInformation($"Generate number {nameof(RequestPasswordResetAsync)} PasswordResetMobile: {JsonConvert.SerializeObject(passordResetMobile)}");

                Commit();
                return passordResetMobile.CheckerNumber;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordInput input)
        {
            _logger.LogInformation($"Init password reset {nameof(ResetPasswordAsync)} input: {JsonConvert.SerializeObject(input)}");
            using (_unitOfWork.BeginTransaction())
            {
                var searchUser = await _repository.SelectFilterAsync(x => x.Email.Equals(input.Email));
                var user = searchUser.FirstOrDefault();

                if (user == null)
                {
                    _notification.NewNotificationBadRequest(new string[] { input.Email },
                        "O e-mail '{0}' não foi encontrado em nosso sistema.");
                    _logger.LogInformation($"E-mail {input.Email} not found {nameof(ResetPasswordAsync)}");
                    return false;
                }

                var passwordsResets = await _unitOfWork.PasswordResetMobile.SelectFilterAsync(x =>
                    x.CheckerNumber.Equals(input.CheckerNumber) && x.UserEmail.Equals(input.Email) &&
                    x.Used.Equals((byte)PasswordResetMobileEnum.NO));

                var checkerNumber = passwordsResets.FirstOrDefault();

                if (checkerNumber == null)
                {
                    _notification.NewNotificationBadRequest(new string[] { input.Email },
                        "O código de verificação é inválido.");
                    _logger.LogInformation($"Checker Number invalid for e-mail: {input.Email} {nameof(ResetPasswordAsync)}");
                    return false;
                }

                if (checkerNumber.CreatedAt.AddHours(5) < DateTime.Now)
                {
                    _notification.NewNotificationBadRequest(_notification.EmptyPositions(),
                        "O código de verificação expirou!");
                    _logger.LogInformation($"Checker Number expired for e-mail: {input.Email} {nameof(ResetPasswordAsync)}");

                    return false;
                }

                user.Password = input.NewPasswordCrypto;
                var userUpdatedPassword = await _unitOfWork.User.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();


                var passwordResetMobile = new PasswordReset(checkerNumber.UserId, checkerNumber.UserEmail, (byte)PasswordResetMobileEnum.YES, checkerNumber.CheckerNumber);
                passwordResetMobile.Id = checkerNumber.Id;
                passwordResetMobile.CreatedAt = checkerNumber.CreatedAt;
                var updatedPasswordResetForUsed = await _unitOfWork.PasswordResetMobile.UpdateAsync(passwordResetMobile);
                await _unitOfWork.SaveChangesAsync();

                if (userUpdatedPassword == null || updatedPasswordResetForUsed == null)
                {
                    _logger.LogError($"Error Reset password for e-mail: {input.Email} {nameof(ResetPasswordAsync)}");
                    return false;
                }

                _logger.LogInformation($"Reset password with success for e-mail: {input.Email} {nameof(ResetPasswordAsync)}");

                Commit();
                return true;
            }
        }

        public async Task<User> GetUserAsync(int userId)
        {
            _logger.LogInformation($"GetUserAsync with id {userId}");
            var searchUser = await _repository.SelectByIdAsync(userId);
            if (null != searchUser)
            {
                _logger.LogInformation($"GetUserAsync with id {userId} find user successfull");
                return searchUser;
            }
            else
            {
                _logger.LogWarning($"GetUserAsync with id {userId} could not find user");
                return default;
            }
        }

        public async Task<User> GetUserByEmailAsync(User user)
        {
            _logger.LogInformation($"GetUserByEmailAsync with email {user.Email}");
            var searchUser = await _repository.SelectFilterAsync(x => x.Email.Equals(user.Email));
            if (searchUser.Any())
            {
                User userReturned = searchUser.FirstOrDefault();
                _logger.LogInformation($"GetUserByEmailAsync with email {user.Email} find user {userReturned.Id}");
                return userReturned;
            }
            else
            {
                _logger.LogWarning($"GetUserByEmailAsync with email {user.Email} does'nt find any user");
                return default;
            }
        }

        public async Task<bool> UpdateAreasAsync(int userId, int[] AreasIds)
            {

            var dbUserAreas = await _userAreaRepository.SelectFilterAsync(x => x.UserId.Equals(userId));

            var toDeleteDb = dbUserAreas
                .Where(db => !AreasIds.Any(app => app == db.AreaId))   // Existem no DB mas não existem no App
                .ToList();

            var toInsertDb = AreasIds
                .Where(app => !dbUserAreas.Any(db => db.AreaId == app))                    // Existem no App mas nao existem no DB
                .ToList();
            bool result = true;

            foreach (var areaId in toInsertDb)
                {
                var userAreaInserted = await _userAreaRepository.InsertAsync(
                    new UserAreas()
                        {
                        AreaId = areaId,
                        UserId = userId
                        }
                    );
                if (0 == userAreaInserted.Id)
                    {
                    _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao incluir user area");
                    return false;
                    }
                }

            foreach (var userAreas in toDeleteDb)
                {
                result = await _userAreaRepository.DeleteAsync(userAreas.Id);
                if (!result)
                    {
                    _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao excluir user areas");
                    return false;
                    }
                }

            return result;
            }

        public async Task<UserControlAccessVO> GetControlAccessAsync(int userId)
        {
            var userPlan = await _userPlansRepository.GetPlanUserTerm(userId);
            var plan = await _planRepository.GetWithType(userPlan.PlanId);
            var profile = await _profileRepository.GetProfileAsync(plan.Id);
            var profileCategories = await _profileCategoryRepository.GetCategoriesProfile(profile.Id);
            var profileFunctionalities = await _profileFunctionalityRepository.GetFunctionalitiesProfile(profile.Id);
            var userAreas = await _userAreaRepository.GetUserAreas(userId);

            return new UserControlAccessVO(userPlan, plan, profile, profileCategories, profileFunctionalities, userAreas);
        }
    }
}
