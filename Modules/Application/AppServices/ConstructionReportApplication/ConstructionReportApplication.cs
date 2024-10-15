using Application.AppServices.ConstructionReportApplication.ViewModel;
using Application.Interfaces;
using AutoMapper;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Application.AppServices.ChecklistApplication.ViewModel;
using Application.AppServices.ChecklistApplication.Input;
using Newtonsoft.Json;
using System;
using Application.AppServices.ConstructionReportApplication.ViewPDF;
using Infra.CrossCutting.PDF.Interfaces;
using Infra.CrossCutting.BlobStorage.Interfaces;
using Infra.CrossCutting.Utils;
using System.IO;
using Domain.Entities;

namespace Application.AppServices.ConstructionReportApplication
    {
    public class ConstructionReportApplication : BaseValidationService, IConstructionReportApplication
        {
        private readonly ISmartNotification _notification;
        private readonly IConstructionDomainService _constructionDomainService;
        private readonly IConstructionReportsDomainService _constructionReportsDomainService;
        private readonly IMapper _mapper;
        private readonly ILogger<ConstructionReportApplication> _logger;
        private readonly IChecklistDomainService _checklistDomainService;
        private readonly ICategoryDomainService _categoryDomainService;
        private readonly IExportPDF _exportPDF;
        private readonly IBlob _blob;

        public ConstructionReportApplication(IConstructionDomainService constructionDomainService, ISmartNotification notification,
            IMapper mapper, ILogger<ConstructionReportApplication> logger, IConstructionReportsDomainService constructionReportsDomainService, IChecklistDomainService checklistDomainService,
            ICategoryDomainService categoryDomainService, IExportPDF exportPDF, IBlob blob
            ) : base(notification)
            {
            _constructionDomainService = constructionDomainService;
            _constructionReportsDomainService = constructionReportsDomainService;
            _categoryDomainService = categoryDomainService;
            _checklistDomainService = checklistDomainService;
            _notification = notification;
            _mapper = mapper;
            _logger = logger;
            _exportPDF = exportPDF;
            _blob = blob;
            }

        public async Task<IEnumerable<ConstructionReportViewModel>> ListReportsAsync(int userId, string constructionAppId, string path)
            {
            _logger.LogInformation($"Init list of construction reports {nameof(ListReportsAsync)}");

            if (userId == 0)
                {
                _logger.LogWarning($"Init list of construction reports with param invalid {nameof(ListReportsAsync)} with userId: {userId} and constructionId: {constructionAppId}");
                return default;
                }

            var constructionList = await _constructionDomainService.SelectFilterAsync(x => x.UserId == userId && x.AppId == constructionAppId);
            if (!constructionList.Any())
                {
                _logger.LogWarning($"Init list constructios with params {nameof(ListReportsAsync)} userId: {userId} and constructionId: {constructionAppId} doesn't find results");

                //return default;
                }

            var constructionReportsList = await _constructionReportsDomainService.SelectFilterAsync(x => x.ConstructionId == constructionList.FirstOrDefault().Id);
            if (!constructionReportsList.Any())
                {
                _logger.LogWarning($"Init list of constructios reports with params {nameof(ListReportsAsync)} userId: {userId} and constructionId: {constructionAppId} doesn't find results");

                //return default;
                }

            List<ConstructionReportViewModel> mappedList = new List<ConstructionReportViewModel>();

            foreach (var constructionReport in constructionReportsList) {
                var mappedConstructionReport = _mapper.Map<ConstructionReportViewModel>(constructionReport);
                mappedConstructionReport.Url = await _blob.GenerateUrlAsync(path, constructionReport.BlobId, 300);
                mappedList.Add((ConstructionReportViewModel)mappedConstructionReport);
                }
        
            return mappedList.OrderByDescending(x => x.CreatedAt);
            }

        public async Task<ConstructionReports> GetAsync(int userId, int constructionReportId)
            {
            _logger.LogInformation($"Init get construction reports {nameof(GetAsync)}");

            if (userId == 0)
                {
                _logger.LogWarning($"Init get construction report with param invalid {nameof(GetAsync)} with userId: {userId} and constructionReportId: {constructionReportId}");
                return default;
                }

            var constructionReports = await _constructionReportsDomainService.SelectFilterAsync(x => x.Id == constructionReportId);
            if (!constructionReports.Any())
                {
                _logger.LogWarning($"Init get construction report with params {nameof(GetAsync)} userId: {userId} and constructionReportId: {constructionReportId} doesn't find results");

                return default;
                }

            var report = constructionReports.FirstOrDefault();

            var constructionList = await _constructionDomainService.SelectFilterAsync(x => x.UserId == userId && x.Id == report.ConstructionId);
            if (!constructionList.Any())
                {
                _logger.LogWarning($"Init get construction report with params {nameof(GetAsync)} userId: {userId} and constructionId: {report.ConstructionId} doesn't find results");

                return default;
                }

            return report;
            }

            public async Task<ConstructionReportViewModel> GetReportAsync(int userId, int constructionReportId, string path)
            {
            _logger.LogInformation($"Init get construction reports {nameof(GetReportAsync)}");

            var report = await GetAsync(userId, constructionReportId);
            if(null == report)
                {
                return default;
                }
            var mappedConstructionReport = _mapper.Map<ConstructionReportViewModel>(report);
            mappedConstructionReport.Url = await _blob.GenerateUrlAsync(path, report.BlobId, 300);
            _logger.LogInformation($"Get construction reports {nameof(GetReportAsync)}");

            return mappedConstructionReport;
            }

        public async Task<bool> DeleteReportAsync(int userId, int constructionReportId, string path)
            {
            _logger.LogInformation($"Init delete construction reports {nameof(DeleteReportAsync)}");
            var deleted = false;
            var report = await GetAsync(userId, constructionReportId);
            if (null == report)
                {
                _logger.LogWarning($"Init delete construction reports with params {nameof(DeleteReportAsync)} userId: {userId} and constructionReportId: {constructionReportId} doesn't find results");
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Construction Report not found");
                return default;
                }

            await _blob.RemoveFileAsync(path, report.BlobId);
             deleted = await _constructionReportsDomainService.DeleteAsync(report.Id);

            return deleted;
            }

        public async Task<ConstructionReportViewModel> InsertAsync(int userId, ExportChecklistInput input, string path)
            {
            var viewModel = new ConstructionReportViewModel();
            if (!input.IsValid())
                {
                viewModel.ValidationResult = input.ValidationResult;
                NotifyErrorsAndValidation(_notification.EmptyPositions(), viewModel);
                _logger.LogWarning($"Export checklist with param invalid {nameof(InsertAsync)} with param: {JsonConvert.SerializeObject(input)}");
                return default;
                }

            if (userId == 0)
                {
                _logger.LogWarning($"Init list of construction reports with param invalid {nameof(ListReportsAsync)} with userId: {userId} and constructionId: {input.ConstructionAppId}");
                return default;
                }

            var constructionList = await _constructionDomainService.SelectFilterAsync(x => x.UserId == userId && x.AppId == input.ConstructionAppId);
            if (!constructionList.Any())
                {
                    _logger.LogWarning($"Init list constructions with params {nameof(ListReportsAsync)} userId: {userId} and constructionId: {input.ConstructionAppId} doesn't find results");
                    _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Construction not found");
                    return default;
                }

            var constructionReportType = await _constructionReportsDomainService.getReportType((int)input.Dados.TipoRelatorio);
            if(null != constructionReportType)
                {
                input.Dados.TituloRelatorioPDF = constructionReportType.Name;
                }

            var bytePdf = await GenerateReportPDF(input);
            var idBlob = await UploadConstructionReportToBlobStorage(new MemoryStream(bytePdf), path, userId, constructionList.FirstOrDefault().Id);
            ConstructionReports report = new ConstructionReports()
                {
                ConstructionId = constructionList.FirstOrDefault().Id,
                Title = input.Title,
                MimeType = "application/pdf",
                PicturesQuantity = input.Dados.Fotos.Length,
                Comments = input.Dados.Comentarios,
                Guarantee = input.Dados.Garantia,
                BlobId = idBlob,
                Value = input.Dados.Valor,
                Discount = input.Dados.Desconto,
                TypeId = input.Dados.TipoRelatorio,
                AssociatedDate = input.Dados.DataAssociada
                };

            var savedReport = await _constructionReportsDomainService.InsertAsync(report);

            var mappedConstructionReport = _mapper.Map<ConstructionReportViewModel>(savedReport);
            mappedConstructionReport.Url = await _blob.GenerateUrlAsync(path, idBlob, 300);

            return mappedConstructionReport;
            }

        public async Task<byte[]> GenerateReportPDF(ExportChecklistInput input)
            {
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
                return  _exportPDF.ExportHTMLToPDF(html);
            
            }

        private async Task<string> UploadConstructionReportToBlobStorage(Stream file, string folder, int userId, int constructionId)
            {
            var fileName = String.Concat("construction-report-", userId.ToString(), constructionId.ToString(),"-", DateTime.Now.ToString("dd-MM-yyyy-hh-mm"),".pdf");
            _logger.LogInformation($"Init UploadDbMobileToBlobStorage {nameof(UploadConstructionReportToBlobStorage)} - rootPath:  - folder: {folder} ");
            var blobId = await _blob.UploadFileAsync(file, folder, fileName);
            _logger.LogInformation($"BlobID: {blobId} {nameof(UploadConstructionReportToBlobStorage)}");
            return blobId;
            }
        }
    }
