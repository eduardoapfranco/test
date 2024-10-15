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
    public class UserAreaRepository: GenericRepository<UserAreas, int, MySQLCoreContext>, IUserAreaRepository
        {
        public UserAreaRepository(MySQLCoreContext context, ILogger<GenericRepository<UserAreas, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }

        public async Task<IEnumerable<Area>> GetUserAreas(int userId)
            {
            var areasQuery = _context.UserAreas.AsQueryable();

            var query = from uAreas in areasQuery
                        join ar in _context.Areas on uAreas.AreaId equals ar.Id
                        where uAreas.UserId == userId
                        select ar;

            return await query.ToListAsync();

            }

        }
}
