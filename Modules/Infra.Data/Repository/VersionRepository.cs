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
    public class VersionRepository: GenericRepository<Version, int, MySQLCoreContext>, IVersionRepository
    {
        public VersionRepository(MySQLCoreContext context, ILogger<GenericRepository<Version, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }

        public async Task<Version> GetLast(string platform)
            {
            var lastVersion = await _context.Versions
                        .Where(x => x.Platform.Equals(platform))
                       .OrderByDescending(x => x.Id)
                       .FirstOrDefaultAsync();
            return lastVersion;
            }
        }
}
