using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Utils;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class ProfileCategoryRepository : GenericRepository<ProfileCategory, int, MySQLCoreContext>, IProfileCategoryRepository
    {
        public ProfileCategoryRepository(MySQLCoreContext context, ILogger<GenericRepository<ProfileCategory, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategoriesProfile(int idProfile)
        {
            var categoriesQuery = _context.ProfilesCategories.AsQueryable();

            var query = from catProfile in categoriesQuery
                        join cat in _context.Categories on catProfile.CategoryId equals cat.Id
                        where catProfile.ProfileId == idProfile
                        select cat;

            var result = await query.ToListAsync();

            if(result.Any())
            {
                return result;
            }

            return CreateDefaultFreemium.CreateProfileCategories();;
        }
    }
}
