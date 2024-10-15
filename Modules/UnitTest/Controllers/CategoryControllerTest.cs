using Application.AppServices.CategoryApplication.ViewModel;
using Application.Interfaces;
using ConstruaApp.Api.Controllers;
using FizzWare.NBuilder;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Controllers
{
    public class CategoryControllerTest
    {
        private DomainNotificationHandler _notificationHandler;
        private Mock<ICategoryApplication> _categoryApplicationMock;
        private CategoryController _controller;

        public CategoryControllerTest()
        {
            _categoryApplicationMock = new Mock<ICategoryApplication>();
            _notificationHandler = new DomainNotificationHandler();
            _controller = new CategoryController(_notificationHandler, _categoryApplicationMock.Object);
        }

        [Fact(DisplayName = "Should return list of categories after get async")]
        [Trait("[WebApi.Controllers]-CategoryController", "Controllers-GetAsync")]
        public async Task ShouldReturnListOfCategoriesAfterGetAsync()
        {
            var lastDateSync = new System.DateTime();
            var categoryViewModelList = Builder<List<CategoryViewModel>>.CreateNew().Build();
            _categoryApplicationMock.Setup(x => x.GetCategoriesLastDateUpdatedAsync(lastDateSync)).ReturnsAsync(categoryViewModelList);

            var result = await _controller.GetCategoriesLastDateUpdatedAsync(new System.DateTime());

            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<IEnumerable<CategoryViewModel>>>(okObjectResult.Value);

            var resultVerify = (Result<IEnumerable<CategoryViewModel>>)okObjectResult.Value;

            Assert.NotNull(result);
            Assert.Equal(categoryViewModelList, resultVerify.Data);
        }
    }
}
