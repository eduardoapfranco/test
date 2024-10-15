using Application.AppServices.CategoryApplication.ViewModel;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Auth.Intefaces;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.AppServices.CategoryApplication
{
    public class CategoryApplication : BaseValidationService, ICategoryApplication
    {
        private readonly ISmartNotification _notification;
        private readonly ICategoryDomainService _categoryDomainService;
        private readonly IAuthService<User> _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryApplication> _logger;


        public CategoryApplication(
            ICategoryDomainService categoryDomainService, ISmartNotification notification, 
            IMapper mapper, IAuthService<User> authService, ILogger<CategoryApplication> logger) : base(notification)
        {
            _categoryDomainService = categoryDomainService;
            _notification = notification;
            _mapper = mapper;
            _authService = authService;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetCategoriesLastDateUpdatedAsync(DateTime? lastDateSync)
        {
            if (!lastDateSync.HasValue)
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "O parâmetro lastDateSync é obrigatório");
                _logger.LogWarning($"Param invalid {nameof(GetCategoriesLastDateUpdatedAsync)}");
                return default;
            }
            var listCategories = await _categoryDomainService.SelectFilterAsync(x => x.UpdatedAt >= lastDateSync);

            if (listCategories.Any())
            {
                var mappedListCategories = _mapper.Map<IEnumerable<CategoryViewModel>>(listCategories);
                return mappedListCategories;
            }
            return new List<CategoryViewModel>();            
        }

        public async Task<IEnumerable<CategoryViewModel>> GetRootCategoriesBasedOnProfileAsync(int userId)
        {
            var categories = await _categoryDomainService.GetRootCategoriesBasedOnProfileAsync(userId);
            return _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
        }

        public async Task<IEnumerable<CategoryViewModel>> GetCategoriesByParentBasedOnProfileAsync(int categoryId)
        {
            var categories = await _categoryDomainService.GetCategoriesByParentBasedOnProfileAsync(categoryId);
            return _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAllAsync()
        {
            var categories = await _categoryDomainService.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
        }

        public async Task<CategoryViewModel> SelectByIdAsync(int id)
        {
            var category = await _categoryDomainService.SelectByIdAsync(id);
            return _mapper.Map<CategoryViewModel>(category);
        }
    }
}