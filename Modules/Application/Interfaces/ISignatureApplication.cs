using Application.AppServices.SignatureApplication.Input;
using Application.AppServices.SignatureApplication.ViewModels;
using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces
    {
    public interface ISignatureApplication
        {
        Task<SignatureViewModel> PostSignatureAsync(SignatureInput input);
        Task<bool> ChangePaymentMethodAsync(SignatureInput input);

        Task<UserPlans> ProcessIuguWebhookAsync(IuguWebhookInput input);

        Task<SignatureViewModel> PostChangeStatusSignatureAsync(SignatureInput signatureInput, string actionStatus);

        Task<UserPlans> ProcessFoveaWebhookAsync(FoveaWebhookInput input, string environmentName); 

        Task<UserPlans> CreateTempSignatureAsync(SignatureInput signatureInput, string platform);
        }
    }
