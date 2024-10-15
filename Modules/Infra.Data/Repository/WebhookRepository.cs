using Domain.Entities;
using Domain.Interfaces.Repositories;
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
    public class WebhookRepository : GenericRepository<Webhook, int, MySQLCoreContext>, IWebhookRepository
    {
        public WebhookRepository(MySQLCoreContext context, ILogger<GenericRepository<Webhook, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}
