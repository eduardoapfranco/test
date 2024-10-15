using Domain.Entities;
using Infra.CrossCutting.Repository.Interfaces;

namespace Domain.Interfaces.Repositories
    {
    public interface IAreaRepository : IRepository<Area, int>
    {
    }
}
