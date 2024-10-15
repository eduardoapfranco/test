using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infra.CrossCutting.Repository.Interfaces
{
    public interface IRepository<TEntity, TKey>
    {
        Task<TEntity> InsertAsync(TEntity item);
        Task<bool> InsertAllAsync(IEnumerable<TEntity> item);
        Task<TEntity> SelectByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> SelectAllAsync();
        Task<IEnumerable<TEntity>> SelectFilterAsync(Expression<Func<TEntity, bool>> where);
        Task<TEntity> UpdateAsync(TEntity item);
        Task<bool> DeleteAsync(TKey id);
        Task<bool> DeleteAllAsync();
        Task<bool> ExistsAsync(TKey id);
        Task ExecuteVACUUMForSqlite();
    }
}
