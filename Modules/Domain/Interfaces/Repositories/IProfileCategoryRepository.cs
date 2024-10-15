using Domain.Entities;
using Infra.CrossCutting.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IProfileCategoryRepository : IRepository<ProfileCategory, int>
    {
        Task<IEnumerable<Category>> GetCategoriesProfile(int idProfile);
    }
}
