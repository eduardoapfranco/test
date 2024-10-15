using Application.AppServices.OrderApplication.Input;
using Application.AppServices.OrderApplication.Input.Pagseguro;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Application.Emails.User;
using Infra.CrossCutting.Email;
using Infra.CrossCutting.Email.Interfaces;

namespace Application.AppServices.OrderApplication
{
    public class OrderApplication : BaseValidationService, IOrderApplication
    {
        private readonly ISmartNotification _notification;
        private readonly IWebhookDomainService _webhookDomainService;
        private readonly IUserDomainService _userDomainService;
        private readonly IPlanDomainService _planDomainService;
        private readonly IEmailSendService _emailSendService;
        private readonly ISignatureDomainService _signatureDomainService;
        private readonly IMapper _mapper;
        ILogger<OrderApplication> _logger;

        public OrderApplication(IWebhookDomainService webhookDomainService, ISmartNotification notification, IMapper mapper, ILogger<OrderApplication> logger, IUserDomainService userDomainService, IPlanDomainService planDomainService, IEmailSendService emailSendService, ISignatureDomainService signatureDomainService) : base(notification)
        {
            _webhookDomainService = webhookDomainService;
            _userDomainService = userDomainService;
            _planDomainService = planDomainService;
            _emailSendService = emailSendService;
            _signatureDomainService = signatureDomainService;
            _notification = notification;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> GetPagseguroOrderFormAsync(int userId, int planId, WebhookPagSeguroNotificationInput webhookPagseguroNotificationInput)
        {
            _logger.LogInformation("GetPagseguroOrderFormAsync initialized at {date} with userId {@userId}, planId {@planId} and input parameter {@param}", DateTime.UtcNow, userId, planId, webhookPagseguroNotificationInput);
            string orderFormUrl = String.Empty;
            try
            {
                User user = await _userDomainService.GetUserAsync(userId);
                Plan plan = await _planDomainService.GetPremiumPlanAsync(planId);
                Domain.Input.PagSeguroNotificationInput pagseguroNotificationInput = new Domain.Input.PagSeguroNotificationInput()
                {
                    UrlBase = webhookPagseguroNotificationInput.UrlBase,
                    UrlBaseOrderForm = webhookPagseguroNotificationInput.UrlBaseOrderForm,
                    Token = webhookPagseguroNotificationInput.Token,
                    Email = webhookPagseguroNotificationInput.Email
                };

                Checkout pagseguroCheckout = Checkout.Load(user, plan, webhookPagseguroNotificationInput);

                string orderCode = await _webhookDomainService.GetPagseguroCheckoutCodeAsync(pagseguroCheckout.ToFormParameter(), pagseguroNotificationInput);
                if (String.IsNullOrEmpty(orderCode))
                {
                    return null;
                }
                orderFormUrl = String.Concat(webhookPagseguroNotificationInput.UrlBaseOrderForm, "/v2/checkout/payment.html?code=", orderCode);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPagseguroOrderFormAsync failed trying to create orderFormUrl at {@date} with result {@param}", DateTime.UtcNow, ex.Message);
            }
            finally
            {
                _logger.LogInformation("GetPagseguroOrderFormAsync returned url {@orderFormUrl} at {@date}", orderFormUrl, DateTime.UtcNow);
            }

            return orderFormUrl;
        }


        public async Task<bool> ProcessPagseguroNotificationAsync(WebhookPagSeguroNotificationInput webhookPagseguroNotificationInput)
        {
            ILogEventEnricher[] enrichers =
               {
                new PropertyEnricher("NotificationCode", webhookPagseguroNotificationInput.NotificationCode),
                new PropertyEnricher("NotificationType", webhookPagseguroNotificationInput.NotificationType)
                };

            using (LogContext.Push(enrichers))
            {
                _logger.LogInformation("ProcessPagseguroNotificationAsync initialized at {date} with parameter {@param}", DateTime.UtcNow, webhookPagseguroNotificationInput);
                Domain.Input.PagSeguroNotificationInput pagseguroNotificationInput = new Domain.Input.PagSeguroNotificationInput()
                {
                    UrlBase = webhookPagseguroNotificationInput.UrlBase,
                    Token = webhookPagseguroNotificationInput.Token,
                    Email = webhookPagseguroNotificationInput.Email,
                    NotificationCode = webhookPagseguroNotificationInput.NotificationCode
                };

                string fullNotification = await _webhookDomainService.GetFullPagseguroNotificationAsync(pagseguroNotificationInput);
                if (String.IsNullOrEmpty(fullNotification))
                {
                    return false;
                }
                Webhook webhookNotication = new Webhook()
                {
                    Partner = "pagseguro",
                    Type = "notification",
                    Subtype = "transaction",
                    Content = fullNotification
                };

                var insertResult = await _webhookDomainService.InsertAsync(webhookNotication);
                _logger.LogInformation("ProcessPagseguroNotificationAsync inserted successfully Webhook Pagseguro Notification at {date} with result {@param}", DateTime.UtcNow, JsonConvert.SerializeObject(insertResult));
                PagSeguroTransactionInput pagSeguroTransaction = new PagSeguroTransactionInput();
                ILogEventEnricher[] enricherTransaction;
                try
                {
                    pagSeguroTransaction = PagSeguroTransactionInput.Load(fullNotification);
                    enricherTransaction = new ILogEventEnricher[]
                       {
                        new PropertyEnricher("UserEmail", pagSeguroTransaction.Sender.Email),
                        new PropertyEnricher("PlanId", pagSeguroTransaction.Items.FirstOrDefault().Id),
                        new PropertyEnricher("TransactionStatus", pagSeguroTransaction.Status)
                        };

                    using (LogContext.Push(enricherTransaction))
                    {

                        _logger.LogInformation($"ProcessPagseguroNotificationAsync receive status pagseguro {pagSeguroTransaction.Status} in {DateTime.UtcNow} for user {pagSeguroTransaction.Sender.Email}");
                        if (TransactionStatus.AGUARDANDO_PAGAMENTO == pagSeguroTransaction.Status || TransactionStatus.EM_ANALISE == pagSeguroTransaction.Status)
                        {
                            User user = new User()
                            {
                                Email = pagSeguroTransaction.Sender.Email
                            };
                            user = await _userDomainService.GetUserByEmailAsync(user);

                            var body = WelcomeUserPlan.FormatEmailRequestOrderPlan(user.Name);

                            var emailSendInput = new EmailSendInput()
                            {
                                Name = user.Name,
                                Email = user.Email,
                                Body = body,
                                BodyHtml = true,
                                Subject = "Início de Assinatura do Construa App Pro"
                            };

                            _logger.LogInformation($"ProcessPagseguroNotificationAsync init plan premium at {DateTime.UtcNow} for user {user.Email}");

                            _emailSendService.SendEmailAsync(emailSendInput, new EmailConfiguration());
                        }

                        if (TransactionStatus.PAGA == pagSeguroTransaction.Status)
                        {
                            User user = new User()
                            {
                                Email = pagSeguroTransaction.Sender.Email
                            };
                            Plan plan = await _planDomainService.GetPremiumPlanAsync(pagSeguroTransaction.Items.FirstOrDefault().Id);
                            user = await _userDomainService.GetUserByEmailAsync(user);
                            user = await _signatureDomainService.UpdateUserToPremiumPlanAsync(user, plan);
                            _logger.LogInformation("ProcessPagseguroNotificationAsync User updated to Premium plan after process pagseguro notification at {date}", DateTime.UtcNow);

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
                        else
                        {
                            _logger.LogInformation("ProcessPagseguroNotificationAsync User not updated to premium plan after process pagseguro notification at {date} because transaction is'nt payed", DateTime.UtcNow);
                        }
                    }
                }
                catch (XmlException ex)
                {
                    _logger.LogError("ProcessPagseguroNotificationAsync failed trying to serialize webhook Pagseguro notification at {date} with result {@param}", DateTime.UtcNow, ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError("ProcessPagseguroNotificationAsync failed trying to update user to Premium plan at {date} with result {@param}", DateTime.UtcNow, ex.Message);
                }
                finally
                {
                    _logger.LogInformation("ProcessPagseguroNotificationAsync finished at {@date}", DateTime.UtcNow);
                }
                if (null == insertResult)
                {
                    return false;
                }

                return !String.IsNullOrEmpty(insertResult.Id.ToString());
            }
        }
    }
}