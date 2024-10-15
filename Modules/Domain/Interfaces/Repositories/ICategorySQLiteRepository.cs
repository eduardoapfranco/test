using Domain.Entities.Sqlite;
using Infra.CrossCutting.Repository.Interfaces;

namespace Domain.Interfaces.Repositories
{
    public interface ICategorySQLiteRepository : IRepository<CategorySqlite, int>
    {
    }
}
