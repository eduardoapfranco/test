using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.Repositories;
using Domain.Utils;
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
    public class ProfileRepository : GenericRepository<Profile, int, MySQLCoreContext>, IProfileRepository
    {
        public ProfileRepository(MySQLCoreContext context, ILogger<GenericRepository<Profile, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }

        public async Task<Profile> GetProfileAsync(int planId)
        {
            var profile = await _context.Profiles.Where(x => x.PlanId == planId && x.Active == (byte)BoolEnum.YES).FirstOrDefaultAsync();

            if (profile == null)
            {
                profile = CreateDefaultFreemium.CreateProfile();
            }

            return profile;
        }
    }
}
