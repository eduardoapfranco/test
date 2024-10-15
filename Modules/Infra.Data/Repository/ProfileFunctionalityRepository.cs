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
    public class ProfileFunctionalityRepository : GenericRepository<ProfileFunctionality, int, MySQLCoreContext>, IProfileFunctionalityRepository
    {
        public ProfileFunctionalityRepository(MySQLCoreContext context, ILogger<GenericRepository<ProfileFunctionality, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }

        public async Task<IEnumerable<Functionality>> GetFunctionalitiesProfile(int idProfile)
        {
            var functionalitiesQuery = _context.ProfilesFunctionalities.AsQueryable();

            var query = from funProfile in functionalitiesQuery
                        join fun in _context.Functionalities on funProfile.FunctionalityId equals fun.Id
                        where funProfile.ProfileId == idProfile
                        select fun;

            var result = await query.ToListAsync();

            if (result.Any())
            {
                return result;
            }

            return CreateDefaultFreemium.CreateProfileFunctionalities();
        }
    }
}
