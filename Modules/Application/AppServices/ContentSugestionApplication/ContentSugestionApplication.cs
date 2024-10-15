using Application.AppServices.ContentSugestionApplication.Input;
using Application.AppServices.ContentSugestionApplication.ViewModel;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Application.AppServices.ContentSugestionApplication
    {
    public class ContentSugestionApplication : BaseValidationService, IContentSugestionApplication
    {
        private readonly ISmartNotification _notification;
        private readonly IContentSugestionDomainService _contentSugestionDomainService;
        private readonly ICategoryDomainService _categoryDomainService;
        private readonly IChecklistDomainService _checklistDomainService;
        private readonly IMapper _mapper;
        private readonly ILogger<ContentSugestionApplication> _logger;


        public ContentSugestionApplication(IContentSugestionDomainService contentSugestionDomainService, ISmartNotification notification, 
            IMapper mapper, ILogger<ContentSugestionApplication> logger,
            ICategoryDomainService categoryDomainService, IChecklistDomainService checklistDomainService
            ) : base(notification)
        {
            _contentSugestionDomainService = contentSugestionDomainService;
            _notification = notification;
            _mapper = mapper;
            _logger = logger;
            _categoryDomainService = categoryDomainService;
            _checklistDomainService = checklistDomainService;
            }

        public async Task<ContentSugestionViewModel> InsertAsync(ContentSugestionInput input)
        {
            _logger.LogInformation($"Init insert contentSugestion {nameof(InsertAsync)}");

            if (!input.IsValid())
            {
                var contentSugestionViewModel = _mapper.Map<ContentSugestionViewModel>(input);
                NotifyErrorsAndValidation(_notification.EmptyPositions(), contentSugestionViewModel);
                _logger.LogWarning($"Init insert contentSugestion with param invalid {nameof(InsertAsync)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
            }

            if (input.ChecklistId.HasValue)
                {
                var checklist = await _checklistDomainService.SelectByIdAsync(input.ChecklistId.Value);
                if(null == checklist)
                    {
                    _notification.NewNotificationBadRequest(new string[] { input.ChecklistId.Value.ToString() }, 
                        "O checklist com id '{0}' não está cadastrado em nosso sistema.");
                    _logger.LogWarning($"Init insert contentSugestion failed because checklistId {0} doesn't exists", input.ChecklistId.Value.ToString());
                    return default;
                    }
                }

            if (input.CategoryId.HasValue)
                {
                var category = await _categoryDomainService.SelectByIdAsync(input.CategoryId.Value);
                if (null == category)
                    {
                    _notification.NewNotificationBadRequest(new string[] { input.CategoryId.Value.ToString() },
                        "O checklist com id '{0}' não está cadastrado em nosso sistema.");
                    _logger.LogWarning($"Init insert contentSugestion failed because checklistId {0} doesn't exists", input.CategoryId.Value.ToString());
                    return default;
                    }
                }

            var entity = _mapper.Map<ContentSugestion>(input);
            var result = await _contentSugestionDomainService.InsertAsync(entity);
            var mappedContentSugestion = _mapper.Map<ContentSugestionViewModel>(result);
            _logger.LogInformation($"ContentSugestion inserted {nameof(InsertAsync)}");

            return mappedContentSugestion;
        }
    }
}
