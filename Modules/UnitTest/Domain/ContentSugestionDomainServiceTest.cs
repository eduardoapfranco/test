using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.UoW;
using Domain.Services;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Moq;
using System.Threading.Tasks;
using UnitTest.Application.ContentSugestionApplication.Faker;
using Xunit;

namespace UnitTest.Domain
{
    public class ContentSugestionDomainServiceTest
        {
        private Mock<IContentSugestionRepository> _contentSugestionRepositoryMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private ContentSugestionDomainService _contentSugestionDomainService;
        private Mock<ILogger<ContentSugestionDomainService>> _loggerMock;


        public ContentSugestionDomainServiceTest()
            {
            _contentSugestionRepositoryMock = new Mock<IContentSugestionRepository>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ContentSugestionDomainService>>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _contentSugestionDomainService = new ContentSugestionDomainService(_contentSugestionRepositoryMock.Object, _smartNotificationMock.Object, _unitOfWorkMock.Object, new DomainNotificationHandler(), _loggerMock.Object);
            }

        [Fact(DisplayName = "Shoud return created contentSugestion")]
        [Trait("[Domain.Services]-ContentSugestionDomainService", "ContentSugestion-InsertAsync")]
        public async Task ShouldReturnCreatedContentSugestion()
            {
            // arrange
            ContentSugestion contentSugestion = ContentSugestionFaker.CreateContentSugestion;
            CommandResponse commandResponse = new CommandResponse(true);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(commandResponse);
            _contentSugestionRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<ContentSugestion>())).ReturnsAsync(contentSugestion);

            // act
            var result = await _contentSugestionDomainService.InsertAsync(contentSugestion);

            // assert
            Assert.NotNull(result);
            }
        }
    }
