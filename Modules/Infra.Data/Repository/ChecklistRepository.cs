using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Data.Repository
    {
    [ExcludeFromCodeCoverage]
    public class ChecklistRepository : GenericRepository<Checklist, int, MySQLCoreContext>, IChecklistRepository
    {
        public ChecklistRepository(MySQLCoreContext context, ILogger<GenericRepository<Checklist, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }

        public async Task<Checklist> GetLastUpdated()
            {
            var lastUpdatedChecklist = await _context.Checklists
                       .OrderByDescending(x => x.UpdatedAt)
                       .FirstOrDefaultAsync();
            return lastUpdatedChecklist;
            }
        }
}
