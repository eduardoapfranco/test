using Domain.Entities;
using Infra.CrossCutting.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
    {
    public interface IAreaDomainService : IDomainService<Area, int>
    {
        Task<IEnumerable<Domain.Entities.Area>> GetAreasAsync();
       }
}
