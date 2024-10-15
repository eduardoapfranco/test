using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;
using Infra.CrossCutting.Domain.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class ConstructionDomainService : DomainService<Construction, int, IUnitOfWork>, IConstructionDomainService
    {
        private readonly IConstructionRepository _constructionRepository;
        private ISmartNotification _notification;
        private ILogger<ConstructionDomainService> _logger;

        public ConstructionDomainService(
           IConstructionRepository constructionRepository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<ConstructionDomainService> logger
       ) : base(constructionRepository, unitOfWork, messageHandler)
        {
            _constructionRepository = constructionRepository;
            _notification = notification;
            _logger = logger;
        }

        public async Task<bool> Sync(IEnumerable<Construction> toInsert, IEnumerable<Construction> toDelete, IEnumerable<Construction> toUpdate)
            {
            await using (_unitOfWork.BeginTransaction())
                {
                bool result = await _unitOfWork.Construction.InsertAllAsync(toInsert);
                if (!result)
                    {
                    _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao inserir obras");
                    return false;
                    }

                foreach (var construction in toUpdate)
                    {
                    try
                        {
                        await _unitOfWork.Construction.UpdateAsync(construction);
                        }
                    catch (Exception e)
                        {
                        _logger.LogError("Ao atualizar obra {0}: {1}", JsonConvert.SerializeObject(construction), e.Message);
                        _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao atualizar obras");
                        return false;
                        }
                    }
                foreach (var construction in toDelete)
                    {
                    try
                        {
                        construction.Deleted = true;
                        await _unitOfWork.Construction.UpdateAsync(construction);
                        }
                    catch (Exception e)
                        {
                        _logger.LogError("Ao atualizar obra {0}: {1}", JsonConvert.SerializeObject(construction), e.Message);
                        _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao apagar obras");
                        return false;
                        }
                    }

                Commit();
                return true;
                }
            }
        }
    }
