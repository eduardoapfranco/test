using Domain.Entities;
using Infra.CrossCutting.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface IConstructionDomainService : IDomainService<Construction, int>
    {
        Task<bool> Sync(IEnumerable<Construction> toInsert, IEnumerable<Construction> toDelete, IEnumerable<Construction> toUpdate);
    }
}
