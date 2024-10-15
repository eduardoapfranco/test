using Domain.Entities;
using Infra.CrossCutting.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
    {
    public interface IUserAreaDomainService : IDomainService<UserAreas, int>
    {
       }
}
