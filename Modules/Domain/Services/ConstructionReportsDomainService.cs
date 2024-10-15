using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;
using Infra.CrossCutting.Domain.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Domain.Services
    {
    public class ConstructionReportsDomainService : DomainService<ConstructionReports, int, IUnitOfWork>, IConstructionReportsDomainService
        {
        private readonly IConstructionReportsRepository _constructionReportsRepository;
        private readonly IConstructionReportsTypesRepository _constructionReportsTypesRepository;
        private ISmartNotification _notification;
        private ILogger<ConstructionReportsDomainService> _logger;

        public ConstructionReportsDomainService(
           IConstructionReportsRepository constructionReportsRepository,
           IConstructionReportsTypesRepository constructionReportsTypesRepository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<ConstructionReportsDomainService> logger
       ) : base(constructionReportsRepository, unitOfWork, messageHandler)
            {
            _constructionReportsRepository = constructionReportsRepository;
            _constructionReportsTypesRepository = constructionReportsTypesRepository;
            _notification = notification;
            _logger = logger;
            }

        public async Task<ConstructionReportsTypes> getReportType(int typeId)
            {
            var reportType = await _constructionReportsTypesRepository.SelectByIdAsync(typeId);
            if(null != reportType)
                {
                return reportType;
                }

            return null;
            }

        }
    }
