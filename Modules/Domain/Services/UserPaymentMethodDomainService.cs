using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;
using Infra.CrossCutting.Domain.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Domain.Enum;

namespace Domain.Services
{
    public class UserPaymentMethodDomainService : DomainService<Entities.UserPaymentMethod, long, IUnitOfWork>, IUserPaymentMethodDomainService
    {
        private readonly IUserPaymentMethodRepository _repository;
        private ISmartNotification _notification;
        private ILogger<UserPaymentMethodDomainService> _logger;
        public UserPaymentMethodDomainService(
            IUserPaymentMethodRepository repository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<UserPaymentMethodDomainService> logger
       ) : base(repository, unitOfWork, messageHandler)
        {
            _repository = repository;
            _notification = notification;
            _logger = logger;
        }


        public async Task<UserPaymentMethod> InsertAndInactiveAsync(UserPaymentMethod entity)
        {
            using (_unitOfWork.BeginTransaction())
            {
                var userPaymentsMethods = await _repository.SelectFilterAsync(x => x.UserId.Equals(entity.UserId));

                if (userPaymentsMethods.Any())
                {
                    foreach (var userPaymentMethod in userPaymentsMethods)
                    {
                        userPaymentMethod.Active = (sbyte) BoolEnum.NO;
                        await _unitOfWork.UserPaymentMethod.UpdateAsync(userPaymentMethod);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }

                var result = await _unitOfWork.UserPaymentMethod.InsertAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                Commit();
                return result;
            }
        }
    }
}
