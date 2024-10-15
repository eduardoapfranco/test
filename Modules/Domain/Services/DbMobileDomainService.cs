using Domain.Entities;
using Domain.Entities.Sqlite;
using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.Notification.Model;
using Infra.CrossCutting.UoW.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class DbMobileDomainService : BaseUnitOfWork<IUnitOfWork>, IDbMobileDomainService
    {
        protected readonly IUnitOfWork _unitOfWork;
        private ISmartNotification _notification;
        ILogger<DbMobileDomainService> _logger;


        public DbMobileDomainService(IUnitOfWork unitOfWork, ISmartNotification notification, INotificationHandler<DomainNotification> messageHandler, ILogger<DbMobileDomainService> logger) : base(unitOfWork, messageHandler)
        {
            _unitOfWork = unitOfWork;
            _notification = notification;
            _logger = logger;
        }

        public async Task<bool> CreateDBMobileAsync(IEnumerable<CategorySqlite> categories, IEnumerable<ChecklistSqlite> checklists, bool imageCategoryIsOnline = false)
        {
            try
            {
               
                _logger.LogInformation($"Init process {nameof(CreateDBMobileAsync)}");
                await using (_unitOfWork.BeginTransaction())
                {
                    _logger.LogInformation($"Init transaction {nameof(CreateDBMobileAsync)} MySQL");
                    await using (_unitOfWork.BeginTransactionSQLite())
                    {
                        _logger.LogInformation($"Init transaction {nameof(CreateDBMobileAsync)} SQLite");
                        var deleteChecklists = await DeleteAllChecklistsSqlite();
                        Commit();
                        if (!deleteChecklists)
                        {
                            _logger.LogWarning($"Error for delete checklists {nameof(CreateDBMobileAsync)} SQLite");
                            _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao limpar checklists");
                            return false;
                        }

                        await _unitOfWork.ChecklistSQLite.ExecuteVACUUMForSqlite();
                        Commit();

                        var deleteCategories = await DeleteAllCategoriesSqlite();
                        Commit();
                        if (!deleteCategories)
                        {
                            _logger.LogWarning($"Error for delete categories {nameof(CreateDBMobileAsync)} SQLite");
                            _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao limpar categorias");
                            return false;
                        }

                        await _unitOfWork.CategorySQLite.ExecuteVACUUMForSqlite();
                        Commit();

                        var insertCategories = await InsertAllCategoriesSqlite(categories, imageCategoryIsOnline);
                        Commit();
                        if (!insertCategories)
                        {
                            _logger.LogWarning($"Error for insert all categories {nameof(CreateDBMobileAsync)} SQLite");
                            _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao inserir todas as categorias");
                            return false;
                        }

                        var insertChecklists = await InsertAllChecklistsSqlite(checklists);
                        Commit();
                        if (!insertChecklists)
                        {
                            _logger.LogWarning($"Error for insert all checklists {nameof(CreateDBMobileAsync)} SQLite");
                            _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao inserir todos os checklists");
                            return false;
                        }

                        _logger.LogInformation($"Init validate checker complete categories {nameof(CreateDBMobileAsync)}");
                        var checkerIsCompleteCategories = await IsCompleteGenerateCategoriesSqlite();
                        if (!checkerIsCompleteCategories)
                        {
                            await DeleteAllCategoriesSqlite();
                            Commit();
                            _logger.LogInformation($"Delete all categories because error for migrate all categories {nameof(CreateDBMobileAsync)}");
                            _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao migrar categorias");
                            return false;
                        }
                        _logger.LogInformation($"Init validate checker complete checklists {nameof(CreateDBMobileAsync)}");
                        var checkerIsCompleteChecklists = await IsCompleteGenerateChecklistsSqlite();
                        if (!checkerIsCompleteChecklists)
                        {
                            await DeleteAllChecklistsSqlite();
                            Commit();
                            await DeleteAllCategoriesSqlite();
                            Commit();
                            _logger.LogInformation($"Delete all categories and checklists because error for migrate all checklists {nameof(CreateDBMobileAsync)}");
                            _notification.NewNotificationBadRequest(_notification.EmptyPositions(), "Erro ao migrar checklists");
                            return false;
                        }
                        return true;
                    }
                }                
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error for - {nameof(CreateDBMobileAsync)}: {JsonConvert.SerializeObject(ex)}");
                return false;
            }
            
        }

        private Task<bool> DeleteAllCategoriesSqlite()
        {
            try
            {
                var delete = _unitOfWork.CategorySQLite.DeleteAllAsync();
                _unitOfWork.SaveChangesSQLiteAsync();
                return delete;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error for delele all categories: {nameof(DeleteAllCategoriesSqlite)}, {JsonConvert.SerializeObject(ex)}");
                return Task.FromResult(false);
            }           
        }

        private Task<bool> DeleteAllChecklistsSqlite()
        {
            try
            {
                var delete = _unitOfWork.ChecklistSQLite.DeleteAllAsync();
                _unitOfWork.SaveChangesSQLiteAsync();
                return delete;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error for delele all checklists: {nameof(DeleteAllChecklistsSqlite)}, {JsonConvert.SerializeObject(ex)}");
                return Task.FromResult(false);
            }         
           
        }

        private Task<bool> InsertAllCategoriesSqlite(IEnumerable<CategorySqlite> categories, bool imageCategoryIsOnline = false)
        {
            try
            {
                if(imageCategoryIsOnline)
                {
                    foreach (var item in categories)
                    {
                        if (!string.IsNullOrEmpty(item.Image))
                        {
                            item.Image = "api/v1/categories/" + item.Id;
                        }
                    }
                }              
                   
                var insert = _unitOfWork.CategorySQLite.InsertAllAsync(categories);
                _unitOfWork.SaveChangesSQLiteAsync();
                return insert;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error for insert all categories: {nameof(InsertAllCategoriesSqlite)}, {JsonConvert.SerializeObject(ex)}");
                return Task.FromResult(false);
            }                     
        }

        private Task<bool> InsertAllChecklistsSqlite(IEnumerable<ChecklistSqlite> checklists)
        {
            try
            {
                var insert = _unitOfWork.ChecklistSQLite.InsertAllAsync(checklists);
                _unitOfWork.SaveChangesSQLiteAsync();
                return insert;
            }
            catch (System.Exception ex)
            {

                _logger.LogError($"Error for insert all checklists: {nameof(InsertAllChecklistsSqlite)}, {JsonConvert.SerializeObject(ex)}");
                return Task.FromResult(false);
            }                    
        }

        private async Task<bool> IsCompleteGenerateCategoriesSqlite()
        {
            var categoriesMySql = await _unitOfWork.Category.SelectAllAsync();
            var categoriesSqlite = await _unitOfWork.CategorySQLite.SelectAllAsync();
            var countCategoriesMysql = categoriesMySql.Count();
            _logger.LogInformation($"Total Categories MySQL: {countCategoriesMysql} - {nameof(IsCompleteGenerateCategoriesSqlite)}");
            var countCategoriesSqlite = categoriesSqlite.Count();
            _logger.LogInformation($"Total Categories SQLite: {countCategoriesSqlite} - {nameof(IsCompleteGenerateCategoriesSqlite)}");
            if (countCategoriesMysql != countCategoriesSqlite)
            {
                return false;
            } 
            return true;
        }

        private async Task<bool> IsCompleteGenerateChecklistsSqlite()
        {
            var checklistsMySql = await _unitOfWork.Checklist.SelectAllAsync();
            var checklistsSqlite = await _unitOfWork.ChecklistSQLite.SelectAllAsync();
            var countChecklistsMysql = checklistsMySql.Count();
            _logger.LogInformation($"Total Checklists MySQL: {countChecklistsMysql} - {nameof(IsCompleteGenerateChecklistsSqlite)}");
            var countChecklistsSqlite = checklistsSqlite.Count();
            _logger.LogInformation($"Total Checklists SQLite: {countChecklistsSqlite} - {nameof(IsCompleteGenerateChecklistsSqlite)}");
            if (countChecklistsMysql != countChecklistsSqlite)
            {
                return false;
            }
            return true;
        }
    }
}
