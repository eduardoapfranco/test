using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class FunctionalityRepository : GenericRepository<Functionality, int, MySQLCoreContext>, IFunctionalityRepository
    {
        public FunctionalityRepository(MySQLCoreContext context, ILogger<GenericRepository<Functionality, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}
