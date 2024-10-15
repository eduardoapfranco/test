using Domain.Entities;
using Infra.CrossCutting.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IPlanRepository : IRepository<Plan, int>
    {
        Task<Plan> GetWithType(int planId);

        Task<Plan> GetWithTypeByTitle(string title);
        
        Task<IEnumerable<Plan>> GetPlansPremiumWithType();
    }
}
