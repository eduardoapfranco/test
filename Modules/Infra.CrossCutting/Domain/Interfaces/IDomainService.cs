using Infra.CrossCutting.Repository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infra.CrossCutting.Domain.Interfaces
{
    public interface IDomainService<TEntity, TKey>
       where TEntity : BaseEntity<TKey>
    {
        Task<TEntity> InsertAsync(TEntity item);
        Task<TEntity> SelectByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> SelectAllAsync();
        Task<IEnumerable<TEntity>> SelectFilterAsync(Expression<Func<TEntity, bool>> where);
        Task<TEntity> UpdateAsync(TEntity item);
        Task<bool> DeleteAsync(TKey id);
        Task<bool> ExistsAsync(TKey id);
    }
}
