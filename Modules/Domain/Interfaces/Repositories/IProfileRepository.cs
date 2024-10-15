using Domain.Entities;
using Infra.CrossCutting.Repository.Interfaces;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IProfileRepository : IRepository<Profile, int>
    {
        Task<Profile> GetProfileAsync(int planId);
    }
}
