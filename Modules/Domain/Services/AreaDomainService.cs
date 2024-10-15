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
    public class AreaDomainService : DomainService<Entities.Area, int, IUnitOfWork>, IAreaDomainService
    {
        private readonly IAreaRepository _areaRepository;
        private ISmartNotification _notification;
        private ILogger<AreaDomainService> _logger;
        public AreaDomainService(
           IAreaRepository areaRepository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<AreaDomainService> logger
       ) : base(areaRepository, unitOfWork, messageHandler)
        {
            _areaRepository = areaRepository;
            _notification = notification;
            _logger = logger;
        }

        public async Task<IEnumerable<Domain.Entities.Area>> GetAreasAsync()
            {
            _logger.LogInformation("GetAreasAsync initialized at {date}", DateTime.UtcNow);
            return await _areaRepository.SelectAllAsync();
            }
        }
}
