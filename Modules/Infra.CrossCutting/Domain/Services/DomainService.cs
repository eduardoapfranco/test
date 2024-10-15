using Infra.CrossCutting.Domain.Interfaces;
using Infra.CrossCutting.Notification.Model;
using Infra.CrossCutting.Repository;
using Infra.CrossCutting.Repository.Interfaces;
using Infra.CrossCutting.UoW.Interfaces;
using Infra.CrossCutting.UoW.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infra.CrossCutting.Domain.Services
{
    public class DomainService<TEntity, TKey, TIUnitOfWork> : BaseUnitOfWork<TIUnitOfWork>, IDomainService<TEntity, TKey>
         where TEntity : BaseEntity<TKey>
         where TKey : struct
        where TIUnitOfWork : IUnitOfWorkBase
    {
        protected readonly IRepository<TEntity, TKey> _repository;
        protected readonly TIUnitOfWork _unitOfWork;

        public DomainService(
            IRepository<TEntity, TKey> repository,
            TIUnitOfWork unitOfWork,
            INotificationHandler<DomainNotification> messageHandler) : base(unitOfWork, messageHandler)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public virtual async Task<bool> DeleteAsync(TKey id)
        {
            using (_unitOfWork.BeginTransaction())
            {
                var result = await _repository.DeleteAsync(id);
                Commit();
                return result;
            }
        }

        public virtual async Task<bool> ExistsAsync(TKey id)
        {
            return await _repository.ExistsAsync(id);
        }

        public virtual async Task<TEntity> InsertAsync(TEntity item)
        {
            using (_unitOfWork.BeginTransaction())
            {
                var entity = await _repository.InsertAsync(item);
                Commit();
                return entity;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> SelectAllAsync()
        {
            return await _repository.SelectAllAsync();
        }

        public virtual async Task<TEntity> SelectByIdAsync(TKey id)
        {
            return await _repository.SelectByIdAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> SelectFilterAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _repository.SelectFilterAsync(where);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity item)
        {
            using (_unitOfWork.BeginTransaction())
            {
                var entity = await _repository.UpdateAsync(item);
                Commit();
                return entity;
            }
        }
    }
}
