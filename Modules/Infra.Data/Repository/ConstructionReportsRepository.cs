using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class ConstructionReportsRepository : GenericRepository<ConstructionReports, int, MySQLCoreContext>, IConstructionReportsRepository
    {
        public ConstructionReportsRepository(MySQLCoreContext context, ILogger<GenericRepository<ConstructionReports, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }
        }
}
