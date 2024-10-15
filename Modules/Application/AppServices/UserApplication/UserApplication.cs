using Application.AppServices.UserApplication.Input;
using Application.AppServices.UserApplication.ViewModel;
using Application.Emails.User;
using Application.Interfaces;
using AutoMapper;
using BCrypt.Net;
using Domain.Entities;
using Domain.Input;
using Domain.Interfaces.Services;
using Domain.Services;
using Infra.CrossCutting.Auth.Intefaces;
using Infra.CrossCutting.Email;
using Infra.CrossCutting.Email.Interfaces;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.AppServices.UserApplication
{
    public class UserApplication : BaseValidationService, IUserApplication
    {
        private readonly ISmartNotification _notification;
        private readonly IUserDomainService _userDomainService;
        private readonly IUserPaymentMethodDomainService _userPaymentMethodDomainService;
        private readonly IEmailSendService _emailSendService;
        private readonly IAuthService<User> _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserApplication> _logger;


        public UserApplication(IUserDomainService userDomainService, ISmartNotification notification, IMapper mapper,
            IEmailSendService emailSendService, IAuthService<User> authService, ILogger<UserApplication> logger, IUserPaymentMethodDomainService userPaymentMethodDomainService) : base(notification)
        {
            _userDomainService = userDomainService;
            _notification = notification;
            _mapper = mapper;
            _emailSendService = emailSendService;
            _authService = authService;
            _logger = logger;
            _userPaymentMethodDomainService = userPaymentMethodDomainService;
        }

        public async Task<UserViewModel> InsertMobileAsync(UserInput input)
        {
            _logger.LogInformation($"Init register user {nameof(InsertMobileAsync)}");

            if (!input.IsValid())
            {
                var userViewModel = _mapper.Map<UserViewModel>(input);
                NotifyErrorsAndValidation(_notification.EmptyPositions(), userViewModel);
                _logger.LogWarning($"Init register user with param invalid {nameof(InsertMobileAsync)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
            }

            var existsUser = await _userDomainService.SelectFilterAsync(x => x.Email.Equals(input.Email));
            _logger.LogInformation($"Validate existent user in {nameof(InsertMobileAsync)} for e-mail: {input.Email}");

            if (existsUser.Any())
            {
                _notification.NewNotificationBadRequest(new string[] { input.Email }, "O e-mail '{0}' já está cadastrado em nosso sistema.");
                _logger.LogWarning($"User existent in {nameof(InsertMobileAsync)} for e-mail: {input.Email}");

                return default;
            }

            var entity = _mapper.Map<User>(input);

            entity.Status = "A";
            entity.IsAdmin = "N";

            entity.Password = BCrypt.Net.BCrypt.HashPassword(entity.Password, BCrypt.Net.BCrypt.GenerateSalt(), false, HashType.SHA256);

            var result = await _userDomainService.InsertAsync(entity);
            _logger.LogInformation($"User register {nameof(InsertMobileAsync)} for e-mail: {JsonConvert.SerializeObject(result)}");


            var body = WelcomeUser.FormatEmailSendWelcomeUser(entity.Name);

            var emailSendInput = new EmailSendInput()
            {
                Name = result.Name,
                Email = result.Email,
                Body = body,
                BodyHtml = true,
                Subject = "Construa App - Seja Bem Vindo(a)"
            };

            _logger.LogInformation($"Call SendEmailAsync in  {nameof(InsertMobileAsync)} for email: {input.Email}");
            _emailSendService.SendEmailAsync(emailSendInput, new EmailConfiguration());

            var userToken = _mapper.Map<UserViewModel>(result);
            userToken.Token = _authService.GenerateToken(result);

            return userToken;
        }

        public async Task<UserViewModel> LoginAsync(UserLoginInput input)
        {
            _logger.LogInformation($"Init login user {nameof(LoginAsync)}");
            if (!input.IsValid())
            {
                var userViewModel = _mapper.Map<UserViewModel>(input);
                NotifyErrorsAndValidation(_notification.EmptyPositions(), userViewModel);
                _logger.LogWarning($"Init logyn async with param invalid {nameof(LoginAsync)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
            }

            var listUsers = await _userDomainService.SelectFilterAsync(x => x.Email.Equals(input.Email));

            if (!listUsers.Any())
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "E-mail e/ou senha inválidos.");
                _logger.LogWarning($"User not found {nameof(LoginAsync)} for e-mail: {input.Email}");

                return default;
            }

            bool verified = BCrypt.Net.BCrypt.Verify(input.Password, listUsers.FirstOrDefault().Password);

            if (verified)
            {
                var user = listUsers.FirstOrDefault();
                var userToken = _mapper.Map<UserViewModel>(user);
                userToken.Token = _authService.GenerateToken(user); ;

                if (userToken.Token != null && userToken.Token != "")
                {
                    _logger.LogInformation($"Token generated with success in {nameof(LoginAsync)} for e-mail:  {input.Email}");

                    if (!string.IsNullOrWhiteSpace(input.TokenNotification))
                    {
                        user.TokenNotification = input.TokenNotification;
                    }

                    user.LastLoginDate = DateTime.Now;
                    await _userDomainService.UpdateAsync(user);
                    var payments = await _userPaymentMethodDomainService.SelectFilterAsync(x => x.Active.Equals(1));
                    userToken.PaymentDefaultMethod = payments.FirstOrDefault();

                    return userToken;
                }
                _logger.LogWarning($"TToken don't generate {nameof(LoginAsync)} for e-mail: {input.Email}");

            }
            _logger.LogWarning($"Credential invalid  {nameof(LoginAsync)} for e-mail: {input.Email}");
            _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "E-mail e/ou senha inválidos.");
            return default;
        }

        public async Task<UserRequestPasswordResetViewModel> RequestPasswordResetAsync(UserRequestPasswordResetInput input)
        {
            _logger.LogInformation($"Init {nameof(RequestPasswordResetAsync)}");
            var userRequestPasswordResetViewModel = new UserRequestPasswordResetViewModel();
            if (!input.IsValid())
            {
                userRequestPasswordResetViewModel.ValidationResult = input.ValidationResult;
                NotifyErrorsAndValidation(_notification.EmptyPositions(), userRequestPasswordResetViewModel);
                _logger.LogWarning($"Request password reset with param invalid {nameof(RequestPasswordResetAsync)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
            }

            var paramInput = new RequestPasswordResetInput()
            {
                Email = input.Email
            };

            var requestNumber = await _userDomainService.RequestPasswordResetAsync(paramInput);

            var body = PasswordResetMobileSendEmail.FormatEmailSendPasswordResetMobile(requestNumber);

            if (requestNumber > 0)
            {
                var emailSendInput = new EmailSendInput()
                {
                    Name = "",
                    Email = paramInput.Email,
                    Body = body,
                    BodyHtml = true,
                    Subject = "Construa App - Número de Verificação"
                };
                _logger.LogInformation($"Call SendEmailAsync in  {nameof(RequestPasswordResetAsync)} for email: {input.Email}");
                _emailSendService.SendEmailAsync(emailSendInput, new EmailConfiguration());

                return userRequestPasswordResetViewModel;
            }
            _logger.LogWarning($"Checker number don't request generate {nameof(RequestPasswordResetAsync)} for e-mail: {input.Email}");
            return default;
        }

        public async Task<UserViewModel> ResetPasswordAsync(UserResetPasswordInput input)
        {
            var userViewModel = new UserViewModel();
            _logger.LogInformation($"Init {nameof(ResetPasswordAsync)}");
            if (!input.IsValid())
            {
                userViewModel.ValidationResult = input.ValidationResult;
                NotifyErrorsAndValidation(_notification.EmptyPositions(), userViewModel);
                _logger.LogWarning($"Reset password invalid {nameof(ResetPasswordAsync)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
            }

            var domainInput = new ResetPasswordInput()
            {
                Email = input.Email,
                NewPasswordCrypto = BCrypt.Net.BCrypt.HashPassword(input.Password, BCrypt.Net.BCrypt.GenerateSalt(), false, HashType.SHA256),
                CheckerNumber = input.CheckerNumber
            };

            var updatedResetPassword = await _userDomainService.ResetPasswordAsync(domainInput);

            if (!updatedResetPassword)
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao alterar senha, tente novamente.");
                _logger.LogWarning($"Fail Reset password {nameof(ResetPasswordAsync)} for e-mail: {input.Email}");
                return default;
            }

            var body = PasswordResetMobileSendEmailSuccessfully.FormatEmailSendPasswordResetSuccessfully();

            var emailSendInput = new EmailSendInput()
            {
                Name = "",
                Email = input.Email,
                Body = body,
                BodyHtml = true,
                Subject = "Construa App - Alteração de Senha"
            };
            _logger.LogInformation($"Call SendEmailAsync in  {nameof(ResetPasswordAsync)} for email: {input.Email}");
            _emailSendService.SendEmailAsync(emailSendInput, new EmailConfiguration());

            var login = await LoginAsync(new UserLoginInput() { Email = input.Email, Password = input.Password });

            return login;
        }

        public async Task<UserControlAccessVOViewModel> GetControlAccessAsync(int userId)
        {
            var result = await _userDomainService.GetControlAccessAsync(userId);
            var user = await _userDomainService.SelectByIdAsync(userId);
            var payments = await _userPaymentMethodDomainService.SelectFilterAsync(x => x.Active.Equals(1) && x.UserId.Equals(user.Id));
            var viewModel = new UserControlAccessVOViewModel(result, user, payments.FirstOrDefault());
            return viewModel;
        }

        public async Task<UserControlAccessVOViewModel> UpdateAsync(int userId, UserUpdateInput input)
        {

            _logger.LogInformation($"Init update user {nameof(UpdateAsync)}");
            if (!input.IsValid())
            {
                var userViewModel = _mapper.Map<UserControlAccessVOViewModel>(input);
                NotifyErrorsAndValidation(_notification.EmptyPositions(), userViewModel);
                _logger.LogWarning($"Init update user param invalid {nameof(UpdateAsync)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
            }

            var user = await _userDomainService.SelectByIdAsync(userId);
            if (user == null)
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Usuário não encontrado.");
                _logger.LogWarning($"User not found {nameof(LoginAsync)} for id: {userId}");
                return default;
            }

            var userToUpdate = new User(input.Name, user.Email, user.Cpf, user.Rg, user.BirthDate, user.Password, user.IsAdmin, user.Status,
                input.PhoneNumber1, input.PhoneNumber2, input.Address, input.AddressNumber, input.Neighborhood, input.AddressComplement,
                input.ZipCode, input.City, input.State, user.TokenNotification, user.IuguCustomerId, input.WebSite, input.ActArea, input.Avatar, input.Company, user.CreatedAt, DateTime.Now);
            userToUpdate.Id = userId;
            var update = await _userDomainService.UpdateAsync(userToUpdate);

            if (update == null)
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Falha na atualização, tente novamente.");
                _logger.LogError($"Fail to update user {nameof(LoginAsync)} for id: {userId}");
                return default;
            }

            var updateAreas = await _userDomainService.UpdateAreasAsync(userId, input.AreasIds);
            var getUserProfile = await GetControlAccessAsync(userId);
            return getUserProfile;
        }

        public async Task<UserViewModel> UpdatePasswordAsync(int userId, UserUpdatePasswordInput input)
        {
            var userViewModel = new UserViewModel();
            _logger.LogInformation($"Init {nameof(UpdatePasswordAsync)}");
            if (!input.IsValid())
            {
                userViewModel.ValidationResult = input.ValidationResult;
                NotifyErrorsAndValidation(_notification.EmptyPositions(), userViewModel);
                _logger.LogWarning($"Update password invalid {nameof(ResetPasswordAsync)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
            }

            var user = await _userDomainService.SelectByIdAsync(userId);

            var password = BCrypt.Net.BCrypt.HashPassword(input.Password, BCrypt.Net.BCrypt.GenerateSalt(), false,
                HashType.SHA256);

            var userToUpdate = new User(user.Name, user.Email, user.Cpf, user.Rg, user.BirthDate, password, user.IsAdmin, user.Status,
                user.PhoneNumber1, user.PhoneNumber2, user.Address, user.AddressNumber, user.Neighborhood, user.AddressComplement,
                user.ZipCode, user.City, user.State, user.TokenNotification, user.IuguCustomerId, user.WebSite, user.ActArea, user.Avatar, user.Company, user.CreatedAt, DateTime.Now);
            userToUpdate.Id = userId;
            var update = await _userDomainService.UpdateAsync(userToUpdate);

            if (update == null)
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Falha na atualização, tente novamente.");
                _logger.LogError($"Fail to update user password{nameof(LoginAsync)} for id: {userId}");
                return default;
            }

            var body = PasswordResetMobileSendEmailSuccessfully.FormatEmailSendPasswordResetSuccessfully();

            var emailSendInput = new EmailSendInput()
            {
                Name = "",
                Email = user.Email,
                Body = body,
                BodyHtml = true,
                Subject = "Construa App - Alteração de Senha"
            };
            _logger.LogInformation($"Call SendEmailAsync in  {nameof(ResetPasswordAsync)} for email: {user.Email}");
            _emailSendService.SendEmailAsync(emailSendInput, new EmailConfiguration());

            var login = await LoginAsync(new UserLoginInput() { Email = user.Email, Password = input.Password });

            return login;
        }
    }
}
