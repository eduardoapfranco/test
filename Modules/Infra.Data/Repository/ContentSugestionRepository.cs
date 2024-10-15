using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Repository
    {
    [ExcludeFromCodeCoverage]
    public class ContentSugestionRepository: GenericRepository<ContentSugestion, int, MySQLCoreContext>, IContentSugestionRepository
    {
        public ContentSugestionRepository(MySQLCoreContext context, ILogger<GenericRepository<ContentSugestion, int, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}
