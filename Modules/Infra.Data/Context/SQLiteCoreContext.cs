using Domain.Entities;
using Domain.Entities.Sqlite;
using Infra.Data.Config;
using Infra.Data.Config.Mappers.MySQLCore;
using Infra.Data.Config.Mappers.SQLiteCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Data.Context
{
    [ExcludeFromCodeCoverage]
    public class SQLiteCoreContext : DbContext
    {
        public DbSet<CategorySqlite> CategoriesSqlite { get; set; }
        public DbSet<ChecklistSqlite> ChecklistsSqlite { get; set; }

        public SQLiteCoreContext(DbContextOptions<SQLiteCoreContext> options) : base(options)
        { }

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfiguration(new CategoryMapSQLiteCoreConfig())
                .ApplyConfiguration(new ChecklistMapSQLiteCoreConfig())
                ;
        }

        public override int SaveChanges()
        {
            ConfigPropertieDefault.SaveDefaultPropertiesChanges(ChangeTracker);
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            ConfigPropertieDefault.SaveDefaultPropertiesChanges(ChangeTracker);
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
