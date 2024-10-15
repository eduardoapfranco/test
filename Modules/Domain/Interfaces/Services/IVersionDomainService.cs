using Domain.Entities;
using Infra.CrossCutting.Domain.Interfaces;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
    {
    public interface IVersionDomainService : IDomainService<Version, int>
    {
        Task<Domain.Entities.Version> GetVersionAsync(string platform, string version);
        }
}
