using System;
using AutoMapper;
using Domain.Core;
using Domain.Interfaces.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Moq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using UnitTest.Application.RatingApplication.Faker;
using Xunit;
using App = Application.AppServices;
using System.Linq.Expressions;
using Domain.Entities;
using Application.AppServices.RatingApplication.ViewModel;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Application.AppServices.RatingApplication.Input;

namespace UnitTest.Application.RatingApplication
    {
    public class RatingApplicationTest
    {
        private Mock<IRatingDomainService> _ratingDomainServiceMock;
        private Mock<ICategoryDomainService> _categoryDomainServiceMock;
        private Mock<IChecklistDomainService> _checklistDomainServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private App.RatingApplication.RatingApplication _ratingApplication;
        private Mock<ILogger<App.RatingApplication.RatingApplication>> _loggerMock;
        private AuthService _authService;
        private readonly Fixture _fixture;

        public RatingApplicationTest()
        {
            // configure
            _ratingDomainServiceMock = new Mock<IRatingDomainService>();
            _categoryDomainServiceMock = new Mock<ICategoryDomainService>();
            _checklistDomainServiceMock = new Mock<IChecklistDomainService>();
            _mapperMock = new Mock<IMapper>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _authService = new AuthService();
            _loggerMock = new Mock<ILogger<App.RatingApplication.RatingApplication>>(); 
            _ratingApplication = new App.RatingApplication.RatingApplication(_ratingDomainServiceMock.Object, _smartNotificationMock.Object, _mapperMock.Object, _loggerMock.Object, _categoryDomainServiceMock.Object, _checklistDomainServiceMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }


        //[Fact(DisplayName = "Shoud return failed when try to insert rating without checklist or category")]
        //[Trait("[Application.AppServices]-RatingApplication", "Application-InsertAsync")]
        //public async Task ShouldReturnFailWhenTryToInsertRatingWithouChecklistOrCategoryAsync()
        //    {
        //    //arrange
        //    RatingInput ratingInput = RatingFaker.CreateRatingInput;
        //    ratingInput.CategoryId = null;
        //    ratingInput.ChecklistId = null;

        //    //act
        //    var result = await _ratingApplication.InsertAsync(ratingInput);

        //    //assert
        //    Assert.Null(result);
        //    }

        [Fact(DisplayName = "Shoud return success when try to insert rating with checklist")]
        [Trait("[Application.AppServices]-RatingApplication", "Application-InsertAsync")]
        public async Task ShouldReturnSuccessWhenTryToInsertRatingWithChecklistAsync()
            {
            //arrange
            Rating rating = RatingFaker.CreateRating;
            RatingInput ratingInput = RatingFaker.CreateRatingInput;
            ratingInput.CategoryId = null;
            ratingInput.ChecklistId = 1;

            var ratingViewModel = RatingFaker.RatingViewModel;
            _mapperMock.Setup(x => x.Map<RatingViewModel>(ratingInput)).Returns(ratingViewModel);
            _mapperMock.Setup(x => x.Map<RatingViewModel>(rating)).Returns(ratingViewModel);
            _mapperMock.Setup(x => x.Map<Rating>(ratingInput)).Returns(rating);
            _ratingDomainServiceMock.Setup(x => x.InsertAsync(rating)).ReturnsAsync(rating);
            var checklist = _fixture.Create<Checklist>();
            _checklistDomainServiceMock.Setup(x => x.SelectByIdAsync(It.IsAny<int>())).ReturnsAsync(checklist);

            //act
            var result = await _ratingApplication.InsertAsync(ratingInput);

            //assert
            Assert.NotNull(result);
            }

        [Fact(DisplayName = "Shoud return success when try to insert rating with category")]
        [Trait("[Application.AppServices]-RatingApplication", "Application-InsertAsync")]
        public async Task ShouldReturnSuccessWhenTryToInsertRatingWithCategoryAsync()
            {
            //arrange
            Rating rating = RatingFaker.CreateRating;
            RatingInput ratingInput = RatingFaker.CreateRatingInput;
            ratingInput.ChecklistId = null;
            ratingInput.CategoryId = 1;

            var ratingViewModel = RatingFaker.RatingViewModel;
            _mapperMock.Setup(x => x.Map<RatingViewModel>(ratingInput)).Returns(ratingViewModel);
            _mapperMock.Setup(x => x.Map<RatingViewModel>(rating)).Returns(ratingViewModel);
            _mapperMock.Setup(x => x.Map<Rating>(ratingInput)).Returns(rating);
            _ratingDomainServiceMock.Setup(x => x.InsertAsync(rating)).ReturnsAsync(rating);
            var category = _fixture.Create<Category>();
            _categoryDomainServiceMock.Setup(x => x.SelectByIdAsync(It.IsAny<int>())).ReturnsAsync(category);

            //act
            var result = await _ratingApplication.InsertAsync(ratingInput);

            //assert
            Assert.NotNull(result);
            }
        }
}
