using Application.AppServices.ChecklistApplication.Input;
using Application.AppServices.ChecklistApplication.ViewModel;
using Application.AppServices.ConstructionReportApplication.ViewPDF;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.Services;
using Domain.ValueObjects;
using Infra.CrossCutting.Auth.Intefaces;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.PDF.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.AppServices.ChecklistApplication
{
    public class ChecklistApplication : BaseValidationService, IChecklistApplication
    {
        private readonly ISmartNotification _notification;
        private readonly IChecklistDomainService _checklistDomainService;
        private readonly ICategoryDomainService _categoryDomainService;
        private readonly IExportPDF _exportPDF;
        private readonly IAuthService<User> _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<ChecklistApplication> _logger;

        public ChecklistApplication(
            IChecklistDomainService checklistDomainService,
            ICategoryDomainService categoryDomainService,
            ISmartNotification notification,
            IMapper mapper, IAuthService<User> authService, ILogger<ChecklistApplication> logger, IExportPDF exportPDF) : base(notification)
        {
            _checklistDomainService = checklistDomainService;
            _notification = notification;
            _mapper = mapper;
            _authService = authService;
            _logger = logger;
            _exportPDF = exportPDF;
            _categoryDomainService = categoryDomainService;
        }

        public async Task<IEnumerable<ChecklistViewModel>> GetChecklistsLastDateUpdatedAsync(DateTime? lastDateSync)
        {
            if (!lastDateSync.HasValue)
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "O parâmetro lastDateSync é obrigatório");
                _logger.LogWarning($"Param invalid {nameof(GetChecklistsLastDateUpdatedAsync)}");

                return default;
            }
            var listChecklists = await _checklistDomainService.SelectFilterAsync(x => x.UpdatedAt >= lastDateSync);

            if (listChecklists.Any())
            {
                var mappedListChecklists = _mapper.Map<IEnumerable<ChecklistViewModel>>(listChecklists);
                return mappedListChecklists;
            }
            return new List<ChecklistViewModel>();
        }

        public async Task<ExportChecklistPDFViewModel> ExportChecklistsToPDF(ExportChecklistInput input)
        {
            var viewModel = new ExportChecklistPDFViewModel();
            if (!input.IsValid())
            {
                viewModel.ValidationResult = input.ValidationResult;
                NotifyErrorsAndValidation(_notification.EmptyPositions(), viewModel);
                _logger.LogWarning($"Export checklist with param invalid {nameof(ExportChecklistsToPDF)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
            }

            var checklists = await _checklistDomainService.SelectExportSectionPDF(input.CategoryId, input.Ids, input.Type);
            var category = await _categoryDomainService.SelectByIdAsync(input.CategoryId);

            if (category == null)
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Categoria não encontrada para exportação de checklists");
                return default;
            }

            if (!checklists.Any())
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), $"A categoria {category.Title} não possui checklists para exportação");
                return default;
            }

            var html = String.Empty;
            if (null == input.Dados)
            {
                html = CreateLayoutHtmlExportPdf.Create(checklists, category);
            }
            else
            {
                html = CreateLayoutHtmlExportWithConstructionInfoPdf.Create(checklists, category, input.Dados);
            }
            viewModel.Pdf = _exportPDF.ExportHTMLToPDF(html);
            return viewModel;
        }

        public async Task<ExportChecklistViewModel> ExportChecklists(ExportChecklistInput input)
        {
            var viewModel = new ExportChecklistViewModel();
            if (!input.IsValid())
            {
                viewModel.ValidationResult = input.ValidationResult;
                NotifyErrorsAndValidation(_notification.EmptyPositions(), viewModel);
                _logger.LogWarning($"Export checklist with param invalid {nameof(ExportChecklistsToPDF)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
            }

            var checklists = await _checklistDomainService.SelectExportSectionPDF(input.CategoryId, input.Ids, input.Type);
            var category = await _categoryDomainService.SelectByIdAsync(input.CategoryId);

            if (category == null)
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Categoria não encontrada para exportação de checklists");
                return default;
            }

            var checklistSectionExportVos = checklists as ChecklistSectionExportVO[] ?? checklists.ToArray();
            if (!checklistSectionExportVos.Any())
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), $"A categoria {category.Title} não possui checklists para exportação");
                return default;
            }

            viewModel.Checklists = checklistSectionExportVos;
            return viewModel;
        }

        public async Task<IEnumerable<ChecklistViewModel>> SelectByCategoryIdAsync(int categoryId)
        {
            var checklists = await _checklistDomainService.SelectFilterAsync(x =>
                x.CategoryId == categoryId && x.Active == (byte) BoolEnum.YES && x.VisibleApp == (byte) BoolEnum.YES);
            checklists = checklists.OrderBy(x => x.Order);
            return _mapper.Map<IEnumerable<ChecklistViewModel>>(checklists);
        }
    }
}