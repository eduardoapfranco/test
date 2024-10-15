using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Sqlite;

namespace Domain.Interfaces.Services
{
    public interface IDbMobileDomainService
    {
        Task<bool> CreateDBMobileAsync(IEnumerable<CategorySqlite> categories, IEnumerable<ChecklistSqlite> checklists, bool imageCategoryIsOnline = false);
    }
}
