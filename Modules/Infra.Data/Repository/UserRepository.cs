using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class UserRepository: GenericRepository<User, int, MySQLCoreContext>, IUserRepository
    {
        public UserRepository(MySQLCoreContext context, ILogger<GenericRepository<User, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}
