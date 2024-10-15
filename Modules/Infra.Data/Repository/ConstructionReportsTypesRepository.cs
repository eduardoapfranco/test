using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class ConstructionReportsTypesRepository : GenericRepository<ConstructionReportsTypes, int, MySQLCoreContext>, IConstructionReportsTypesRepository
    {
        public ConstructionReportsTypesRepository(MySQLCoreContext context, ILogger<GenericRepository<ConstructionReportsTypes, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}
