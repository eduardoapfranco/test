using Infra.CrossCutting.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Newtonsoft.Json;

namespace Infra.CrossCutting.Repository
{
    [ExcludeFromCodeCoverage]
    public abstract class GenericRepository<TEntity, TKey, TContext> : IRepository<TEntity, TKey>, IDisposable
        where TContext : DbContext
        where TEntity : BaseEntity<TKey>
    {
        protected TContext _context;
        protected DbSet<TEntity> _db;
        ILogger<GenericRepository<TEntity, TKey, TContext>> _logger;
        protected string _tableName = "";
        protected string _contextName = "";
        protected string _entityName = "";

        public GenericRepository(TContext context, ILogger<GenericRepository<TEntity, TKey, TContext>> logger)
        {
            _context = context;
            _logger = logger;
            _db = _context.Set<TEntity>();
            var entityType = _context.Model.FindEntityType(typeof(TEntity));
            _tableName = entityType.GetTableName();
            _entityName = typeof(TEntity).Name;
            _contextName = typeof(TContext).Name;
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<TEntity> InsertAsync(TEntity item)
        {
            try
            {
                ILogEventEnricher[] enrichers =
                {
                    new PropertyEnricher("GenericRepositoryTable", _tableName),
                    new PropertyEnricher("GenericRepositoryEntity", _entityName),
                    new PropertyEnricher("GenericRepositoryContext", _contextName),
                    new PropertyEnricher("GenericRepositoryEntityAction", _entityName+"-"+nameof(InsertAsync))
                };

                using (LogContext.Push(enrichers))
                {
                    _db.Add(item);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"TableName: {_tableName} - {nameof(InsertAsync)} in GenericRepository for entity {_entityName} with item: {JsonConvert.SerializeObject(item)} in {DateTime.UtcNow}");
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TableName: {_tableName} - {JsonConvert.SerializeObject(ex)} - Exception for: {nameof(InsertAsync)} in GenericRepository for entity {_entityName} with param: {JsonConvert.SerializeObject(item)} in {DateTime.UtcNow}");
                throw ex;
            }
        }

        public async Task<bool> InsertAllAsync(IEnumerable<TEntity> item)
        {
            try
            {
                ILogEventEnricher[] enrichers =
                {
                    new PropertyEnricher("GenericRepositoryTable", _tableName),
                    new PropertyEnricher("GenericRepositoryEntity", _entityName),
                    new PropertyEnricher("GenericRepositoryContext", _contextName),
                    new PropertyEnricher("GenericRepositoryEntityAction", _entityName+"-"+nameof(InsertAllAsync))
                };

                using (LogContext.Push(enrichers))
                {
                    _db.AddRange(item);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"TableName: {_tableName} - {nameof(InsertAllAsync)} in GenericRepository for entity IEnumerable {_entityName} in {DateTime.UtcNow}");
                    return true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"TableName: {_tableName} - {JsonConvert.SerializeObject(ex)} - Exception for: {nameof(InsertAllAsync)} in GenericRepository for entity IEnumerable {_entityName} in {DateTime.UtcNow}");
                throw ex;
            }
        }


        public async Task<TEntity> SelectByIdAsync(TKey id)
        {
            try
            {
                ILogEventEnricher[] enrichers =
                {
                    new PropertyEnricher("GenericRepositoryTable", _tableName),
                    new PropertyEnricher("GenericRepositoryEntity", _entityName),
                    new PropertyEnricher("GenericRepositoryContext", _contextName),
                    new PropertyEnricher("GenericRepositoryEntityAction", _entityName+"-"+nameof(SelectByIdAsync))
                };

                using (LogContext.Push(enrichers))
                {
                    Expression<Func<TEntity, bool>> predicate = t => t.Id.Equals(id);
                    var result = await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
                    _logger.LogInformation($"TableName: {_tableName} - {nameof(SelectByIdAsync)} in GenericRepository for entity {_entityName} with param: ID - {id} -- with result: {JsonConvert.SerializeObject(result)} in {DateTime.UtcNow}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TableName: {_tableName} - {JsonConvert.SerializeObject(ex)} - Exception for: {nameof(SelectByIdAsync)} in GenericRepository for entity {_entityName} with param: ID - {id}  in {DateTime.UtcNow}");
                throw ex;
            }

        }

        public async Task<IEnumerable<TEntity>> SelectAllAsync()
        {
            try
            {
                ILogEventEnricher[] enrichers =
                {
                    new PropertyEnricher("GenericRepositoryTable", _tableName),
                    new PropertyEnricher("GenericRepositoryEntity", _entityName),
                    new PropertyEnricher("GenericRepositoryContext", _contextName),
                    new PropertyEnricher("GenericRepositoryEntityAction", _entityName+"-"+nameof(SelectAllAsync))
                };

                using (LogContext.Push(enrichers))
                {
                    var result = await _db.ToListAsync().ConfigureAwait(false);
                    _logger.LogInformation($"TableName: {_tableName} - {nameof(SelectAllAsync)} in GenericRepository for entity {_entityName}  in {DateTime.UtcNow}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{JsonConvert.SerializeObject(ex)} - Exception for: {nameof(SelectAllAsync)} in GenericRepository for entity {_entityName} in {DateTime.UtcNow}");
                throw ex;
            }
        }

        public async Task<IEnumerable<TEntity>> SelectFilterAsync(Expression<Func<TEntity, bool>> where)
        {
            try
            {
                ILogEventEnricher[] enrichers =
                  {
                    new PropertyEnricher("GenericRepositoryTable", _tableName),
                    new PropertyEnricher("GenericRepositoryEntity", _entityName),
                    new PropertyEnricher("GenericRepositoryContext", _contextName),
                    new PropertyEnricher("GenericRepositoryEntityAction", _entityName+"-"+nameof(SelectFilterAsync))
                };

                using (LogContext.Push(enrichers))
                {
                    var query =
                    await _db
                   .Where(where)
                   .OrderByDescending(ent => ent.Id)
                   .AsNoTracking()
                   .ToListAsync()
                   .ConfigureAwait(false);
                    _logger.LogInformation($"TableName: {_tableName} - {nameof(SelectFilterAsync)} in GenericRepository for entity {_entityName}in {DateTime.UtcNow}");
                    return query;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TableName: {_tableName} - {JsonConvert.SerializeObject(ex)} - Exception for: {nameof(SelectFilterAsync)} in GenericRepository for entity {_entityName} in {DateTime.UtcNow}");
                throw ex;
            }
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            try
            {
                ILogEventEnricher[] enrichers =
                {
                    new PropertyEnricher("GenericRepositoryTable", _tableName),
                    new PropertyEnricher("GenericRepositoryEntity", _entityName),
                    new PropertyEnricher("GenericRepositoryContext", _contextName),
                    new PropertyEnricher("GenericRepositoryEntityAction", _entityName+"-"+nameof(UpdateAsync))
                };

                using (LogContext.Push(enrichers))
                {
                    var currentEntity = await SelectByIdAsync(entity.Id).ConfigureAwait(false);
                    _logger.LogInformation($"TableName: {_tableName} - {nameof(UpdateAsync)} in GenericRepository for entity {_entityName} with lastEntity: {JsonConvert.SerializeObject(currentEntity)} in {DateTime.UtcNow}");
                    _context.Entry(currentEntity).CurrentValues.SetValues(entity);
                    _logger.LogInformation($"TableName: {_tableName} - {nameof(UpdateAsync)} in GenericRepository for entity {_entityName} with updatedEntity: {JsonConvert.SerializeObject(entity)} in {DateTime.UtcNow}");
                    return entity;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"TableName: {_tableName} - {JsonConvert.SerializeObject(ex)} - Exception for: {nameof(UpdateAsync)} in GenericRepository for entity {_entityName} with param: {JsonConvert.SerializeObject(entity)} in {DateTime.UtcNow}");
                throw ex;
            }
        }

        public async Task<bool> DeleteAsync(TKey id)
        {
            try
            {
                ILogEventEnricher[] enrichers =
                 {
                    new PropertyEnricher("GenericRepositoryTable", _tableName),
                    new PropertyEnricher("GenericRepositoryEntity", _entityName),
                    new PropertyEnricher("GenericRepositoryContext", _contextName),
                    new PropertyEnricher("GenericRepositoryEntityAction", _entityName+"-"+nameof(DeleteAsync))
                };

                using (LogContext.Push(enrichers))
                {
                    var entity = await SelectByIdAsync(id).ConfigureAwait(false);
                    if (entity != null)
                    {
                        await Task.FromResult(_db.Remove(entity));
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"TableName: {_tableName} - {nameof(DeleteAsync)} in GenericRepository for entity {_entityName} deleted entity: {JsonConvert.SerializeObject(entity)} in {DateTime.UtcNow}");
                        return true;
                    }
                    _logger.LogInformation($"TableName: {_tableName} - {nameof(DeleteAsync)} in GenericRepository for entity {_entityName} entity not found for ID - {id} in {DateTime.UtcNow}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TableName: {_tableName} - {JsonConvert.SerializeObject(ex)} - Exception for: {nameof(DeleteAsync)} in GenericRepository for entity {_entityName} with ID - {id} in {DateTime.UtcNow}");
                return false;
            }
        }

        public async Task<bool> DeleteAllAsync()
        {
            try
            {
                ILogEventEnricher[] enrichers =
                {
                    new PropertyEnricher("GenericRepositoryTable", _tableName),
                    new PropertyEnricher("GenericRepositoryEntity", _entityName),
                    new PropertyEnricher("GenericRepositoryContext", _contextName),
                    new PropertyEnricher("GenericRepositoryEntityAction", _entityName+"-"+nameof(DeleteAllAsync))
                };

                using (LogContext.Push(enrichers))
                {
                    await _context.Database.ExecuteSqlCommandAsync($"DELETE FROM {_tableName} where id > @p0", 0);
                    _logger.LogInformation($"TableName: {_tableName} - {nameof(DeleteAllAsync)} in GenericRepository for entity {_entityName} deleted with success in {DateTime.UtcNow}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TableName: {_tableName} - {JsonConvert.SerializeObject(ex)} - Exception for: {nameof(DeleteAllAsync)} in GenericRepository for entity {_entityName} in {DateTime.UtcNow}");
                return false;
            }
        }

        public async Task ExecuteVACUUMForSqlite()
        {
            await _context.Database.ExecuteSqlCommandAsync("VACUUM");
        }

        public async Task<bool> ExistsAsync(TKey id)
        {
            var result = await SelectByIdAsync(id).ConfigureAwait(false);

            if (result != null)
            {
                return true;
            }

            return false;
        }
    }
}
