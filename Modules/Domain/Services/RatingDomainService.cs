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
    public class RatingDomainService : DomainService<Rating, int, IUnitOfWork>, IRatingDomainService
    {
        private readonly IRatingRepository _ratingRepository;
        private ISmartNotification _notification;
        private ILogger<RatingDomainService> _logger;

        public RatingDomainService(
           IRatingRepository ratingRepository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<RatingDomainService> logger
       ) : base(ratingRepository, unitOfWork, messageHandler)
        {
            _ratingRepository = ratingRepository;
            _notification = notification;
            _logger = logger;
        }
    }
    }
