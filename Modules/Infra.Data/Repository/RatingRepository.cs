using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class RatingRepository: GenericRepository<Rating, int, MySQLCoreContext>, IRatingRepository
    {
        public RatingRepository(MySQLCoreContext context, ILogger<GenericRepository<Rating, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}
