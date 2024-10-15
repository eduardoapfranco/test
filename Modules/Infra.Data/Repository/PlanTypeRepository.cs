using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class PlanTypeRepository : GenericRepository<PlanType, int, MySQLCoreContext>, IPlanTypeRepository
    {
        public PlanTypeRepository(MySQLCoreContext context, ILogger<GenericRepository<PlanType, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}
