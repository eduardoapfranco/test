using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;
using Infra.CrossCutting.Domain.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
    {
    public class VersionDomainService : DomainService<Entities.Version, int, IUnitOfWork>, IVersionDomainService
    {
        private readonly IVersionRepository _versionRepository;
        private ISmartNotification _notification;
        private ILogger<VersionDomainService> _logger;
        public VersionDomainService(
           IVersionRepository versionRepository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<VersionDomainService> logger
       ) : base(versionRepository, unitOfWork, messageHandler)
        {
            _versionRepository = versionRepository;
            _notification = notification;
            _logger = logger;
        }

        public async Task<Domain.Entities.Version> GetVersionAsync(string platform, string version)
            {
            _logger.LogInformation("GetVersionAsync initialized at {date} with platform {@platform} and version {@version}", DateTime.UtcNow, platform, version);
            Domain.Entities.Version versionEntity;

            if (version.Equals("latest"))
                {
                versionEntity = await _versionRepository.GetLast(platform);
                }
            else
                {
                    {
                    var result = await _versionRepository.SelectFilterAsync(x => x._Version.Equals(version) && x.Platform.Equals(platform));
                    if (result.Any())
                        {
                        versionEntity = result.FirstOrDefault();
                        }
                    else
                        {
                        _notification.NewNotificationBadRequest(new string[] { version, platform },
                            "A versão '{0}' da plataforma '`{1}' não está cadastrada em nosso sistema.");
                        _logger.LogWarning($"GetVersionAsync could not found any version with parameters version {@version} and platform {platform}", version, platform);
                        return default;
                        }
                    }
                }

            return versionEntity;
            }
    }
}
