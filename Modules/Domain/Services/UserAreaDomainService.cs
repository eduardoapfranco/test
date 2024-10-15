using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;
using Infra.CrossCutting.Domain.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
    {
    public class UserAreaDomainService : DomainService<Entities.UserAreas, int, IUnitOfWork>, IUserAreaDomainService
    {
        private readonly IUserAreaRepository _userAreaRepository;
        private ISmartNotification _notification;
        private ILogger<UserAreaDomainService> _logger;
        public UserAreaDomainService(
           IUserAreaRepository userAreaRepository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<UserAreaDomainService> logger
       ) : base(userAreaRepository, unitOfWork, messageHandler)
        {
            _userAreaRepository = userAreaRepository;
            _notification = notification;
            _logger = logger;
        }
        }
}
