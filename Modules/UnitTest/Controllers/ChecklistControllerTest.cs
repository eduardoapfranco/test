using Application.AppServices.ChecklistApplication.Input;
using Application.AppServices.ChecklistApplication.ViewModel;
using Application.Interfaces;
using ConstruaApp.Api.Controllers;
using FizzWare.NBuilder;
using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.Notification.Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Controllers
{
    public class ChecklistControllerTest
    {
        private DomainNotificationHandler _notificationHandler;
        private Mock<IChecklistApplication> _checklistApplicationMock;
        private ChecklistController _controller;
        private Mock<ILogger<ChecklistController>> _loggerMock;


        public ChecklistControllerTest()
        {
            _checklistApplicationMock = new Mock<IChecklistApplication>();
            _loggerMock = new Mock<ILogger<ChecklistController>>();
            _notificationHandler = new DomainNotificationHandler();
            _controller = new ChecklistController(_notificationHandler, _checklistApplicationMock.Object, _loggerMock.Object);
        }

        [Fact(DisplayName = "Should return list of checklists after get async")]
        [Trait("[WebApi.Controllers]-ChecklistController", "Controllers-GetAsync")]
        public async Task ShouldReturnListOfChecklistsAfterGetAsync()
        {
            var lastDateSync = new System.DateTime();
            var checklistViewModelList = Builder<List<ChecklistViewModel>>.CreateNew().Build();
            _checklistApplicationMock.Setup(x => x.GetChecklistsLastDateUpdatedAsync(lastDateSync)).ReturnsAsync(checklistViewModelList);

            var result = await _controller.GetChecklistsLastDateUpdatedAsync(new System.DateTime());

            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<IEnumerable<ChecklistViewModel>>>(okObjectResult.Value);

            var resultVerify = (Result<IEnumerable<ChecklistViewModel>>)okObjectResult.Value;

            Assert.NotNull(result);
            Assert.Equal(checklistViewModelList, resultVerify.Data);
        }

        [Fact(DisplayName = "Should return export checklists to pdf after get async")]
        [Trait("[WebApi.Controllers]-ChecklistController", "Controllers-ExportChecklistsToPDF")]
        public async Task ShouldReturnViewModelExportChecklistsToPDF()
        {
            var input = Builder<ExportChecklistInput>.CreateNew().Build();
            var checklistViewModelList = Builder<ExportChecklistPDFViewModel>.CreateNew().Build();
            _checklistApplicationMock.Setup(x => x.ExportChecklistsToPDF(input)).ReturnsAsync(checklistViewModelList);

            var result = await _controller.ExportChecklistsToPDF(input);

            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<Result<ExportChecklistPDFViewModel>>(okObjectResult.Value);

            var resultVerify = (Result<ExportChecklistPDFViewModel>)okObjectResult.Value;

            Assert.NotNull(result);

        }

        [Fact(DisplayName = "Should return internal server error export checklists to pdf after get async")]
        [Trait("[WebApi.Controllers]-ChecklistController", "Controllers-ExportChecklistsToPDF")]
        public async Task ShouldReturnInternalServerErrorExportChecklistsToPDF()
        {
            var input = Builder<ExportChecklistInput>.CreateNew().Build();
            _checklistApplicationMock.Setup(x => x.ExportChecklistsToPDF(input)).ThrowsAsync(new Exception("error"));

            var result = await _controller.ExportChecklistsToPDF(input);
            Assert.IsType<ObjectResult>(result);
            var okObjectResult = (ObjectResult)result;
            Assert.Equal((int) HttpStatusCode.InternalServerError, okObjectResult.StatusCode);
            Assert.NotNull(result);
        }
    }
}
