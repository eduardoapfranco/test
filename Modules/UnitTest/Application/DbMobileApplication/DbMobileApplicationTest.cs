using Application.AppServices.DbMobileApplication;
using Application.AppServices.DbMobileApplication.Input;
using Application.AppServices.DbMobileApplication.ViewModel;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Sqlite;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Auth;
using Infra.CrossCutting.BlobStorage.Interfaces;
using Infra.CrossCutting.Email.Interfaces;
using Infra.CrossCutting.Notification.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using App = Application.AppServices;

namespace UnitTest.Application.UserApplication
{
    public class DbMobileApplicationTest
    {
        private Mock<IDbMobileDomainService> _dbMobileDomainServiceMock;
        private Mock<ICategoryRepository> _categoryRepositorykMock;
        private Mock<IChecklistRepository> _checklistRepositoryMock;
        private Mock<IFileBlobStorageRepository> _fileBlobStorageMock;
        private Mock<IBlob> _blobMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private App.DbMobileApplication.DbMobileApplication _dbMobileApplication;
        private readonly Fixture _fixture;
        private Mock<ILogger<DbMobileApplication>> _loggerMock;

        public DbMobileApplicationTest()
        {
            // configure
            _dbMobileDomainServiceMock = new Mock<IDbMobileDomainService>();
            _categoryRepositorykMock = new Mock<ICategoryRepository>();
            _checklistRepositoryMock = new Mock<IChecklistRepository>();
            _fileBlobStorageMock = new Mock<IFileBlobStorageRepository>();
            _blobMock = new Mock<IBlob>();
            _mapperMock = new Mock<IMapper>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _loggerMock = new Mock<ILogger<DbMobileApplication>>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _dbMobileApplication = new App.DbMobileApplication.DbMobileApplication(_dbMobileDomainServiceMock.Object, _categoryRepositorykMock.Object, _checklistRepositoryMock.Object, _fileBlobStorageMock.Object, _blobMock.Object, _mapperMock.Object, _smartNotificationMock.Object, _loggerMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }
      

        [Fact(DisplayName = "Shoud return fields invalids create db mobile async")]
        [Trait("[Application.AppServices]-DbMobileApplication", "Application-CreateDBMobile")]
        public async Task ShouldReturnFieldsInvalidsCreateDBMobileAsync()
        {
            // arrange
            var dbMobileInput = new DbMobileInput() { Secret = null };
            var dbViewModel = _fixture.Create<DbMobileViewModel>();
            _mapperMock.Setup(x => x.Map<DbMobileViewModel>(dbMobileInput)).Returns(dbViewModel);

            // act
            var result = await _dbMobileApplication.CreateDBMobileAsync(dbMobileInput, "rootPath", "folder-test");

            // assert
            Assert.NotNull(result);
            Assert.NotNull(result.ValidationResult);
        }

        [Fact(DisplayName = "Shoud return invalid secret create db mobile async")]
        [Trait("[Application.AppServices]-DbMobileApplication", "Application-CreateDBMobile")]
        public async Task ShouldReturnSecretInvalidCreateDBMobileAsync()
        {
            // arrange
            var dbMobileInput = new DbMobileInput() { Secret = AuthSettings.Secret };

            // act
            var result = await _dbMobileApplication.CreateDBMobileAsync(dbMobileInput, "rootPath", "folder-test");

            // assert
            Assert.NotNull(result);
            Assert.Null(result.ValidationResult);
        }

        [Fact(DisplayName = "Shoud return null when not found categories create db mobile async")]
        [Trait("[Application.AppServices]-DbMobileApplication", "Application-CreateDBMobile")]
        public async Task ShouldReturnNullWhenNotFoundCategoriesCreateDBMobileAsync()
        {
            // arrange
            var dbMobileInput = new DbMobileInput() { Secret = AuthSettings.SecretGenerateDB };
            var categoriesMySql = _fixture.CreateMany<Category>();
            var categoriesSqlite = _fixture.CreateMany<CategorySqlite>();
            _categoryRepositorykMock.Setup(x => x.SelectAllAsync()).ReturnsAsync(new List<Category>());
            _mapperMock.Setup(x => x.Map<IEnumerable<CategorySqlite>>(categoriesMySql)).Returns(categoriesSqlite);

            // act
            var result = await _dbMobileApplication.CreateDBMobileAsync(dbMobileInput, "rootPath", "folder-test");

            // assert
            Assert.NotNull(result);
            Assert.Null(result.ValidationResult);
        }

        [Fact(DisplayName = "Shoud return null when not found checklists create db mobile async")]
        [Trait("[Application.AppServices]-DbMobileApplication", "Application-CreateDBMobile")]
        public async Task ShouldReturnNullWhenNotFoundChecklistsCreateDBMobileAsync()
        {
            // arrange
            var dbMobileInput = new DbMobileInput() { Secret = AuthSettings.SecretGenerateDB };
            var categoriesMySql = _fixture.CreateMany<Category>();
            var categoriesSqlite = _fixture.CreateMany<CategorySqlite>();
            _categoryRepositorykMock.Setup(x => x.SelectAllAsync()).ReturnsAsync(categoriesMySql);
            _checklistRepositoryMock.Setup(x => x.SelectAllAsync()).ReturnsAsync(new List<Checklist>());
            _mapperMock.Setup(x => x.Map<IEnumerable<CategorySqlite>>(categoriesMySql)).Returns(categoriesSqlite);

            // act
            var result = await _dbMobileApplication.CreateDBMobileAsync(dbMobileInput, "rootPath", "folder-test");

            // assert
            Assert.NotNull(result);
            Assert.Null(result.ValidationResult);
        }

        [Fact(DisplayName = "Shoud return null when not found db mobile for download")]
        [Trait("[Application.AppServices]-DbMobileApplication", "Application-GetLastDBMobileGeneratedAsync")]
        public async Task ShouldReturnNullWhenNotFoundDbMobileForDownloadGetLastDBMobileGeneratedAsync()
        {
            // arrange
            _fileBlobStorageMock.Setup(x => x.SelectAllAsync()).ReturnsAsync(new List<FileBlobStorage>());
            // act
            var result = await _dbMobileApplication.GetLastDBMobileGeneratedAsync("folder-test");

            // assert
            Assert.NotNull(result);
            Assert.Null(result.ValidationResult);
        }

        [Fact(DisplayName = "Shoud return last updated dates async")]
        [Trait("[Application.AppServices]-DbMobileApplication", "Application-GetLastUpdatedDatesAsync")]
        public async Task ShouldReturnLastUpdatedDatesAsync()
        {
            // arrange
            var categoriesMySql = _fixture.CreateMany<Category>();
            var checklistsMySql = _fixture.CreateMany<Checklist>();
            _categoryRepositorykMock.Setup(x => x.GetLastUpdated()).ReturnsAsync(
                        categoriesMySql.OrderByDescending(x => x.UpdatedAt)
                        .FirstOrDefault());
            _checklistRepositoryMock.Setup(x => x.GetLastUpdated()).ReturnsAsync(
                        checklistsMySql.OrderByDescending(x => x.UpdatedAt)
                        .FirstOrDefault());

            // act
            var result = await _dbMobileApplication.GetLastUpdatedDatesAsync();

            // assert
            Assert.NotNull(result);
            Assert.Null(result.ValidationResult);
        }

        [Fact(DisplayName = "Shoud delete db mobile created less than 7 days")]
        [Trait("[Application.AppServices]-DbMobileApplication", "Application-DeleteDBMobileAsync")]
        public async Task ShouldReturnSuccessDeleteDBMobileAsync()
        {
            // arrange
            var filesBlobStorage = _fixture.CreateMany<FileBlobStorage>();
            var count = filesBlobStorage.Count();
            _fileBlobStorageMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<FileBlobStorage, bool>>>())).ReturnsAsync(filesBlobStorage);
            _fileBlobStorageMock.Setup(x => x.DeleteAsync(It.IsAny<long>())).ReturnsAsync(true);
            _blobMock.Setup(x => x.BlobExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            _blobMock.Setup(x => x.RemoveFileAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // act
            await _dbMobileApplication.DeleteDBMobileAsync("folder-test");

            // assert
            _fileBlobStorageMock.Verify(x => x.SelectFilterAsync(It.IsAny<Expression<Func<FileBlobStorage, bool>>>()), Times.Once);
            _fileBlobStorageMock.Verify(x => x.DeleteAsync(It.IsAny<long>()), Times.AtLeast(count));
            _blobMock.Verify(x => x.BlobExistsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeast(count));
            _blobMock.Verify(x => x.RemoveFileAsync(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeast(count));
        }
    }
}
