using Application.AppServices.RatingApplication.Input;
using Application.AppServices.RatingApplication.ViewModel;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Application.AppServices.RatingApplication
    {
    public class RatingApplication : BaseValidationService, IRatingApplication
    {
        private readonly ISmartNotification _notification;
        private readonly IRatingDomainService _ratingDomainService;
        private readonly ICategoryDomainService _categoryDomainService;
        private readonly IChecklistDomainService _checklistDomainService;
        private readonly IMapper _mapper;
        private readonly ILogger<RatingApplication> _logger;


        public RatingApplication(IRatingDomainService ratingDomainService, ISmartNotification notification, 
            IMapper mapper, ILogger<RatingApplication> logger,
            ICategoryDomainService categoryDomainService, IChecklistDomainService checklistDomainService
            ) : base(notification)
        {
            _ratingDomainService = ratingDomainService;
            _notification = notification;
            _mapper = mapper;
            _logger = logger;
            _categoryDomainService = categoryDomainService;
            _checklistDomainService = checklistDomainService;
            }

        public async Task<RatingViewModel> InsertAsync(RatingInput input)
        {
            _logger.LogInformation($"Init insert rating {nameof(InsertAsync)}");

            if (!input.IsValid())
            {
                var ratingViewModel = _mapper.Map<RatingViewModel>(input);
                NotifyErrorsAndValidation(_notification.EmptyPositions(), ratingViewModel);
                _logger.LogWarning($"Init insert rating with param invalid {nameof(InsertAsync)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
            }

            if (input.ChecklistId.HasValue)
                {
                var checklist = await _checklistDomainService.SelectByIdAsync(input.ChecklistId.Value);
                if(null == checklist)
                    {
                    _notification.NewNotificationBadRequest(new string[] { input.ChecklistId.Value.ToString() }, 
                        "O checklist com id '{0}' não está cadastrado em nosso sistema.");
                    _logger.LogWarning($"Init insert rating failed because checklist id {0} doesn't exists", input.ChecklistId.Value.ToString());
                    return default;
                    }
                }

            if (input.CategoryId.HasValue)
                {
                var category = await _categoryDomainService.SelectByIdAsync(input.CategoryId.Value);
                if (null == category)
                    {
                    _notification.NewNotificationBadRequest(new string[] { input.CategoryId.Value.ToString() },
                        "A categoria com id '{0}' não está cadastrada em nosso sistema.");
                    _logger.LogWarning($"Init insert rating failed because category id {0} doesn't exists", input.CategoryId.Value.ToString());
                    return default;
                    }
                }

            var entity = _mapper.Map<Rating>(input);
            var result = await _ratingDomainService.InsertAsync(entity);
            var mappedRating = _mapper.Map<RatingViewModel>(result);
            _logger.LogInformation($"Rating inserted {nameof(InsertAsync)}");

            return mappedRating;
        }
    }
}
