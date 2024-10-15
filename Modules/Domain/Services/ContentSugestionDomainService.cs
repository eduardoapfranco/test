using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;
using Infra.CrossCutting.Domain.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Domain.Services
{
    public class ContentSugestionDomainService : DomainService<ContentSugestion, int, IUnitOfWork>, IContentSugestionDomainService
    {
        private readonly IContentSugestionRepository _contentSugestionRepository;
        private ISmartNotification _notification;
        private ILogger<ContentSugestionDomainService> _logger;

        public ContentSugestionDomainService(
           IContentSugestionRepository contentSugestionRepository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<ContentSugestionDomainService> logger
       ) : base(contentSugestionRepository, unitOfWork, messageHandler)
        {
            _contentSugestionRepository = contentSugestionRepository;
            _notification = notification;
            _logger = logger;
        }
    }
    }
