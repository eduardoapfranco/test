using System;
using AutoMapper;
using Domain.Core;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Moq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using UnitTest.Application.ContentSugestionApplication.Faker;
using Xunit;
using App = Application.AppServices;
using System.Linq.Expressions;
using Domain.Entities;
using Application.AppServices.ContentSugestionApplication.ViewModel;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Application.AppServices.ContentSugestionApplication.Input;

namespace UnitTest.Application.ContentSugestionApplication
    {
    public class ContentSugestionApplicationTest
    {
        private Mock<IContentSugestionDomainService> _contentSugestionDomainServiceMock;
        private Mock<ICategoryDomainService> _categoryDomainServiceMock;
        private Mock<IChecklistDomainService> _checklistDomainServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private App.ContentSugestionApplication.ContentSugestionApplication _contentSugestionApplication;
        private Mock<ILogger<App.ContentSugestionApplication.ContentSugestionApplication>> _loggerMock;
        private AuthService _authService;
        private readonly Fixture _fixture;

        public ContentSugestionApplicationTest()
        {
            // configure
            _contentSugestionDomainServiceMock = new Mock<IContentSugestionDomainService>();
            _categoryDomainServiceMock = new Mock<ICategoryDomainService>();
            _checklistDomainServiceMock = new Mock<IChecklistDomainService>();
            _mapperMock = new Mock<IMapper>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _authService = new AuthService();
            _loggerMock = new Mock<ILogger<App.ContentSugestionApplication.ContentSugestionApplication>>(); 
            _contentSugestionApplication = new App.ContentSugestionApplication.ContentSugestionApplication(_contentSugestionDomainServiceMock.Object, _smartNotificationMock.Object, _mapperMock.Object, _loggerMock.Object, _categoryDomainServiceMock.Object, _checklistDomainServiceMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }

        [Fact(DisplayName = "Shoud return success when try to insert contentSugestion with checklist")]
        [Trait("[Application.AppServices]-ContentSugestionApplication", "Application-InsertAsync")]
        public async Task ShouldReturnSuccessWhenTryToInsertContentSugestionWithChecklistAsync()
            {
            //arrange
            ContentSugestion contentSugestion = ContentSugestionFaker.CreateContentSugestion;
            ContentSugestionInput contentSugestionInput = ContentSugestionFaker.CreateContentSugestionInput;
            contentSugestionInput.CategoryId = null;
            contentSugestionInput.ChecklistId = 1;

            var contentSugestionViewModel = ContentSugestionFaker.ContentSugestionViewModel;
            _mapperMock.Setup(x => x.Map<ContentSugestionViewModel>(contentSugestionInput)).Returns(contentSugestionViewModel);
            _mapperMock.Setup(x => x.Map<ContentSugestionViewModel>(contentSugestion)).Returns(contentSugestionViewModel);
            _mapperMock.Setup(x => x.Map<ContentSugestion>(contentSugestionInput)).Returns(contentSugestion);
            _contentSugestionDomainServiceMock.Setup(x => x.InsertAsync(contentSugestion)).ReturnsAsync(contentSugestion);
            var checklist = _fixture.Create<Checklist>();
            _checklistDomainServiceMock.Setup(x => x.SelectByIdAsync(It.IsAny<int>())).ReturnsAsync(checklist);

            //act
            var result = await _contentSugestionApplication.InsertAsync(contentSugestionInput);

            //assert
            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Shoud return success when try to insert contentSugestion with category")]
        [Trait("[Application.AppServices]-ContentSugestionApplication", "Application-InsertAsync")]
        public async Task ShouldReturnSuccessWhenTryToInsertContentSugestionWithCategoryAsync()
            {
            //arrange
            ContentSugestion contentSugestion = ContentSugestionFaker.CreateContentSugestion;
            ContentSugestionInput contentSugestionInput = ContentSugestionFaker.CreateContentSugestionInput;
            contentSugestionInput.ChecklistId = null;
            contentSugestionInput.CategoryId = 1;

            var contentSugestionViewModel = ContentSugestionFaker.ContentSugestionViewModel;
            _mapperMock.Setup(x => x.Map<ContentSugestionViewModel>(contentSugestionInput)).Returns(contentSugestionViewModel);
            _mapperMock.Setup(x => x.Map<ContentSugestionViewModel>(contentSugestion)).Returns(contentSugestionViewModel);
            _mapperMock.Setup(x => x.Map<ContentSugestion>(contentSugestionInput)).Returns(contentSugestion);
            _contentSugestionDomainServiceMock.Setup(x => x.InsertAsync(contentSugestion)).ReturnsAsync(contentSugestion);
            var category = _fixture.Create<Category>();
            _categoryDomainServiceMock.Setup(x => x.SelectByIdAsync(It.IsAny<int>())).ReturnsAsync(category);

            //act
            var result = await _contentSugestionApplication.InsertAsync(contentSugestionInput);

            //assert
            Assert.NotNull(result);
            }
        }
}
