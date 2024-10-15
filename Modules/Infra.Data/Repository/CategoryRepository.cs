using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class CategoryRepository : GenericRepository<Category, int, MySQLCoreContext>, ICategoryRepository
    {
        public CategoryRepository(MySQLCoreContext context, ILogger<GenericRepository<Category, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }

        public async Task<Category> GetLastUpdated()
            {
            var lastUpdatedCategory = await _context.Categories
                       .OrderByDescending(x => x.UpdatedAt)
                       .FirstOrDefaultAsync();
            return lastUpdatedCategory;
            }
        }
}
