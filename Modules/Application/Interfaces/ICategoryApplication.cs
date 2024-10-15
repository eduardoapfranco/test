using Application.AppServices.CategoryApplication.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICategoryApplication
    {
        Task<IEnumerable<CategoryViewModel>> GetCategoriesLastDateUpdatedAsync(DateTime? lastDateSync);
        Task<IEnumerable<CategoryViewModel>> GetRootCategoriesBasedOnProfileAsync(int userId);
        Task<IEnumerable<CategoryViewModel>> GetCategoriesByParentBasedOnProfileAsync(int categoryId);
        Task<IEnumerable<CategoryViewModel>> GetAllAsync();
        Task<CategoryViewModel> SelectByIdAsync(int id);
    }
}