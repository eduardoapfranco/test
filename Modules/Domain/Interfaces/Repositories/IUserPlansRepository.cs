using Domain.Entities;
using Infra.CrossCutting.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IUserPlansRepository : IRepository<UserPlans, int>
    {
        Task<UserPlans> GetPlanUserTerm(int userId);
    }
}
