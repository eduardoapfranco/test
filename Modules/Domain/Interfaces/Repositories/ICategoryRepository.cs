using Domain.Entities;
using Infra.CrossCutting.Repository.Interfaces;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface ICategoryRepository : IRepository<Category, int>
    {
        Task<Category> GetLastUpdated();
    }
}
