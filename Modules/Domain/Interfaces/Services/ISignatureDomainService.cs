using Domain.Entities;
using Domain.Input;
using Domain.Input.Fovea;
using Domain.Input.Iugu;
using Infra.CrossCutting.Domain.Interfaces;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
    {
    public interface ISignatureDomainService : IDomainService<UserPlans, int>
        {
        Task<Customer> PostUserAsync(User user, IuguInput input);
        Task<PaymentMethod> PostUserPaymentMethodAsync(string iuguCustomerId, string PaymentMethodToken, IuguInput input);
        Task<Signature> PostSignatureMethodAsync(Plan plan, string iuguPaymentMethodId, IuguInput input);
        Task<UserPlans> GetUserPlanByPartnerSignatureId(string partnerSignatureId);
        Task<UserPlans> GetUserPlanWithIuguIdByUserId(int userId);
        Task<Signature> PostChangeStatusSignatureAsync(IuguInput input, string iuguSubscriptionId, string actionStatus);
        Task<User> UpdateUserToPremiumPlanAsync(User user, Plan plan, string partnerSignatureId = null);
        Task<UserPlans> GetUserPlan(User user, Plan plan);
        Task<UserPlans> UpdateUserToTempPremiumPlanAsync(User user, Plan plan, string partnerSignatureId = null);
        Task<FoveaReceipt> GetFoveaReceipts(IuguInput input, User user);
        Task<bool> PostFoveaSandobxWebhookToDevelopment(string jsonWebhook);
        }
    }
