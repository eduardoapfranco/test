using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enum;
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
    public class CategoryDomainService : DomainService<Category, int, IUnitOfWork>, ICategoryDomainService
    {
        private readonly ICategoryRepository _categoryRepository;
        private ISmartNotification _notification;
        private ILogger<CategoryDomainService> _logger;
        private readonly IUserDomainService _userDomainService;

        public CategoryDomainService(
           ICategoryRepository categoryRepository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<CategoryDomainService> logger,
           IUserDomainService userDomainService
       ) : base(categoryRepository, unitOfWork, messageHandler)
        {
            _categoryRepository = categoryRepository;
            _notification = notification;
            _logger = logger;
            _userDomainService = userDomainService;
            _userDomainService = userDomainService;
        }

        public async Task<IEnumerable<Category>> GetRootCategoriesBasedOnProfileAsync(int userId)
        {
            var userProfile = await _userDomainService.GetControlAccessAsync(userId);

            if (userProfile?.Categories == null)
            {
                return default;
            }

            var categoriesId = userProfile.Categories.Select(x => x.Id);
            var categories = await _categoryRepository.SelectFilterAsync(x => categoriesId.Contains(x.Id) && x.Active == (byte) BoolEnum.YES && x.VisibleApp == (byte)BoolEnum.YES);
            return categories.OrderBy(x => x.Order);
        }

        public async Task<IEnumerable<Category>> GetCategoriesByParentBasedOnProfileAsync(int categoryId)
        {
            var categories = await _categoryRepository.SelectFilterAsync(x => x.ParentId == categoryId && x.Active == (byte)BoolEnum.YES && x.VisibleApp == (byte)BoolEnum.YES);
            return categories.OrderBy(x => x.Order);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            var categories = await _categoryRepository.SelectFilterAsync(x => x.Active == (byte)BoolEnum.YES && x.VisibleApp == (byte)BoolEnum.YES);
            return categories.OrderBy(x => x.Order);
        }
    }
}
