using Domain.Entities;
using Infra.CrossCutting.Domain.Interfaces;
using Org.BouncyCastle.Crypto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface IPlanDomainService : IDomainService<Plan, int>
    {
        Task<Plan> GetPremiumPlanAsync(int planId);
        Task<Plan> GetFreemiumPlanAsync();
        Task<IEnumerable<Plan>> GetPlansPremiumWithType();
        Task<Plan> GetPlanByPartnerId(string partnerId);
    }
}
