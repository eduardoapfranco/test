using Domain.Entities;
using Infra.CrossCutting.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IProfileFunctionalityRepository : IRepository<ProfileFunctionality, int>
    {
        Task<IEnumerable<Functionality>> GetFunctionalitiesProfile(int idProfile);
    }
}
