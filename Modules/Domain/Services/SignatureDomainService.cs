using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Domain.Entities;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.Domain.Services;
using Domain.Interfaces.Repositories;
using System.Threading.Tasks;
using System.Net.Http;
using Domain.Input;
using System;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Context;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Domain.Input.Iugu;
using System.Linq;
using Domain.Enum;
using Newtonsoft.Json;
using Domain.Input.Fovea;

namespace Domain.Services
{
    public class SignatureDomainService : DomainService<UserPlans, int, IUnitOfWork>, ISignatureDomainService
    {
        private readonly IUserPlansRepository _userPlansRepository;
        private ISmartNotification _notification;
        public HttpClient _httpClient = new HttpClient();
        ILogger<SignatureDomainService> _logger;
        public SignatureDomainService(
           IUserPlansRepository userPlansRepository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<SignatureDomainService> logger
       ) : base(userPlansRepository, unitOfWork, messageHandler)
        {
            _userPlansRepository = userPlansRepository;
            _notification = notification;
            _logger = logger;
        }

        public async Task<Customer> PostUserAsync(User user, IuguInput input)
        {
            ILogEventEnricher[] enrichers =
                {
                new PropertyEnricher("UrlBase", input.UrlBase),
                new PropertyEnricher("Token", input.Token),
                new PropertyEnricher("UserEmail", user.Email),
                };

            using (LogContext.Push(enrichers))
            {
                _logger.LogInformation("PostUserAsync initialized at {date} with  user {user} and parameter {@param}", DateTime.UtcNow, user, input);
                Customer iuguCustomer = new Customer()
                {
                    Name = user.Name,
                    Email = user.Email
                };
                Customer customer = await PostIugu<Customer>(iuguCustomer, "/v1/customers", input);
                return customer;
            }
        }

        public async Task<PaymentMethod> PostUserPaymentMethodAsync(string iuguCustomerId, string paymentMethodToken, IuguInput input)
        {
            ILogEventEnricher[] enrichers =
                {
                new PropertyEnricher("UrlBase", input.UrlBase),
                new PropertyEnricher("Token", input.Token),
                new PropertyEnricher("iuguCustomerId", iuguCustomerId),
                new PropertyEnricher("PaymentMethodToken", paymentMethodToken),
                };

            using (LogContext.Push(enrichers))
            {
                _logger.LogInformation("PostUserAsync initialized at {date} with  user {user} and parameter {@param}", DateTime.UtcNow, iuguCustomerId, input);
                PaymentMethod iuguCustomerPaymentMethod = new PaymentMethod()
                {
                    Token = paymentMethodToken,
                    Description = "Cartão de crédito",
                    SetAsDefault = "true"
                };
                string endpoint = String.Concat("/v1/customers/", iuguCustomerId, "/payment_methods");
                PaymentMethod paymentMethod = await PostIugu<PaymentMethod>(iuguCustomerPaymentMethod, endpoint, input);
                return paymentMethod;
            }
        }

        public async Task<Signature> PostSignatureMethodAsync(Plan plan, string iuguCustomerId, IuguInput input)
        {
            ILogEventEnricher[] enrichers =
                {
                new PropertyEnricher("UrlBase", input.UrlBase),
                new PropertyEnricher("Token", input.Token),
                new PropertyEnricher("planId", plan.Id),
                new PropertyEnricher("iuguCustomerId", iuguCustomerId),
                };

            using (LogContext.Push(enrichers))
            {
                _logger.LogInformation("PostUserAsync initialized at {date} with plan {planId}, iuguCustomerId {iuguCustomerId} and parameter {@param}", DateTime.UtcNow, plan.Id, iuguCustomerId, input);
                Signature iuguSignature = new Signature()
                {
                    PlanIdentifier = plan.Id.ToString(),
                    CustomerId = iuguCustomerId
                };

                Signature signature = await PostIugu<Signature>(iuguSignature, "/v1/subscriptions", input);
                return signature;
            }
        }

        public async Task<Signature> PostChangeStatusSignatureAsync(IuguInput input, string iuguSubscriptionId, string actionStatus)
        {
            string endpoint = String.Concat("/v1/subscriptions/", iuguSubscriptionId, "/", actionStatus);
            Signature signature = await PostIugu<Signature>(null, endpoint, input);
            return signature;
        }

        private async Task<T> PostIugu<T>(T objeto, string endpoint, IuguInput input)
        {
            HttpResponseMessage postResult = new HttpResponseMessage();
            try
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var tokenBase64 = System.Text.Encoding.UTF8.GetBytes(input.Token + ":");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(System.Convert.ToBase64String(tokenBase64));

                string urlPostIugu = String.Concat(input.UrlBase, endpoint);
                var uriPostIugu = new UriBuilder(urlPostIugu);
                StringContent contentRequest = null;
                if (null != objeto)
                {
                    var jsonRequest = JsonConvert.SerializeObject(objeto);
                    contentRequest = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                }

                postResult = await _httpClient.PostAsync(uriPostIugu.ToString(), contentRequest);
                string responseBody = await postResult.Content.ReadAsStringAsync();

                _logger.LogInformation("PostIugu executed at {date} with result {@param}", DateTime.UtcNow, postResult);
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError("PostIugu failed at {date} with error message {error}", DateTime.UtcNow, ex.Message);
            }
            return default(T);
        }

        public async Task<UserPlans> GetUserPlanByPartnerSignatureId(string partnerSignatureId)
        {
            _logger.LogInformation($"GetUserPlanByPartnerSignatureId with partnerSignatureId {partnerSignatureId}");
            var searchUserPlans = await _userPlansRepository.SelectFilterAsync(x => x.IuguSignatureId == partnerSignatureId);
            if (searchUserPlans.Any())
            {
                UserPlans userPlanReturned = searchUserPlans.OrderByDescending(x => x.DueDateAt).FirstOrDefault();
                _logger.LogInformation($"GetUserPlanByPartnerSignatureId with partnerSignatureId {partnerSignatureId} find user plan {userPlanReturned.Id}");
                return userPlanReturned;
            }
            else
            {
                _logger.LogWarning($"GetUserPlanByPartnerSignatureId with partnerSignatureIdl {partnerSignatureId} does'nt find any user plan");
                return null;
            }
        }

        public async Task<UserPlans> GetUserPlan(User user, Plan plan)
            {
            _logger.LogInformation($"GetUserPlan with user {user} and plan {plan}");
            var searchUserPlans = await _userPlansRepository.SelectFilterAsync(x => x.UserId == user.Id && x.PlanId == plan.Id);
            if (searchUserPlans.Any())
                {
                UserPlans userPlanReturned = searchUserPlans.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                _logger.LogInformation($"GetUserPlan with user {user} and plan {plan} find user plan {userPlanReturned.Id}");
                return userPlanReturned;
                }
            else
                {
                _logger.LogWarning($"GetUserPlan with user {user} and plan {plan}  does'nt find any user plan");
                return null;
                }
            }

        public UserPlans GetUserPlanToUpdateUserToPremiumPlan(User user, Plan plan, string partnerSignatureId = null)
        {
            var userPlan = new UserPlans()
            {
                UserId = user.Id,
                PlanId = plan.Id,
                DueDateAt = DateTime.Now.AddDays(plan.PlanType.Days),
                ValueDebit = plan.ValueFinally,
                StatusPayment = (sbyte)BoolEnum.YES,
                Deleted = (sbyte)BoolEnum.NO,
                DueInstallment = 0,
                IuguSignatureId = partnerSignatureId
            };

            return userPlan;
        }

        public async Task<UserPlans> GetUserPlanWithIuguIdByUserId(int userId)
        {
            _logger.LogInformation($"GetUserPlanWithIuguIdByUserId with userId {userId}");
            var searchUserPlans = await _userPlansRepository.SelectFilterAsync(x => x.UserId == userId && !String.IsNullOrEmpty(x.IuguSignatureId));
            if (searchUserPlans.Any())
            {
                UserPlans userPlanReturned = searchUserPlans.OrderByDescending(x => x.DueDateAt).FirstOrDefault();
                _logger.LogInformation($"GetUserPlanWithIuguIdByUserId with userId {userId} find user plan {userPlanReturned}");
                return userPlanReturned;
            }
            else
            {
                _logger.LogWarning($"GetUserPlanWithIuguIdByUserId with userId {userId} does'nt find any user plan");
                return null;
            }
        }

        public async Task<User> UpdateUserToPremiumPlanAsync(User user, Plan plan, string partnerSignatureId = null)
        {
            _logger.LogInformation($"UpdateUserToPremiumPlanAsync init process to update user with email {user.Email} to premium plan {plan.Title}");
            using (_unitOfWork.BeginTransaction())
            {

                var userPlan = GetUserPlanToUpdateUserToPremiumPlan(user, plan, partnerSignatureId);
                var insertUserPlan = await _unitOfWork.UserPlans.InsertAsync(userPlan);
                await _unitOfWork.SaveChangesAsync();

                if (insertUserPlan == null)
                {
                    _notification.NewNotificationBadRequest(_notification.EmptyPositions(),
                        "Falha ao vincular usuário com plano premium");
                    _logger.LogError($"UpdateUserToPremiumPlanAsync failed in process to update user with email {user.Email} to premium plan {plan.Title}");
                    return default;
                }

                Commit();

                return user;
            }
        }

        public async Task<UserPlans> UpdateUserToTempPremiumPlanAsync(User user, Plan plan, string signatureKey = null)
            {
            _logger.LogInformation($"UpdateUserToTempPremiumPlanAsync init process to update user with email {user.Email} to temp premium plan {plan.Title}");
            using (_unitOfWork.BeginTransaction())
                {

                var userPlan = new UserPlans()
                    {
                    UserId = user.Id,
                    PlanId = plan.Id,
                    DueDateAt = DateTime.Now.AddMinutes(5),
                    ValueDebit = 0,
                    StatusPayment = (sbyte)BoolEnum.YES,
                    Deleted = (sbyte)BoolEnum.NO,
                    DueInstallment = 0,
                    IuguSignatureId = signatureKey
                    };
                var insertUserPlan = await _unitOfWork.UserPlans.InsertAsync(userPlan);
                await _unitOfWork.SaveChangesAsync();

                if (insertUserPlan == null)
                    {
                    _notification.NewNotificationBadRequest(_notification.EmptyPositions(),
                        "Falha ao vincular usuário com plano premium temporário");
                    _logger.LogError($"UpdateUserToPremiumPlanAsync failed in process to update user with email {user.Email} to temp premium plan {plan.Title}");
                    return default;
                    }

                Commit();

                return insertUserPlan;
                }
            }

        public async Task<FoveaReceipt> GetFoveaReceipts(IuguInput input, User user)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var tokenBase64 = System.Text.Encoding.UTF8.GetBytes(String.Concat("br.com.app.construa", ":", input.Token));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {System.Convert.ToBase64String(tokenBase64)}");

            string urlGetReceiptFovea = String.Concat(input.UrlBase, "/customers/", user.Email, "/purchases");
            var uriGetReceiptFovea = new UriBuilder(urlGetReceiptFovea);
            var getResult = await _httpClient.GetAsync(uriGetReceiptFovea.ToString());
            string responseBody = await getResult.Content.ReadAsStringAsync();

            _logger.LogInformation("GetFoveaReceipts executed at {date} with result {@param}", DateTime.UtcNow, getResult);
            return JsonConvert.DeserializeObject<FoveaReceipt>(responseBody);
        }

        public async Task<bool> PostFoveaSandobxWebhookToDevelopment(string jsonWebhook)
        {
            HttpResponseMessage postResult = new HttpResponseMessage();

            try
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                string urlPost = "https://api-dsv.serverconstruaapp.com/api/v1/signature/fovea/webhook";
                var uriPost = new UriBuilder(urlPost);
                StringContent contentRequest = null;

                contentRequest = new StringContent(jsonWebhook, Encoding.UTF8, "application/json");
                postResult = await _httpClient.PostAsync(uriPost.ToString(), contentRequest);
                string responseBody = await postResult.Content.ReadAsStringAsync();

                _logger.LogInformation("PostFoveaSandobxWebhookToDevelopment executed at {date} with result {@param}", DateTime.UtcNow, postResult);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("PostFoveaSandobxWebhookToDevelopment failed at {date} with error message {error}", DateTime.UtcNow, ex.Message);
            }
            return false;
        }
    }
}
