using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.UoW;
using Domain.Services;
using Domain.ValueObjects;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using UnitTest.Application.ChecklistApplication.Faker;
using UnitTest.Domain.Faker;
using Xunit;

namespace UnitTest.Domain
{
    public class ChecklistDomainServiceTest
    {
        private Mock<IChecklistRepository> _checklistRepositoryMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private ChecklistDomainService _checklistDomainService;
        private Mock<ILogger<ChecklistDomainService>> _loggerMock;
        private readonly Fixture _fixture;


        public ChecklistDomainServiceTest()
        {
            _checklistRepositoryMock = new Mock<IChecklistRepository>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ChecklistDomainService>>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _checklistDomainService = new ChecklistDomainService(_checklistRepositoryMock.Object, _smartNotificationMock.Object, _unitOfWorkMock.Object, new DomainNotificationHandler(), _loggerMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }

        [Fact(DisplayName = "Shoud return list of checklists")]
        [Trait("[Domain.Services]-ChecklistDomainService", "Checklist-SelectAllAsync")]
        public async Task ShouldReturnListOfChecklists()
        {
            // arrange
            var checklists = ChecklistFaker.CreateListChecklist();
            _checklistRepositoryMock.Setup(x => x.SelectAllAsync()).ReturnsAsync(checklists);

            // act
            var result = await _checklistDomainService.SelectAllAsync();

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact(DisplayName = "Shoud return list all checklists for export PDF")]
        [Trait("[Domain.Services]-ChecklistDomainService", "Checklist-SelectExportSectionPDF")]
        public async Task ShouldReturnListAllChecklistsForExportPDF()
        {
            // arrange
            var checklists = ChecklistToPDFFaker.CreateList();      
            var categoryId = checklists.FirstOrDefault().CategoryId;
            _checklistRepositoryMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Checklist, bool>>>())).ReturnsAsync(checklists);

            // act
            var result = await _checklistDomainService.SelectExportSectionPDF(categoryId.Value, new int[] {}, ExportTypeChecklistEnum.ALL);

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(30, result.Count());
            Assert.Equal(30, result.Where(x => !x.IsCheck).Count());
        }

        [Fact(DisplayName = "Shoud return list all checklists for export PDF with five (5) checkslist checked")]
        [Trait("[Domain.Services]-ChecklistDomainService", "Checklist-SelectExportSectionPDF")]
        public async Task ShouldReturnListAllChecklistsWithFiveCheckedlistIsCheckForExportPDF()
        {
            // arrange
            var checklists = ChecklistToPDFFaker.CreateList();
            var categoryId = checklists.FirstOrDefault().CategoryId;
            _checklistRepositoryMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Checklist, bool>>>())).ReturnsAsync(checklists);

            // act
            var result = await _checklistDomainService.SelectExportSectionPDF(categoryId.Value, new int[] { 2, 3, 4, 5, 6 }, ExportTypeChecklistEnum.ALL);

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(30, result.Count());
            Assert.Equal(25, result.Where(x => !x.IsCheck).Count());
            Assert.Equal(5, result.Where(x => x.IsCheck).Count());
        }

        [Fact(DisplayName = "Shoud return list checklists checked for export PDF")]
        [Trait("[Domain.Services]-ChecklistDomainService", "Checklist-SelectExportSectionPDF")]
        public async Task ShouldReturnListChecklistsCheckedForExportPDF()
        {
            // arrange
            var checklists = ChecklistToPDFFaker.CreateList();
            var categoryId = checklists.FirstOrDefault().CategoryId;
            _checklistRepositoryMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Checklist, bool>>>())).ReturnsAsync(checklists);

            // act
            var result = await _checklistDomainService.SelectExportSectionPDF(categoryId.Value, new int[] {  2, 3, 4, 5, 6 },ExportTypeChecklistEnum.CHECKS);

            // assert
            Assert.NotEmpty(result);
            var quantityGroups = result.Where(x => x.Type.Equals(ChecklistTypeEnum.GRUPO)).ToList().Count();
            var expected = 5 + quantityGroups;

            Assert.Equal(expected, result.Count());
            Assert.Equal(expected, result.Where(x => x.IsCheck).Count());
        }

        [Fact(DisplayName = "Shoud return list checklists unchecked for export PDF")]
        [Trait("[Domain.Services]-ChecklistDomainService", "Checklist-SelectExportSectionPDF")]
        public async Task ShouldReturnListChecklistsUnCheckedForExportPDF()
        {
            // arrange
            var checklists = ChecklistToPDFFaker.CreateList();
            var categoryId = checklists.FirstOrDefault().CategoryId;
            _checklistRepositoryMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Checklist, bool>>>())).ReturnsAsync(checklists);

            // act
            var result = await _checklistDomainService.SelectExportSectionPDF(categoryId.Value, new int[] { 2, 3, 4, 5, 6 }, ExportTypeChecklistEnum.UNCHECKS);

            // assert
            Assert.NotEmpty(result);

            var quantityGroups = result.Where(x => x.Type.Equals(ChecklistTypeEnum.GRUPO)).ToList().Count();
            var expected = 25;

            Assert.Equal(expected, result.Count());
            Assert.Equal(expected, result.Where(x => !x.IsCheck).Count());
        }
    }
}
