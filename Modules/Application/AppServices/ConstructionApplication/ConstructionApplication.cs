using Application.AppServices.ConstructionApplication.Input;
using Application.AppServices.ConstructionApplication.ViewModel;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Application.AppServices.ConstructionReportApplication.ViewModel;

namespace Application.AppServices.ConstructionApplication
    {
    public class ConstructionApplication : BaseValidationService, IConstructionApplication
    {
        private readonly ISmartNotification _notification;
        private readonly IConstructionDomainService _constructionDomainService;
        private readonly IMapper _mapper;
        private readonly ILogger<ConstructionApplication> _logger;


        public ConstructionApplication(IConstructionDomainService constructionDomainService, ISmartNotification notification, 
            IMapper mapper, ILogger<ConstructionApplication> logger
            ) : base(notification)
        {
            _constructionDomainService = constructionDomainService;
            _notification = notification;
            _mapper = mapper;
            _logger = logger;
            }

        public Construction Map(int userId, ConstructionViewModel construction)
            {
            var entity = _mapper.Map<Construction>(construction);
            entity.UserId = userId;
            return entity;
            }

        public async Task<ConstructionViewModel> InsertAsync(ConstructionInput input)
        {
            _logger.LogInformation($"Init insert construction {nameof(InsertAsync)}");

            if (!input.IsValid())
            {
                var constructionViewModel = _mapper.Map<ConstructionViewModel>(input);
                NotifyErrorsAndValidation(_notification.EmptyPositions(), constructionViewModel);
                _logger.LogWarning($"Init insert construction with param invalid {nameof(InsertAsync)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
            }

            if (await _constructionDomainService.ExistsAsync(input.Id))
                {
                _notification.NewNotificationBadRequest(new string[] { }, "Falha ao criar a construção pois o id informado já consta na base");
                _logger.LogWarning($"Insert construction with invalid id {nameof(InsertAsync)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
                }

            var entity = _mapper.Map<Construction>(input);
            var result = await _constructionDomainService.InsertAsync(entity);
            var mappedConstruction = _mapper.Map<ConstructionViewModel>(result);
            _logger.LogInformation($"Construction inserted {nameof(InsertAsync)}");

            return mappedConstruction;
        }

        public async Task<ConstructionViewModel> UpdateAsync(ConstructionInput input)
            {
            _logger.LogInformation($"Init update construction {nameof(UpdateAsync)}");

            if (!input.IsValid())
                {
                var constructionViewModel = _mapper.Map<ConstructionViewModel>(input);
                NotifyErrorsAndValidation(_notification.EmptyPositions(), constructionViewModel);
                _logger.LogWarning($"Init update construction with param invalid {nameof(UpdateAsync)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
                }

            var constructionList = await _constructionDomainService.SelectFilterAsync(x => x.UserId == input.UserId && x.AppId == input.AppId);
            if (!constructionList.Any())
                {
                _logger.LogWarning($"Init update construction with invalid constructionId or invalid userId {nameof(UpdateAsync)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
                }

            var entity = _mapper.Map<Construction>(input);
            var result = await _constructionDomainService.UpdateAsync(entity);
            var mappedConstruction = _mapper.Map<ConstructionViewModel>(result);
            _logger.LogInformation($"Construction updated {nameof(UpdateAsync)}");

            return mappedConstruction;
            }

        public async Task<IEnumerable<ConstructionViewModel>> ListAsync(int userId)
            {
            _logger.LogInformation($"Init list constructions {nameof(ListAsync)}");

            if (userId == 0)
                {
                _logger.LogWarning($"Init list constructios with param invalid {nameof(ListAsync)} with userId: {userId}");
                return default;
                }

            var constructionList = await _constructionDomainService.SelectFilterAsync(x => x.UserId == userId && !x.Deleted);
            if (!constructionList.Any())
                {
                _logger.LogWarning($"Init list constructios with param invalid {nameof(ListAsync)} with userId: {userId} doesn't find results");

                //return default;
                }


            var mappedConstruction = _mapper.Map<IEnumerable<ConstructionViewModel>>(constructionList);
            _logger.LogInformation($"Construction list {nameof(ListAsync)}");

            return mappedConstruction;
            }

        public async Task<ConstructionSyncResponse> SyncAsync(int userId, List<ConstructionViewModel> appConstrunctions)
            {
            var constructionList = await ListAsync(userId);
            var dbConstructions = constructionList.ToList();

            // Primeiro processa as deleções em ambas as pontas
            var toDeleteDb = dbConstructions
                .Where(db => db.Deleted || appConstrunctions.Any(app => app.AppId == db.AppId && app.Deleted))   // Existem no DB mas não existem (ou estão deletados) no App
                .ToList();

            var toDeleteApp = appConstrunctions
                .Where(app => app.Deleted || dbConstructions.Any(db => db.AppId == app.AppId && db.Deleted))          // Existem no App mas não existem (ou estão deletados) no DB
                .ToList();

            // Agora filtra as deleções em ambas as listas
            dbConstructions = dbConstructions.Where(db => !toDeleteDb.Any(p => p.AppId == db.AppId) && !toDeleteApp.Any(p => p.AppId == db.AppId)).ToList();
            appConstrunctions = appConstrunctions.Where(app => !toDeleteApp.Any(p => p.AppId == app.AppId) && !toDeleteDb.Any(p => p.AppId == app.AppId)).ToList();

            // Restam apenas as contruções realmente a fazer merge
            var toInsertDb = appConstrunctions
                .Where(app => !dbConstructions.Any(db => db.AppId == app.AppId))                    // Existem no App mas nao existem no DB
                .ToList();

            var toUpdateDb = appConstrunctions
                .Where(app => dbConstructions.Any(db => db.AppId == app.AppId && app.IsNewer(db)))  // Existem em ambos mas o App é mais novo
                .ToList();

            if (toUpdateDb.Count > 0)
                {
                // Para os que tem de atualizar no banco, seta o Id, que não veio do app
                foreach (var construction in toUpdateDb)
                    construction.Id = dbConstructions.Where(p => p.AppId == construction.AppId).Select(p => p.Id).FirstOrDefault();
                }

            var toInsertApp = dbConstructions
                .Where(db => !appConstrunctions.Any(app => app.AppId == db.AppId))           // Existem no DB mas não existem no App
                .ToList();

            var toUpdateApp = dbConstructions
                .Where(db => appConstrunctions.Any(app => app.AppId == db.AppId && db.IsNewer(app))) // Existem em ambos mas o DB é mais novo
                .ToList();


            IEnumerable<Construction> mappedInserts = toInsertDb.Select(p => Map(userId, p));
            IEnumerable<Construction> mappedUpdates = toUpdateDb.Select(p => Map(userId, p));
            IEnumerable<Construction> mappedDeletes = toDeleteDb.Select(p => Map(userId, p));
            //IEnumerable<int> mappedDeletes = toDeleteDb.Select(p => p.Id);

            if (await _constructionDomainService.Sync(mappedInserts, mappedDeletes, mappedUpdates))
                {
                return new ConstructionSyncResponse()
                    {
                    toDelete = toDeleteApp.Select(p => p.AppId),
                    toInsert = toInsertApp,
                    toUpdate = toUpdateApp,
                    };
                }

            return null;
            }
        }
}
