using Application.AppServices.ContentSugestionApplication.Input;
using Application.AppServices.ContentSugestionApplication.ViewModel;
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
    public class ContentSugestionControllerTest
        {
        private DomainNotificationHandler _notificationHandler;
        private Mock<IContentSugestionApplication> _contentSugestionApplicationMock;
        private Mock<ILogger<ContentSugestionController>> _loggerMock;
        private ContentSugestionController _controller;

        public ContentSugestionControllerTest()
            {
            _contentSugestionApplicationMock = new Mock<IContentSugestionApplication>();
            _notificationHandler = new DomainNotificationHandler();
            _loggerMock = new Mock<ILogger<ContentSugestionController>>();
            _controller = new ContentSugestionController(_notificationHandler, _contentSugestionApplicationMock.Object, _loggerMock.Object);
            }

        [Fact(DisplayName = "Should return new contentSugestion after insert async")]
        [Trait("[WebApi.Controllers]-ContentSugestionController", "Controllers-InsertAsync")]
        public async Task ShouldReturnNewContentSugestionAfterInsertAsync()
            {

            var contentSugestionInput = Builder<ContentSugestionInput>.CreateNew().Build();
            var contentSugestionViewModel = Builder<ContentSugestionViewModel>.CreateNew().Build();

            _contentSugestionApplicationMock.Setup(x => x.InsertAsync(contentSugestionInput)).ReturnsAsync(contentSugestionViewModel);

            var result = await _controller.PostAsync(contentSugestionInput);

            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<ContentSugestionViewModel>>(okObjectResult.Value);

            var resultVerify = (Result<ContentSugestionViewModel>)okObjectResult.Value;

            Assert.NotNull(result);
            Assert.Equal(contentSugestionViewModel, resultVerify.Data);
            }

        }
    }
