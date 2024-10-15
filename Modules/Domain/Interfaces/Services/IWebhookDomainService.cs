using Domain.Entities;
using Domain.Input;
using Infra.CrossCutting.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
    {
    public interface IWebhookDomainService : IDomainService<Webhook, int>
        {
        Task<string> GetFullPagseguroNotificationAsync(PagSeguroNotificationInput input);
        Task<string> GetPagseguroCheckoutCodeAsync(Dictionary<string, string> checkoutParams, PagSeguroNotificationInput input);
        }
    }
