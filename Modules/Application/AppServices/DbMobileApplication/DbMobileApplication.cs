using Application.AppServices.DbMobileApplication.Input;
using Application.AppServices.DbMobileApplication.ViewModel;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Sqlite;
using Domain.Enum;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Messages;
using Infra.CrossCutting.Auth;
using Infra.CrossCutting.BlobStorage.Interfaces;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Infra.CrossCutting.Utils;
using Ionic.Zip;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Application.AppServices.DbMobileApplication
{
    public class DbMobileApplication : BaseValidationService, IDbMobileApplication
    {
        private readonly ISmartNotification _notification;
        private readonly IDbMobileDomainService _dbMobileDomainService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IChecklistRepository _checklistRepository;
        private readonly IFileBlobStorageRepository _fileBlobStorageRepository;
        private readonly IBlob _blob;
        private readonly IMapper _mapper;
        ILogger<DbMobileApplication> _logger;


        public DbMobileApplication(IDbMobileDomainService dbMobileDomainService,
            ICategoryRepository categoryRepository,
            IChecklistRepository checklistRepository,
            IFileBlobStorageRepository fileBlobStorageRepository,
            IBlob blob,
            IMapper mapper,
            ISmartNotification notification,
            ILogger<DbMobileApplication> logger) : base(notification)
        {
            _dbMobileDomainService = dbMobileDomainService;
            _categoryRepository = categoryRepository;
            _checklistRepository = checklistRepository;
            _fileBlobStorageRepository = fileBlobStorageRepository;
            _blob = blob;
            _notification = notification;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DbMobileViewModel> CreateDBMobileAsync(DbMobileInput input, string rootPath, string folder, string path = "")
        {
            var viewModel = new DbMobileViewModel();

            _logger.LogInformation($"Init process {nameof(CreateDBMobileAsync)}");
            if (!input.IsValid())
            {
                viewModel = _mapper.Map<DbMobileViewModel>(input);
                NotifyErrorsAndValidation(_notification.EmptyPositions(), viewModel);
                _logger.LogWarning($"Error for validate params {nameof(CreateDBMobileAsync)} params: {JsonConvert.SerializeObject(viewModel)}");
                return viewModel;
            }

            if (input.Secret != AuthSettings.SecretGenerateDB)
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), DbMobileMessages.SecretInvalid);
                _logger.LogWarning($"Error for validate secret {nameof(CreateDBMobileAsync)} params: {JsonConvert.SerializeObject(viewModel)}");
                return viewModel;
            }

            var categories = await _categoryRepository.SelectAllAsync();
            if (categories == null || !categories.Any())
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao obter categorias.");
                _logger.LogWarning($"Error to get categories {nameof(CreateDBMobileAsync)} ");
                return viewModel;
            }

            var checklists = await _checklistRepository.SelectAllAsync();
            if (checklists == null || !checklists.Any())
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao obter checklists.");
                _logger.LogWarning($"Error to get checklists {nameof(CreateDBMobileAsync)} ");
                return viewModel;
            }

            var categoriesParam = _mapper.Map<IEnumerable<CategorySqlite>>(categories);
            var checklistsParam = _mapper.Map<IEnumerable<ChecklistSqlite>>(checklists);
            // create DbMobile with image categories offline
            await _dbMobileDomainService.CreateDBMobileAsync(categoriesParam, checklistsParam);
            var blobIdWithCategoriesImageLocal = await UploadDbMobileToBlobStorage(rootPath, folder);
            // create DbMobile with image categories online prepare to zip
            await _dbMobileDomainService.CreateDBMobileAsync(categoriesParam, checklistsParam, true);

            using (ZipFile zipFile = new ZipFile())
            {
                try
                {
                    zipFile.AddFile("construa.bd", "");
                    zipFile.Save("construa.bd.zip");
                }
                catch
                {
                    throw;
                }
            }

            var blobIdZip = await UploadDbMobileToBlobStorage(rootPath, folder, true);

            viewModel.DownloadUrl = await _blob.GenerateUrlAsync(folder, blobIdWithCategoriesImageLocal, 300);
            viewModel.DownloadUrlZip = await _blob.GenerateUrlAsync(folder, blobIdZip, 300);
            return viewModel;
        }

        public async Task DeleteDBMobileAsync(string folder, bool deleteZip = false)
        {
            try
            {
                var dbs = await _fileBlobStorageRepository.SelectFilterAsync(x => x.CreatedAt.Date < DateTime.Now.Date.AddDays(-7) && x.Zip == deleteZip);
                _logger.LogInformation($"DB for deletes {JsonConvert.SerializeObject(dbs)}");

                foreach (var db in dbs)
                {
                    await _fileBlobStorageRepository.DeleteAsync(db.Id);
                    _logger.LogInformation($"Deleted register db mobile in database mysql {nameof(CreateDBMobileAsync)} - id: {db.Id} - blobId: {db.BlobId} - folder: {folder} ");

                    var getBlob = await _blob.BlobExistsAsync(folder, db.BlobId);

                    if (getBlob)
                    {
                        await _blob.RemoveFileAsync(folder, db.BlobId);
                        _logger.LogInformation(
                            $"Deleted db mobile in blob storage {nameof(CreateDBMobileAsync)} - id: {db.Id} - blobId: {db.BlobId} - folder: {folder} ");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to execute delete db mobile in database mysql or blob storage");
            }
        }

        public async Task<DbMobileViewModel> GetLastDBMobileGeneratedAsync(string folder)
        {
            var dbViewModel = new DbMobileViewModel();
            var dbs = await _fileBlobStorageRepository.SelectFilterAsync(x => x.Zip == false);
            var dbsZip = await _fileBlobStorageRepository.SelectFilterAsync(x => x.Zip == true);

            if (!dbs.Any())
            {
                _notification.NewNotificationBadRequest(_notification.EmptyPositions(), DbMobileMessages.DbNotFound);
                return dbViewModel;
            }

            var getDb = dbs.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            var getDbZip = dbsZip.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            dbViewModel.DownloadUrl = await _blob.GenerateUrlAsync(folder, getDb.BlobId, 300);

            if (dbsZip.Any())
            {
                dbViewModel.DownloadUrlZip = await _blob.GenerateUrlAsync(folder, getDbZip.BlobId, 300);
            }

            return dbViewModel;
        }

        private async Task<string> UploadDbMobileToBlobStorage(string rootPath, string folder, bool zip = false)
        {
            var nameDb = "construa.bd" + ((zip) ? ".zip" : null);
            var zipExtension = (zip) ? ".zip" : "";
            _logger.LogInformation($"Init UploadDbMobileToBlobStorage {nameof(UploadDbMobileToBlobStorage)} - rootPath: {rootPath} - folder: {folder} ");
            var mimeType = FileInfoUtil.GetFileMimeType(rootPath + zipExtension);
            _logger.LogInformation($"MimeType: {mimeType} {nameof(UploadDbMobileToBlobStorage)}");
            var size = FileInfoUtil.GetFileSize(rootPath + zipExtension);
            _logger.LogInformation($"MimeType: {size} {nameof(UploadDbMobileToBlobStorage)}");
            var stream = FileInfoUtil.GetFileStream(rootPath + zipExtension);
            var blobId = await _blob.UploadFileAsync(stream, folder, nameDb);
            _logger.LogInformation($"BlobID: {blobId} {nameof(UploadDbMobileToBlobStorage)}");
            var fileBlob = new FileBlobStorage(nameDb, blobId, size.ToString(), mimeType, FileUploadOriginEnum.DB_MOBILE_GENERATOR.ToString(), zip);
            _logger.LogInformation($"Prepare FileBlobStorage: {JsonConvert.SerializeObject(fileBlob)} {nameof(UploadDbMobileToBlobStorage)}");
            await _fileBlobStorageRepository.InsertAsync(fileBlob);
            return blobId;
        }

        public async Task<LastUpdatedDatesViewModel> GetLastUpdatedDatesAsync()
        {
            var lastUpdatedChecklist = await _checklistRepository.GetLastUpdated();
            var lastUpdatedCategory = await _categoryRepository.GetLastUpdated();
            var lastUpdatedDates = new LastUpdatedDatesViewModel()
            {
                LastDateChecklist = lastUpdatedChecklist.UpdatedAt.ToUniversalTime(),
                LastDateCategory = lastUpdatedCategory.UpdatedAt.ToUniversalTime()
            };

            return lastUpdatedDates;
        }
    }
}
