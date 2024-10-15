using Domain.Interfaces.Repositories;
using Domain.Interfaces.UoW;
using Domain.Services;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Domain.Interfaces.Services;
using UnitTest.Application.CategoryApplication.Faker;
using Xunit;

namespace UnitTest.Domain
{
    public class CategoryDomainServiceTest
        {
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IUserDomainService> _userDomainServiceMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private CategoryDomainService _categoryDomainService;
        private Mock<ILogger<CategoryDomainService>> _loggerMock;


        public CategoryDomainServiceTest()
            {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CategoryDomainService>>();
            _userDomainServiceMock = new Mock<IUserDomainService>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _categoryDomainService = new CategoryDomainService(_categoryRepositoryMock.Object, _smartNotificationMock.Object, _unitOfWorkMock.Object, new DomainNotificationHandler(), _loggerMock.Object, _userDomainServiceMock.Object);
            }

        [Fact(DisplayName = "Shoud return list of categories")]
        [Trait("[Domain.Services]-CategoryDomainService", "Category-SelectAllAsync")]
        public async Task ShouldReturnListOfCategories()
            {
            // arrange
            var categories = CategoryFaker.CreateListCategory();
            _categoryRepositoryMock.Setup(x => x.SelectAllAsync()).ReturnsAsync(categories);

            // act
            var result = await _categoryDomainService.SelectAllAsync();

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            }
        }

    }
