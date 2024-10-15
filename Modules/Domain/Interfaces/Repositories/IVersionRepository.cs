using Domain.Entities;
using Infra.CrossCutting.Repository.Interfaces;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IVersionRepository : IRepository<Version, int>
    {
        Task<Version> GetLast(string platform);
    }
}
