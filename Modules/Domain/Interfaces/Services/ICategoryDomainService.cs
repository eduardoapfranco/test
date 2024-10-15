using System.Collections;
using System.Collections.Generic;
using Domain.Entities;
using Domain.Input;
using Infra.CrossCutting.Domain.Interfaces;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface ICategoryDomainService : IDomainService<Category, int>
    {
        Task<IEnumerable<Category>> GetRootCategoriesBasedOnProfileAsync(int userId);
        Task<IEnumerable<Category>> GetCategoriesByParentBasedOnProfileAsync(int categoryId);
        Task<IEnumerable<Category>> GetAllAsync();
    }
}
