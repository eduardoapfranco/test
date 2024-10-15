using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class FileBlobStorageRepository : GenericRepository<FileBlobStorage, long, MySQLCoreContext>, IFileBlobStorageRepository
    {
        public FileBlobStorageRepository(MySQLCoreContext context, ILogger<GenericRepository<FileBlobStorage, long, MySQLCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}
