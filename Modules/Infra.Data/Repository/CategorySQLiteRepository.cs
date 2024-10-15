using Domain.Entities.Sqlite;
using Domain.Interfaces.Repositories;
using Infra.CrossCutting.Repository;
using Infra.Data.Context;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class CategorySQLiteRepository : GenericRepository<CategorySqlite, int, SQLiteCoreContext>, ICategorySQLiteRepository
    {
        public CategorySQLiteRepository(SQLiteCoreContext context, ILogger<GenericRepository<CategorySqlite, int, SQLiteCoreContext>> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}
