using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class PasswordResetMobileRepository : GenericRepository<PasswordReset, long, MySQLCoreContext>, IPasswordResetMobileRepository
    {
        public PasswordResetMobileRepository(MySQLCoreContext context, ILogger<GenericRepository<PasswordReset, long, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}
