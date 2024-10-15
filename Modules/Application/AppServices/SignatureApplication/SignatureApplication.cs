using Application.AppServices.SignatureApplication.Input;
using Application.AppServices.SignatureApplication.ViewModels;
using Application.Emails.User;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enum;
using Domain.Input.Fovea;
using Domain.Input.Iugu;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Email;
using Infra.CrossCutting.Email.Interfaces;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.AppServices.SignatureApplication
{
    public class SignatureApplication : BaseValidationService, ISignatureApplication
    {
        private readonly ISmartNotification _notification;
        private readonly ISignatureDomainService _signatureDomainService;
        private readonly IUserPaymentMethodDomainService _userPaymentMethodDomainService;
        private readonly IUserDomainService _userDomainService;
        private readonly IPlanDomainService _planDomainService;
        private readonly IEmailSendService _emailSendService;
        private readonly IWebhookDomainService _webhookDomainService;
        private readonly IMapper _mapper;
        ILogger<SignatureApplication> _logger;

        public SignatureApplication(ISignatureDomainService signatureDomainService, ISmartNotification notification, IMapper mapper, ILogger<SignatureApplication> logger, IUserDomainService userDomainService, IPlanDomainService planDomainService, IEmailSendService emailSendService, IWebhookDomainService webhookDomainService, IUserPaymentMethodDomainService userPaymentMethodDomainService) : base(notification)
        {
            _signatureDomainService = signatureDomainService;
            _userDomainService = userDomainService;
            _planDomainService = planDomainService;
            _webhookDomainService = webhookDomainService;
            _emailSendService = emailSendService;
            _notification = notification;
            _mapper = mapper;
            _logger = logger;
            _userPaymentMethodDomainService = userPaymentMethodDomainService;
        }

        public async Task<bool> ChangePaymentMethodAsync(SignatureInput input)
        {
            return await RegisterPaymentMethod(input, null);
        }

        public async Task<UserPlans> ProcessIuguWebhookAsync(IuguWebhookInput input)
        {
            _logger.LogInformation("ProcessIuguWebhookAsync initialized at {date} with input parameter {@param}", DateTime.UtcNow, input);
            UserPlans userPlan = null;
            try
            {
                string jsonIuguWebhook = JsonSerializer.Serialize<IuguWebhookInput>(input);
                Webhook webhook = new Webhook()
                {
                    Partner = "iugu",
                    Type = "webhook",
                    Subtype = input.Event,
                    Content = jsonIuguWebhook
                };

                var insertResult = await _webhookDomainService.InsertAsync(webhook);
                _logger.LogInformation("ProcessIuguWebhookAsync inserted successfully Webhook Iugu Event at {date} with result {@param}", DateTime.UtcNow, JsonSerializer.Serialize<Webhook>(insertResult));

                if ("invoice.status_changed" == input.Event)
                {
                    userPlan = await ProcessIuguStatusChangedWebhookAsync(input.Data);
                }

                if ("subscription.renewed" == input.Event)
                {
                    userPlan = await ProcessIuguRenewedWebhookAsync(input.Data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ProcessIuguWebhookAsync failed trying to process iugu invoice.status_changed webhook at {@date} with result {@param}", DateTime.UtcNow, ex.Message);
            }
            return userPlan;
        }

        public async Task<UserPlans> ProcessIuguStatusChangedWebhookAsync(IuguWebhookDataInput dataInput)
        {
            UserPlans userPlan = await _signatureDomainService.GetUserPlanByPartnerSignatureId(dataInput.SubscriptionId);
            if (null == userPlan)
            {
                _logger.LogError("ProcessIuguStatusChangedWebhookAsync UserPlan not found when process iugu invoice.status_changed webhook at {date} with subscription id {SubscriptionId}", DateTime.UtcNow, dataInput.SubscriptionId);
                return default;
            }

            if ("paid" == dataInput.Status)
            {
                if ((sbyte)BoolEnum.NO == userPlan.StatusPayment)
                    {
                    userPlan.StatusPayment = 1;
                    userPlan = await _signatureDomainService.UpdateAsync(userPlan);
                    User user = await _userDomainService.GetUserAsync(userPlan.UserId);
                    _logger.LogInformation("ProcessIuguStatusChangedWebhookAsync UserPlan updated to paid after process iugu invoice.status_changed webhook at {date} with subscription id {SubscriptionId}", DateTime.UtcNow, dataInput.SubscriptionId);
                    SendWelcomeEmail(user);
                    }
                }
            else
            {
                userPlan.StatusPayment = 0;
                userPlan = await _signatureDomainService.UpdateAsync(userPlan);
                _logger.LogInformation("ProcessIuguStatusChangedWebhookAsync UserPlan updated to not paid after process iugu invoice.status_changed webhook at {date} with subscription id {SubscriptionId}", DateTime.UtcNow, dataInput.SubscriptionId);
            }

            return userPlan;
        }

        private void SendWelcomeEmail(User user)
            {
            var body = WelcomeUserPlan.FormatEmailWelcomePremiumPlan(user.Name);

            var emailSendInput = new EmailSendInput()
                {
                Name = user.Name,
                Email = user.Email,
                Body = body,
                BodyHtml = true,
                Subject = "Bem-vindo(a) ao Construa App Pro"
                };

            _emailSendService.SendEmailAsync(emailSendInput, new EmailConfiguration());
            }

        private void SendNewSignatureNotificationEmail(User user, string platform)
        {
            var body = WelcomeUserPlan.FormatEmailWelcomePremiumPlan(user.Name);

            var emailSendInput = new EmailSendInput()
            {
                Name = "contato@construa.app",
                Email = "contato@construa.app",
                Body = "O cliente " + user.Name + " acabou de realizar uma assinatura premium temporária com e-mail "+ user.Email + " através da plataforma " + platform,
                BodyHtml = false,
                Subject = "Nova assinatura do Construa App realizada"
            };

            _emailSendService.SendEmailAsync(emailSendInput, new EmailConfiguration());
        }

        public async Task<UserPlans> ProcessIuguRenewedWebhookAsync(IuguWebhookDataInput dataInput)
        {
            UserPlans userPlan = await _signatureDomainService.GetUserPlanByPartnerSignatureId(dataInput.Id);
            if (null == userPlan)
            {
                _logger.LogError("ProcessIuguRenewedWebhookAsync UserPlan not found when process iugu subscription.renewed webhook at {date} with subscription id {SubscriptionId}", DateTime.UtcNow, dataInput.Id);
                return default;
            }
            if ((sbyte)BoolEnum.NO == userPlan.StatusPayment)
            {
                _logger.LogWarning("ProcessIuguRenewedWebhookAsync UserPlan found but not paid when process iugu subscription.renewed webhook at {date} with subscription id {SubscriptionId}", DateTime.UtcNow, dataInput.Id);
                return default;
            }

            if (userPlan.CreatedAt.Date.CompareTo(DateTime.Today) < 0)
            {
                User user = await _userDomainService.GetUserAsync(userPlan.UserId);
                Plan plan = await _planDomainService.GetPremiumPlanAsync(userPlan.PlanId);
                User userWithRenewedPlan = await _signatureDomainService.UpdateUserToPremiumPlanAsync(user, plan, dataInput.Id);
                userPlan = await _signatureDomainService.GetUserPlanByPartnerSignatureId(dataInput.Id);
                _logger.LogInformation("ProcessIuguRenewedWebhookAsync UserPlan renewed after process iugu subscription.renewed webhook at {date} with subscription id {SubscriptionId}", DateTime.UtcNow, dataInput.Id);
                return userPlan;
            }
            else
            {
                _logger.LogInformation("ProcessIuguRenewedWebhookAsync UserPlan found but was not renewed because was been created at same day at {date} with subscription id {SubscriptionId}", DateTime.UtcNow, dataInput.Id);
                return default;
            }
        }

        private async Task<bool> RegisterPaymentMethod(SignatureInput signatureInput, User user)
        {

            if (user == null)
            {
                user = await _userDomainService.GetUserAsync(signatureInput.UserId);
            }

            if (user == null)
            {
                _logger.LogError("Fail get user for register payment method at {@date}", DateTime.UtcNow);
                _notification.NewNotificationBadRequest(new string[] {}, $"Falha ao criar o método de pagamento do usuário ID: {signatureInput.UserId} pois não foi possível obter os dados do usuario no sistema");
                return false;
            }

            Domain.Input.IuguInput iuguInput = new Domain.Input.IuguInput()
            {
                UrlBase = signatureInput.UrlBase,
                Token = signatureInput.Token,
            };

            PaymentMethod paymentMethod = await _signatureDomainService.PostUserPaymentMethodAsync(user.IuguCustomerId, signatureInput.PaymentMethodToken, iuguInput);
            string iuguCustomerPaymenthMethodId = paymentMethod.Id;
            if (String.IsNullOrEmpty(iuguCustomerPaymenthMethodId))
            {
                _logger.LogError("PostSignatureAsync failed trying to create Iugu Payment Method at {@date}", DateTime.UtcNow);
                _notification.NewNotificationBadRequest(new string[] { user.Name, paymentMethod.GetErrors() }, "Falha ao criar o método de pagamento do usuário '{0}' na Iugu. Retorno: {1}");
                return false;
            }

            var userPaymentMethod = new UserPaymentMethod()
            {
                TransactionId = paymentMethod.Id,
                Channel = (int)ChannelPaymentEnum.Iugu,
                Type = (int)PaymentTypeMethodEnum.CreditCard,
                Token = signatureInput.PaymentMethodToken,
                Description = paymentMethod.Description,
                Flag = signatureInput.Flag,
                LastFourDigits = signatureInput.LastFourDigits,
                Active = (sbyte)BoolEnum.YES,
                UserId = user.Id,
                CustomerId = user.IuguCustomerId
            };

            var insertUserPaymentMethod = await _userPaymentMethodDomainService.InsertAndInactiveAsync(userPaymentMethod);
            if (insertUserPaymentMethod == null)
            {
                _logger.LogError("User Payment Method InsertAndInactiveAsync failed trying to create Iugu Payment Method in database construa at {@date}", DateTime.UtcNow);
            }

            var body = WelcomeUserPlan.FormatEmailChangeUserPaymentMethod(user.Name);

            var emailSendInput = new EmailSendInput()
            {
                Name = user.Name,
                Email = user.Email,
                Body = body,
                BodyHtml = true,
                Subject = "Alteração de método de pagamento Construa App Pro"
            };

            _emailSendService.SendEmailAsync(emailSendInput, new EmailConfiguration());

            return true;
        }

        public async Task<SignatureViewModel> PostSignatureAsync(SignatureInput signatureInput)
        {
            _logger.LogInformation("PostSignatureAsync initialized at {date} with input parameter {@param}", DateTime.UtcNow, signatureInput);
            Signature iuguSignature = new Signature();
            SignatureViewModel signatureViewModel = new SignatureViewModel();
            try
            {
                User user = await _userDomainService.GetUserAsync(signatureInput.UserId);
                Plan plan = await _planDomainService.GetPremiumPlanAsync(signatureInput.PlanId);
                Domain.Input.IuguInput iuguInput = new Domain.Input.IuguInput()
                {
                    UrlBase = signatureInput.UrlBase,
                    Token = signatureInput.Token,
                };

                string iuguCustomerId = user.IuguCustomerId;
                if (String.IsNullOrEmpty(iuguCustomerId))
                {
                    Customer customer = await _signatureDomainService.PostUserAsync(user, iuguInput);
                    iuguCustomerId = customer.Id;
                    if (!String.IsNullOrEmpty(iuguCustomerId))
                    {
                        user.IuguCustomerId = iuguCustomerId;
                        user = await _userDomainService.UpdateAsync(user);
                    }
                    else
                    {
                        _logger.LogError("PostSignatureAsync failed trying to create Iugu Customer at {@date}", DateTime.UtcNow);
                        _notification.NewNotificationBadRequest(new string[] { user.Name, customer.getErrors() }, "Falha ao criar o usuário '{0}' na Iugu. Retorno: {1}");
                        return default;
                    }
                }

                var registerPaymentMethod = await RegisterPaymentMethod(signatureInput, user);

                if (!registerPaymentMethod)
                {
                    return default;
                }

                iuguSignature = await _signatureDomainService.PostSignatureMethodAsync(plan, iuguCustomerId, iuguInput);
                signatureViewModel.IuguSignature = iuguSignature;
                if (!String.IsNullOrEmpty(iuguSignature.Id))
                {
                    user = await _signatureDomainService.UpdateUserToPremiumPlanAsync(user, plan, iuguSignature.Id);
                    if (iuguSignature.RecentInvoices.First().Status != "paid")
                    {
                        UserPlans userPlanPremium = await _signatureDomainService.GetUserPlanByPartnerSignatureId(iuguSignature.Id);
                        userPlanPremium.StatusPayment = (sbyte)BoolEnum.NO;
                        userPlanPremium = await _signatureDomainService.UpdateAsync(userPlanPremium);
                        _logger.LogInformation("PostSignatureAsync user not updated to premium plan after create Iugu Signature at {date} because transaction is'nt payed", DateTime.UtcNow);
                        _notification.NewNotificationBadRequest(new string[] { user.Name, iuguSignature.getErrors() }, "Falha no pagamento da assinatura do usuário '{0}' na Iugu. Retorno: {1}");
                    }
                    else
                    {
                        _logger.LogInformation("ProcessPagseguroNotificationAsync User updated to Premium plan after process iugu payment at {date}", DateTime.UtcNow);

                        var body = WelcomeUserPlan.FormatEmailWelcomePremiumPlan(user.Name);

                        var emailSendInput = new EmailSendInput()
                        {
                            Name = user.Name,
                            Email = user.Email,
                            Body = body,
                            BodyHtml = true,
                            Subject = "Bem-vindo(a) ao Construa App Pro"
                        };

                        _emailSendService.SendEmailAsync(emailSendInput, new EmailConfiguration());
                    }
                    return signatureViewModel;
                }
                else if (String.IsNullOrEmpty(iuguSignature.Id))
                {
                    _logger.LogError("PostSignatureAsync failed trying to create Iugu Signature Id at {@date}", DateTime.UtcNow);
                    _notification.NewNotificationBadRequest(new string[] { user.Name, iuguSignature.getErrors() }, "Falha na criação da assinatura do usuário '{0}' na Iugu. Retorno: {1}");
                    return default;
                }
                else
                {
                    _logger.LogError("PostSignatureAsync failed trying to create Iugu Signature Id at {@date}", DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("PostSignatureAsync failed trying to create Iugu Signature at {@date} with result {@param}", DateTime.UtcNow, ex.Message);
            }
            finally
            {
                _logger.LogInformation("PostSignatureAsync Iugu Signature created at {@date} with id {@iuguSignatureId}", DateTime.UtcNow, iuguSignature.Id);
            }

            return signatureViewModel;
        }

        public async Task<SignatureViewModel> PostChangeStatusSignatureAsync(SignatureInput signatureInput, string actionStatus)
        {
            _logger.LogInformation("PostChangeStatusSignatureAsync initialized at {date} with input parameter {@param} and status {status}", DateTime.UtcNow, signatureInput, actionStatus);
            if ("active" != actionStatus && "suspend" != actionStatus)
            {
                _logger.LogError("PostChangeStatusSignatureAsync invalid action status {status} at {date} with input parameter {@param}", actionStatus, DateTime.UtcNow, signatureInput);
                _notification.NewNotificationBadRequest(new string[] { actionStatus }, "O status '{0}' é inválido.");
                return default;
            }

            SignatureViewModel signatureViewModel = new SignatureViewModel();
            UserPlans userPlan = await _signatureDomainService.GetUserPlanWithIuguIdByUserId(signatureInput.UserId);
            if (null == userPlan)
            {
                _logger.LogError("PostChangeStatusSignatureAsync UserPlan not found when process iugu subscription.renewed webhook at {date} with input parameter {@param} and status {status}", DateTime.UtcNow, signatureInput, actionStatus);
                return default;
            }

            Domain.Input.IuguInput iuguInput = new Domain.Input.IuguInput()
            {
                UrlBase = signatureInput.UrlBase,
                Token = signatureInput.Token,
            };

            signatureViewModel.IuguSignature = await _signatureDomainService.PostChangeStatusSignatureAsync(iuguInput, userPlan.IuguSignatureId, actionStatus);
            _logger.LogInformation("ProcessIuguRenewedWebhookAsync UserPlan renewed after process iugu subscription.renewed webhook at {date} with input parameter {@param} and status {status}", DateTime.UtcNow, signatureInput, actionStatus);
            return signatureViewModel;
        }


        public async Task<UserPlans> ProcessFoveaWebhookAsync(FoveaWebhookInput input, string environmentName)
            {
            _logger.LogInformation("ProcessFoveaWebhookAsync initialized at {date} with input parameter {@param}", DateTime.UtcNow, input);

            var compraMaisRecente = input.Purchases.OrderByDescending(x => x.Value.PurchaseDate).FirstOrDefault();
            bool isFoveaSandboxWebhook = compraMaisRecente.Value.Sandbox;
            string jsonFoveaWebhook = JsonSerializer.Serialize<FoveaWebhookInput>(input);
            if ("AzurePrd" == environmentName && isFoveaSandboxWebhook)
            {
                 await _signatureDomainService.PostFoveaSandobxWebhookToDevelopment(jsonFoveaWebhook);
                _logger.LogInformation("ProcessFoveaWebhookAsync send webhook to development environment at {date} with input parameter {@param}", DateTime.UtcNow, input);
                return default;
            }

            try
                {
                Webhook webhook = new Webhook()
                    {
                    Partner = "Fovea",
                    Type = "webhook",
                    Subtype = input.Type,
                    Content = jsonFoveaWebhook
                    };

                var insertResult = await _webhookDomainService.InsertAsync(webhook);
                _logger.LogInformation("ProcessFoveaWebhookAsync inserted successfully Webhook Fovea Event at {date} with result {@param}", DateTime.UtcNow, JsonSerializer.Serialize<Webhook>(insertResult));

                if ("purchases.updated" == input.Type && String.IsNullOrEmpty(compraMaisRecente.Value.CancelationReason))
                    {
                    User user = new User() { Email = input.ApplicationUsername };
                    user = await _userDomainService.GetUserByEmailAsync(user);
                    if (null == user)
                        {
                        _notification.NewNotificationBadRequest(new string[] { input.ApplicationUsername }, "O e-mail '{0}' não está cadastrado em nosso sistema.");
                        _logger.LogInformation("ProcessFoveaWebhookAsync could not found User with email {ApplicationUsername} at process fovea webhook at {date} with ", input.ApplicationUsername, System.DateTime.UtcNow);
                        return default;
                        }
                    foreach (var purchase in input.Purchases)
                        {
                        Plan plan = await _planDomainService.GetPlanByPartnerId(purchase.Key);
                        if(null == plan)
                            {
                            _notification.NewNotificationBadRequest(new string[] { purchase.Key }, "Não foi possível encontrar um plano com a chave '{0}'");
                            _logger.LogInformation("ProcessFoveaWebhookAsync could not found Plan with key {purchaseKey} at process fovea webhook at {date} with ", purchase.Key, System.DateTime.UtcNow);
                            return default;
                            }
                        UserPlans userPlan = await _signatureDomainService.GetUserPlanByPartnerSignatureId(purchase.Value.PurchaseId);
                        //UserPlans userPlan = await _signatureDomainService.GetUserPlan(user, plan);

                        if (purchase.Value.IsExpired && null != userPlan && (sbyte)BoolEnum.NO == userPlan.Deleted)
                            {
                            userPlan.Deleted = 1;
                            userPlan = await _signatureDomainService.UpdateAsync(userPlan);
                            _logger.LogInformation("ProcessFoveaWebhookAsync UserPlan updated to deleted after process fovea webhook at {date} with cancellation reason {cancellationReason}"
                                , DateTime.UtcNow, purchase.Value.CancelationReason);
                            }
                        else if (!purchase.Value.IsExpired)
                            {
                            if (null == userPlan || userPlan.CreatedAt.Date.CompareTo(DateTime.Today) < 0)
                                {
                                User userWithPremiumPlan = await _signatureDomainService.UpdateUserToPremiumPlanAsync(user, plan, purchase.Value.PurchaseId);
                                userPlan = await _signatureDomainService.GetUserPlanByPartnerSignatureId(purchase.Value.PurchaseId);
                                _logger.LogInformation("ProcessFoveaWebhookAsync UserPlan created after process fovea webhook at {date} with purchase id {PurchaseId}", DateTime.UtcNow, purchase.Value.PurchaseId);
                                return userPlan;
                                }
                            else
                                {
                                _logger.LogInformation("ProcessFoveaWebhookAsync UserPlan found but was not created or renewed because was been created at same day at {date} purchase id {PurchaseId}", DateTime.UtcNow, purchase.Value.PurchaseId);
                                return default;
                                }
                            }
                        }
                    }
                }
            catch (Exception ex)
                {
                _logger.LogError("ProcessFoveaWebhookAsync failed trying to process fovea webhook at {@date} with result {@param}", DateTime.UtcNow, ex.Message);
                }
            return null;
            }

        public async Task<UserPlans> CreateTempSignatureAsync(SignatureInput input, string platform)
            {
                User user = await _userDomainService.GetUserAsync(input.UserId);
                Domain.Input.IuguInput iuguInput = new Domain.Input.IuguInput()
                {
                    UrlBase = input.UrlBase,
                    Token = input.Token,
                };
                FoveaReceipt foveaReceipts = await _signatureDomainService.GetFoveaReceipts(iuguInput, user);
                bool hasValidFoveaReceipt = await HasValidFoveaReceipt(foveaReceipts);
                if (!hasValidFoveaReceipt)
                {
                    return default;
                }
                Plan plan = await _planDomainService.GetPremiumPlanAsync(2);
                string signatureKey = string.Concat(platform, ":", "temp:", DateTime.Now.ToString("ddMMyyyhhmmss"));
                UserPlans userWithTempPremiumPlan = await _signatureDomainService.UpdateUserToTempPremiumPlanAsync(user, plan, signatureKey);
                SendNewSignatureNotificationEmail(user, platform);

                return userWithTempPremiumPlan;
            }

        public async Task<bool> HasValidFoveaReceipt(FoveaReceipt foveaReceipt)
        {
            if (!foveaReceipt.Purchases.Any())
            {
                return false;
            }
            //TODO filtrar apenas compras sandbox quando em dev e apenas compras que não são do sandbox quando em prod

            var compraMaisRecente = foveaReceipt.Purchases.OrderByDescending(x => x.Value.PurchaseDate).FirstOrDefault();
            if (!compraMaisRecente.Value.PurchaseDate.Value.Date.Equals(DateTime.Today.Date))
            {
                return false;
            }

            return true;
        }
        }
}