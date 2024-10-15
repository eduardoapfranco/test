using System;
using AutoMapper;
using Domain.Core;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Moq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using UnitTest.Application.CategoryApplication.Faker;
using Xunit;
using App = Application.AppServices;
using System.Linq.Expressions;
using Domain.Entities;
using Application.AppServices.CategoryApplication.ViewModel;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace UnitTest.Application.CategoryApplication
    {
    public class CategoryApplicationTest
    {
        private Mock<ICategoryDomainService> _categoryDomainServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private App.CategoryApplication.CategoryApplication _categoryApplication;
        private Mock<ILogger<App.CategoryApplication.CategoryApplication>> _loggerMock;
        private AuthService _authService;
        private readonly Fixture _fixture;

        public CategoryApplicationTest()
        {
            // configure
            _categoryDomainServiceMock = new Mock<ICategoryDomainService>();
            _mapperMock = new Mock<IMapper>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _authService = new AuthService();
            _loggerMock = new Mock<ILogger<App.CategoryApplication.CategoryApplication>>(); 
            _categoryApplication = new App.CategoryApplication.CategoryApplication(_categoryDomainServiceMock.Object, _smartNotificationMock.Object, _mapperMock.Object, _authService, _loggerMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }


        [Fact(DisplayName = "Shoud return empty when last date sync is greater than last upadted date of categories async")]
        [Trait("[Application.AppServices]-CategoryApplication", "Application-ListAsync")]
        public async Task ShouldReturnEmptyWhenLastDaySyncIsGreaterThanlastUpdatedDateOfCategoriesAsync()
        {
            // arrange
            DateTime? lastDateSync = DateTime.Today.AddDays(1);
            var categoryList = CategoryFaker.CreateListCategory();
            _categoryDomainServiceMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(categoryList);

            // act
            var result = await _categoryApplication.GetCategoriesLastDateUpdatedAsync(lastDateSync);

            // assert
            Assert.Empty(result);
        }

        [Fact(DisplayName = "Shoud return null when last date sync is null async")]
        [Trait("[Application.AppServices]-CategoryApplication", "Application-ListAsync")]
        public async Task ShouldReturnNullWhenLastDaySyncIsNullAsync()
        {
            // arrange
            DateTime? lastDateSync = null;
            var categoryList = CategoryFaker.CreateListCategory();
            _categoryDomainServiceMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(categoryList);

            // act
            var result = await _categoryApplication.GetCategoriesLastDateUpdatedAsync(lastDateSync);

            // assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "Shoud return result list when last date sync is greater than last updated date of categories async")]
        [Trait("[Application.AppServices]-CategoryApplication", "Application-ListAsync")]
        public async Task ShouldReturnResultListWhenLastDaySyncIsLessThanlastUpdatedDateOfCategoriesAsync()
        {
            // arrange
            DateTime? lastDateSync = DateTime.Today.AddDays(-2);
            var categoryList = CategoryFaker.CreateListCategory();
            var categoryListViewModel = _fixture.CreateMany<CategoryViewModel>();
            _categoryDomainServiceMock.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(categoryList);
            _mapperMock.Setup(x => x.Map<IEnumerable<CategoryViewModel>>(categoryList)).Returns(categoryListViewModel);

            // act
            var result = await _categoryApplication.GetCategoriesLastDateUpdatedAsync(lastDateSync);

            // assert
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Shoud return categories based on profile async")]
        [Trait("[Application.AppServices]-CategoryApplication", "Application-GetRootCategoriesBasedOnProfileAsync")]
        public async Task ShouldReturnRootCategoriesBasedOnProfileAsync()
            {
            // arrange
            int userId = 1;
            var categoryList = CategoryFaker.CreateListCategory();
            var categoryListViewModel = _fixture.CreateMany<CategoryViewModel>();
            _categoryDomainServiceMock.Setup(x => x.GetRootCategoriesBasedOnProfileAsync(It.IsAny<int>())).ReturnsAsync(categoryList);
            _mapperMock.Setup(x => x.Map<IEnumerable<CategoryViewModel>>(categoryList)).Returns(categoryListViewModel);

            // act
            var result = await _categoryApplication.GetRootCategoriesBasedOnProfileAsync(userId);

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            }

        [Fact(DisplayName = "Shoud return categories based on profile async")]
        [Trait("[Application.AppServices]-CategoryApplication", "Application-GetCategoriesByParentBasedOnProfileAsync")]
        public async Task ShouldReturnCategoriesByParentBasedOnProfileAsync()
            {
            // arrange
            int userId = 1;
            var categoryList = CategoryFaker.CreateListCategory();
            var categoryListViewModel = _fixture.CreateMany<CategoryViewModel>();
            _categoryDomainServiceMock.Setup(x => x.GetCategoriesByParentBasedOnProfileAsync(It.IsAny<int>())).ReturnsAsync(categoryList);
            _mapperMock.Setup(x => x.Map<IEnumerable<CategoryViewModel>>(categoryList)).Returns(categoryListViewModel);

            // act
            var result = await _categoryApplication.GetCategoriesByParentBasedOnProfileAsync(userId);

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            }

        [Fact(DisplayName = "Shoud return all categories async")]
        [Trait("[Application.AppServices]-CategoryApplication", "Application-GetAllAsync")]
        public async Task ShouldReturnAllCategoriesAsync()
            {
            // arrange
            var categoryList = CategoryFaker.CreateListCategory();
            var categoryListViewModel = _fixture.CreateMany<CategoryViewModel>();
            _categoryDomainServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categoryList);
            _mapperMock.Setup(x => x.Map<IEnumerable<CategoryViewModel>>(categoryList)).Returns(categoryListViewModel);

            // act
            var result = await _categoryApplication.GetAllAsync();

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            }

        [Fact(DisplayName = "Shoud return categories selected by id async")]
        [Trait("[Application.AppServices]-CategoryApplication", "Application-GetAllAsync")]
        public async Task ShouldReturnCategoriesSelectedByIdAsync()
            {
            // arrange
            int categoryId = 1;
            var category = CategoryFaker.CreateCategory;
            var categoryViewModel = _fixture.Create<CategoryViewModel>();

            _categoryDomainServiceMock.Setup(x => x.SelectByIdAsync(It.IsAny<int>())).ReturnsAsync(category);
            _mapperMock.Setup(x => x.Map<CategoryViewModel>(category)).Returns(categoryViewModel);

            // act
            var result = await _categoryApplication.SelectByIdAsync(categoryId);

            // assert
            Assert.NotNull(result);
            }

        }
}
