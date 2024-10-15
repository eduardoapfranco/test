using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class UserPaymentMethodRepository : GenericRepository<UserPaymentMethod, long, MySQLCoreContext>, IUserPaymentMethodRepository
    {
        public UserPaymentMethodRepository(MySQLCoreContext context, ILogger<GenericRepository<UserPaymentMethod, long, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}
