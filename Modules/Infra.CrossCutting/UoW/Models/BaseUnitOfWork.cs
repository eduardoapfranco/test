using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Model;
using Infra.CrossCutting.UoW.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Infra.CrossCutting.UoW.Models
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseUnitOfWork<TIUnitOfWork> : BaseUnitOfWork
     where TIUnitOfWork : IUnitOfWorkBase
    {

        public BaseUnitOfWork(TIUnitOfWork unitOfWork,
            INotificationHandler<DomainNotification> notification) : base(unitOfWork, notification)
        {
        }
    }

    public abstract class BaseUnitOfWork
    {
        private readonly IUnitOfWorkBase _unitOfWork;
        private readonly DomainNotificationHandler _messageHandler;

        public BaseUnitOfWork(IUnitOfWorkBase unitOfWork,
            INotificationHandler<DomainNotification> notification)
        {
            _unitOfWork = unitOfWork;
            _messageHandler = (DomainNotificationHandler)notification;
        }

        protected bool Commit()
        {
            if (_messageHandler == null)
                return false;

            if (_messageHandler.HasNotifications())
                return false;

            var commandReponse = _unitOfWork.Commit();
            if (commandReponse.Success)
                return true;

            return false;
        }
    }
}
