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
using UnitTest.Application.RatingApplication.Faker;
using Xunit;

namespace UnitTest.Domain
{
    public class RatingDomainServiceTest
        {
        private Mock<IRatingRepository> _ratingRepositoryMock;
        private Mock<ISmartNotification> _smartNotificationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private RatingDomainService _ratingDomainService;
        private Mock<ILogger<RatingDomainService>> _loggerMock;


        public RatingDomainServiceTest()
            {
            _ratingRepositoryMock = new Mock<IRatingRepository>();
            _smartNotificationMock = new Mock<ISmartNotification>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<RatingDomainService>>();
            _smartNotificationMock.Setup(x => x.Invoke()).Returns(_smartNotificationMock.Object);
            _ratingDomainService = new RatingDomainService(_ratingRepositoryMock.Object, _smartNotificationMock.Object, _unitOfWorkMock.Object, new DomainNotificationHandler(), _loggerMock.Object);
            }

        [Fact(DisplayName = "Shoud return created rating")]
        [Trait("[Domain.Services]-RatingDomainService", "Rating-InsertAsync")]
        public async Task ShouldReturnCreatedRating()
            {
            // arrange
            Rating rating = RatingFaker.CreateRating;
            CommandResponse commandResponse = new CommandResponse(true);
            _unitOfWorkMock.Setup(x => x.Commit()).Returns(commandResponse);
            _ratingRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<Rating>())).ReturnsAsync(rating);

            // act
            var result = await _ratingDomainService.InsertAsync(rating);

            // assert
            Assert.NotNull(result);
            }
        }
    }
