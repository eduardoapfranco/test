using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Domain.Entities;
using Domain.Entities.Sqlite;
using Domain.Interfaces.UoW;
using Domain.Services;
using FizzWare.NBuilder;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using Moq;
using UnitTest.Application.UserApplication.Faker;
using Xunit;

namespace UnitTest.Domain
{
    public class DbMobileDomainServiceTest
    {
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<DbMobileDomainService>> _loggerMock;
        private DbMobileDomainService _dbMobileDomainService;
        private readonly Fixture _fixture;

        public DbMobileDomainServiceTest()
        {
            _smartNotificationMock = new Mock<ISmartNotification>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DbMobileDomainService>>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _dbMobileDomainService = new DbMobileDomainService( _unitOfWorkMock.Object, _smartNotificationMock.Object, new DomainNotificationHandler(), _loggerMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }

        [Fact(DisplayName = "Shoud return success when create and insert data db mobile")]
        [Trait("[Domain.Services]-DbMobileDomainService", "Domain-CreateDBMobileAsync")]
        public async Task ShouldReturnSuccessWhenCreateAndInsertDataDBMobile()
        {
            // arrange
            var categoriesMySql = _fixture.CreateMany<Category>();
            var checklistsMySql = _fixture.CreateMany<Checklist>();
            var categoriesSqlLite = _fixture.CreateMany<CategorySqlite>();
            var checklistsSqlLite = _fixture.CreateMany<ChecklistSqlite>();

            _unitOfWorkMock.Setup(x => x.CategorySQLite.DeleteAllAsync()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.ChecklistSQLite.DeleteAllAsync()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.CategorySQLite.InsertAllAsync(It.IsAny<IEnumerable<CategorySqlite>>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.ChecklistSQLite.InsertAllAsync(It.IsAny<IEnumerable<ChecklistSqlite>>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(new CommandResponse(true));

            _unitOfWorkMock.Setup(x => x.Category.SelectAllAsync()).ReturnsAsync(categoriesMySql);
            _unitOfWorkMock.Setup(x => x.CategorySQLite.SelectAllAsync()).ReturnsAsync(categoriesSqlLite);
            _unitOfWorkMock.Setup(x => x.Checklist.SelectAllAsync()).ReturnsAsync(checklistsMySql);
            _unitOfWorkMock.Setup(x => x.ChecklistSQLite.SelectAllAsync()).ReturnsAsync(checklistsSqlLite);

            // act
            var result = await _dbMobileDomainService.CreateDBMobileAsync(categoriesSqlLite, checklistsSqlLite);

            // assert
            Assert.True(result);
        }

        [Fact(DisplayName = "Shoud return false when not clear categories db mobile")]
        [Trait("[Domain.Services]-DbMobileDomainService", "Domain-CreateDBMobileAsync")]
        public async Task ShouldReturnFalseWhenNotClearCategoriesDBMobile()
        {
            // arrange
            var categoriesSqlLite = _fixture.CreateMany<CategorySqlite>();
            var checklistsSqlLite = _fixture.CreateMany<ChecklistSqlite>();

            _unitOfWorkMock.Setup(x => x.ChecklistSQLite.DeleteAllAsync()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.CategorySQLite.DeleteAllAsync()).ReturnsAsync(false);

            // act
            var result = await _dbMobileDomainService.CreateDBMobileAsync(categoriesSqlLite, checklistsSqlLite);

            // assert
            Assert.False(result);
        }

        [Fact(DisplayName = "Shoud return false when not clear checklists db mobile")]
        [Trait("[Domain.Services]-DbMobileDomainService", "Domain-CreateDBMobileAsync")]
        public async Task ShouldReturnFalseWhenNotClearChecklistsDBMobile()
        {
            // arrange
            var categoriesSqlLite = _fixture.CreateMany<CategorySqlite>();
            var checklistsSqlLite = _fixture.CreateMany<ChecklistSqlite>();

            _unitOfWorkMock.Setup(x => x.ChecklistSQLite.DeleteAllAsync()).ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.CategorySQLite.DeleteAllAsync()).ReturnsAsync(true);

            // act
            var result = await _dbMobileDomainService.CreateDBMobileAsync(categoriesSqlLite, checklistsSqlLite);

            // assert
            Assert.False(result);
        }

        [Fact(DisplayName = "Shoud return false when not insert categories db mobile")]
        [Trait("[Domain.Services]-DbMobileDomainService", "Domain-CreateDBMobileAsync")]
        public async Task ShouldReturnFalseWhenNotInsertCategoriesDBMobile()
        {
            // arrange
            var categoriesSqlLite = _fixture.CreateMany<CategorySqlite>();
            var checklistsSqlLite = _fixture.CreateMany<ChecklistSqlite>();


            _unitOfWorkMock.Setup(x => x.ChecklistSQLite.DeleteAllAsync()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.CategorySQLite.DeleteAllAsync()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.CategorySQLite.InsertAllAsync(It.IsAny<IEnumerable<CategorySqlite>>())).ReturnsAsync(false);


            // act
            var result = await _dbMobileDomainService.CreateDBMobileAsync(categoriesSqlLite, checklistsSqlLite);

            // assert
            Assert.False(result);
        }

        [Fact(DisplayName = "Shoud return false when not insert checklists db mobile")]
        [Trait("[Domain.Services]-DbMobileDomainService", "Domain-CreateDBMobileAsync")]
        public async Task ShouldReturnFalseWhenNotInsertChecklistsDBMobile()
        {
            // arrange
            var categoriesSqlLite = _fixture.CreateMany<CategorySqlite>();
            var checklistsSqlLite = _fixture.CreateMany<ChecklistSqlite>();

            _unitOfWorkMock.Setup(x => x.ChecklistSQLite.DeleteAllAsync()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.CategorySQLite.DeleteAllAsync()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.CategorySQLite.InsertAllAsync(It.IsAny<IEnumerable<CategorySqlite>>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.ChecklistSQLite.InsertAllAsync(It.IsAny<IEnumerable<ChecklistSqlite>>())).ReturnsAsync(false);


            // act
            var result = await _dbMobileDomainService.CreateDBMobileAsync(categoriesSqlLite, checklistsSqlLite);

            // assert
            Assert.False(result);
        }

        [Fact(DisplayName = "Shoud return false when categories mysql different categories db mobile")]
        [Trait("[Domain.Services]-DbMobileDomainService", "Domain-CreateDBMobileAsync")]
        public async Task ShouldReturnFalseWhenCategoriesMySqlDifferentCategoriesDBMobile()
        {
            // arrange
            var categoriesMySql = Builder<Category>.CreateListOfSize(2).Build();
            var categoriesSqlLite = _fixture.CreateMany<CategorySqlite>();
            var checklistsSqlLite = _fixture.CreateMany<ChecklistSqlite>();


            _unitOfWorkMock.Setup(x => x.ChecklistSQLite.DeleteAllAsync()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.CategorySQLite.DeleteAllAsync()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.CategorySQLite.InsertAllAsync(It.IsAny<IEnumerable<CategorySqlite>>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.ChecklistSQLite.InsertAllAsync(It.IsAny<IEnumerable<ChecklistSqlite>>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(new CommandResponse(true));

            _unitOfWorkMock.Setup(x => x.Category.SelectAllAsync()).ReturnsAsync(categoriesMySql);
            _unitOfWorkMock.Setup(x => x.CategorySQLite.SelectAllAsync()).ReturnsAsync(categoriesSqlLite);

            // act
            var result = await _dbMobileDomainService.CreateDBMobileAsync(categoriesSqlLite, checklistsSqlLite);

            // assert
            Assert.False(result);
        }

        [Fact(DisplayName = "Shoud return false when checklists mysql different checklists db mobile")]
        [Trait("[Domain.Services]-DbMobileDomainService", "Domain-CreateDBMobileAsync")]
        public async Task ShouldReturnFalseWhenChecklistsMySqlDifferentChecklistsDBMobile()
        {
            // arrange
            var checklistsMySql = Builder<Checklist>.CreateListOfSize(2).Build();
            var categoriesMySql = _fixture.CreateMany<Category>();
            var categoriesSqlLite = _fixture.CreateMany<CategorySqlite>();
            var checklistsSqlLite = _fixture.CreateMany<ChecklistSqlite>();


            _unitOfWorkMock.Setup(x => x.ChecklistSQLite.DeleteAllAsync()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.CategorySQLite.DeleteAllAsync()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.CategorySQLite.InsertAllAsync(It.IsAny<IEnumerable<CategorySqlite>>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.ChecklistSQLite.InsertAllAsync(It.IsAny<IEnumerable<ChecklistSqlite>>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(new CommandResponse(true));

            _unitOfWorkMock.Setup(x => x.ChecklistSQLite.SelectAllAsync()).ReturnsAsync(checklistsSqlLite);
            _unitOfWorkMock.Setup(x => x.Category.SelectAllAsync()).ReturnsAsync(categoriesMySql);
            _unitOfWorkMock.Setup(x => x.Checklist.SelectAllAsync()).ReturnsAsync(checklistsMySql);
            _unitOfWorkMock.Setup(x => x.CategorySQLite.SelectAllAsync()).ReturnsAsync(categoriesSqlLite);

            // act
            var result = await _dbMobileDomainService.CreateDBMobileAsync(categoriesSqlLite, checklistsSqlLite);

            // assert
            Assert.False(result);
        }
    }
}
