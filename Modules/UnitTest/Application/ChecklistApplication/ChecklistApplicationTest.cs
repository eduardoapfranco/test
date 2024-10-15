using Application.AppServices.ChecklistApplication.Input;
using Application.AppServices.ChecklistApplication.ViewModel;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Domain.Core;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.Services;
using Domain.ValueObjects;
using FluentValidation.Results;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.PDF.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnitTest.Application.ChecklistApplication.Faker;
using Xunit;
using App = Application.AppServices;

namespace UnitTest.Application.ChecklistApplication
{
    public class ChecklistApplicationTest
    {
        private Mock<IChecklistDomainService> _checklistDomainServiceMock;
        private Mock<ICategoryDomainService> _categoryDomainServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IExportPDF> _exportPDF;
        private App.ChecklistApplication.ChecklistApplication _checklistApplication;
        private Mock<ILogger<App.ChecklistApplication.ChecklistApplication>> _loggerMock;
        private AuthService _authService;
        private readonly Fixture _fixture;

        public ChecklistApplicationTest()
        {
            // configure
            _checklistDomainServiceMock = new Mock<IChecklistDomainService>();
            _categoryDomainServiceMock = new Mock<ICategoryDomainService>();
            _mapperMock = new Mock<IMapper>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _exportPDF = new Mock<IExportPDF>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _authService = new AuthService();
            _loggerMock = new Mock<ILogger<App.ChecklistApplication.ChecklistApplication>>();
            _checklistApplication = new App.ChecklistApplication.ChecklistApplication(_checklistDomainServiceMock.Object, _categoryDomainServiceMock.Object, _smartNotificationMock.Object, _mapperMock.Object, _authService, _loggerMock.Object, _exportPDF.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }


        [Fact(DisplayName = "Shoud return empty when last date sync is greater than last upadted date of checklists async")]
        [Trait("[Application.AppServices]-ChecklistApplication", "Application-GetChecklistsLastDateUpdatedAsync")]
        public async Task ShouldReturnEmptyWhenLastDaySyncIsGreaterThanlastUpdatedDateOfChecklistsAsync()
        {
            // arrange
            DateTime? lastDateSync = DateTime.Today.AddDays(1);
            var checklistList = ChecklistFaker.CreateListChecklist();
            _checklistDomainServiceMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Checklist, bool>>>())).ReturnsAsync(checklistList);

            // act
            var result = await _checklistApplication.GetChecklistsLastDateUpdatedAsync(lastDateSync);

            // assert
            Assert.Empty(result);
        }

        [Fact(DisplayName = "Shoud return null when last date sync is null async")]
        [Trait("[Application.AppServices]-ChecklistApplication", "Application-GetChecklistsLastDateUpdatedAsync")]
        public async Task ShouldReturnNullWhenLastDaySyncIsNullAsync()
        {
            // arrange
            DateTime? lastDateSync = null;
            var checklistList = ChecklistFaker.CreateListChecklist();
            _checklistDomainServiceMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Checklist, bool>>>())).ReturnsAsync(checklistList);

            // act
            var result = await _checklistApplication.GetChecklistsLastDateUpdatedAsync(lastDateSync);

            // assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "Shoud return result list when last date sync is greater than last updated date of checklists async")]
        [Trait("[Application.AppServices]-ChecklistApplication", "Application-GetChecklistsLastDateUpdatedAsync")]
        public async Task ShouldReturnResultListWhenLastDaySyncIsLessThanlastUpdatedDateOfChecklistsAsync()
        {
            // arrange
            DateTime? lastDateSync = DateTime.Today.AddDays(-2);
            var checklistList = ChecklistFaker.CreateListChecklist();
            var checklistListViewModel = _fixture.CreateMany<ChecklistViewModel>();
            _checklistDomainServiceMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Checklist, bool>>>())).ReturnsAsync(checklistList);
            _mapperMock.Setup(x => x.Map<IEnumerable<ChecklistViewModel>>(checklistList)).Returns(checklistListViewModel);

            // act
            var result = await _checklistApplication.GetChecklistsLastDateUpdatedAsync(lastDateSync);

            // assert
            Assert.NotNull(result);
        }


        [Fact(DisplayName = "Shoud return checklists pdf view model for exporta checklists to pdf async")]
        [Trait("[Application.AppServices]-ChecklistApplication", "Application-ExportChecklistsToPDF")]
        public async Task ShouldReturnChecklistPDFViewModelForExportChecklistsToPDFAsync()
        {
            // arrange
            var exportChecklistInput = _fixture.Create<ExportChecklistInput>();
            var checklists = _fixture.CreateMany<ChecklistSectionExportVO>();
            var pdfBytes = _fixture.CreateMany<byte>();
            var category = _fixture.Create<Category>();
            _checklistDomainServiceMock.Setup(x => x.SelectExportSectionPDF(It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<ExportTypeChecklistEnum>())).ReturnsAsync(checklists);
            _categoryDomainServiceMock.Setup(x => x.SelectByIdAsync(It.IsAny<int>())).ReturnsAsync(category);
            _exportPDF.Setup(x => x.ExportHTMLToPDF(It.IsAny<string>())).Returns(pdfBytes.ToArray());


            // act
            var result = await _checklistApplication.ExportChecklistsToPDF(exportChecklistInput);

            // assert
            Assert.NotNull(result);
            Assert.IsType<ExportChecklistPDFViewModel>(result);
            Assert.NotNull(result.Pdf);
        }

        [Fact(DisplayName = "Shoud return null when category not found checklists pdf view model for exporta checklists to pdf async")]
        [Trait("[Application.AppServices]-ChecklistApplication", "Application-ExportChecklistsToPDF")]
        public async Task ShouldReturnNullWhenCategoryNotFoundChecklistPDFViewModelForExportChecklistsToPDFAsync()
        {
            // arrange
            var exportChecklistInput = _fixture.Create<ExportChecklistInput>();
            Category category = null;
            _categoryDomainServiceMock.Setup(x => x.SelectByIdAsync(It.IsAny<int>())).ReturnsAsync(category);

            // act
            var result = await _checklistApplication.ExportChecklistsToPDF(exportChecklistInput);

            // assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "Shoud return null when category without checklists checklist pdf view model for export checklists to pdf async")]
        [Trait("[Application.AppServices]-ChecklistApplication", "Application-ExportChecklistsToPDF")]
        public async Task ShouldReturnNullWhenCategoryWithoutChecklistsChecklistPDFViewModelForExportChecklistsToPDFAsync()
        {
            // arrange
            var exportChecklistInput = _fixture.Create<ExportChecklistInput>();
            var checklists = new List<ChecklistSectionExportVO>();
            var category = _fixture.Create<Category>();
            _checklistDomainServiceMock.Setup(x => x.SelectExportSectionPDF(It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<ExportTypeChecklistEnum>())).ReturnsAsync(checklists);
            _categoryDomainServiceMock.Setup(x => x.SelectByIdAsync(It.IsAny<int>())).ReturnsAsync(category);

            // act
            var result = await _checklistApplication.ExportChecklistsToPDF(exportChecklistInput);

            // assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "Shoud return null when invalid params checklist pdf view model for export checklists to pdf async")]
        [Trait("[Application.AppServices]-ChecklistApplication", "Application-ExportChecklistsToPDF")]
        public async Task ShouldReturnNullWhenInvalidParamsChecklistPDFViewModelForExportChecklistsToPDFAsync()
        {
            // arrange
            var exportChecklistInput = _fixture.Build<ExportChecklistInput>().With(x => x.ValidationResult, new ValidationResult()).Create(); ;
            exportChecklistInput.CategoryId = 0;

            // act
            var result = await _checklistApplication.ExportChecklistsToPDF(exportChecklistInput);

            // assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "Shoud return checklists async")]
        [Trait("[Application.AppServices]-ChecklistApplication", "Application-SelectByCategoryIdAsync")]
        public async Task ShouldReturnChecklistsAsync()
            {
            // arrange
            int categoryId = 123;
            var checklistList = ChecklistFaker.CreateListChecklist();
            _checklistDomainServiceMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Checklist, bool>>>())).ReturnsAsync(checklistList);

            // act
            var result = await _checklistApplication.SelectByCategoryIdAsync(categoryId);

            // assert
            Assert.NotNull(result);
            }
        }
}
