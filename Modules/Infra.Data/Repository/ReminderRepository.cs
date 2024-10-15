using Domain.Entities;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Interfaces.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ReminderRepository : GenericRepository<Reminder, long, MySQLCoreContext>, IReminderRepository
        {
        public ReminderRepository(MySQLCoreContext context, ILogger<GenericRepository<Reminder, long, MySQLCoreContext>> logger) : base(context, logger)
            {
            _context = context;
            }
        }
    }
