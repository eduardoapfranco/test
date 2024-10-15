using Domain.Entities;
using Infra.CrossCutting.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
    {
    public interface IUserAreaRepository : IRepository<UserAreas, int>
    {
        public Task<IEnumerable<Area>> GetUserAreas(int userId);
    }
}
