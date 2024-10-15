using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class ChecklistSQLiteRepository : GenericRepository<ChecklistSqlite, int, SQLiteCoreContext>, IChecklistSQLiteRepository
    {
        public ChecklistSQLiteRepository(SQLiteCoreContext context, ILogger<GenericRepository<ChecklistSqlite, int, SQLiteCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}
