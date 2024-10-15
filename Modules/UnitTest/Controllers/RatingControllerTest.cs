using Application.AppServices.RatingApplication.Input;
using Application.AppServices.RatingApplication.ViewModel;
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
    public class RatingControllerTest
        {
        private DomainNotificationHandler _notificationHandler;
        private Mock<IRatingApplication> _ratingApplicationMock;
        private Mock<ILogger<RatingController>> _loggerMock;
        private RatingController _controller;

        public RatingControllerTest()
            {
            _ratingApplicationMock = new Mock<IRatingApplication>();
            _notificationHandler = new DomainNotificationHandler();
            _loggerMock = new Mock<ILogger<RatingController>>();
            _controller = new RatingController(_notificationHandler, _ratingApplicationMock.Object, _loggerMock.Object);
            }

        [Fact(DisplayName = "Should return new rating after insert async")]
        [Trait("[WebApi.Controllers]-RatingController", "Controllers-InsertAsync")]
        public async Task ShouldReturnNewRatingAfterInsertAsync()
            {

            var ratingInput = Builder<RatingInput>.CreateNew().Build();
            var ratingViewModel = Builder<RatingViewModel>.CreateNew().Build();

            _ratingApplicationMock.Setup(x => x.InsertAsync(ratingInput)).ReturnsAsync(ratingViewModel);

            var result = await _controller.PostAsync(ratingInput);

            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<RatingViewModel>>(okObjectResult.Value);

            var resultVerify = (Result<RatingViewModel>)okObjectResult.Value;

            Assert.NotNull(result);
            Assert.Equal(ratingViewModel, resultVerify.Data);
            }

        }
    }
