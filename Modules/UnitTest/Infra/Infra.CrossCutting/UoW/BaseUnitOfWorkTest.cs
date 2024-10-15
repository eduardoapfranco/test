using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Model;
using Infra.CrossCutting.UoW.Interfaces;
using Infra.CrossCutting.UoW.Models;
using MediatR;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Infra.Infra.CrossCutting.UoW
{
    public class BaseUnitOfWorkTest
    {
        [Fact(DisplayName = "Method commit should return return true when realize commit with success")]
        [Trait("[Infra.CrossCutting]-BaseUnitOfWork", "UoW")]
        public void MethodCommitShouldReturnTrueWhenRealizeCommitWithSuccess()
        {
            // prepare
            var unitOfWorkBaseMock = new Mock<IUnitOfWorkBase>();
            var domainNotificationHandler = new DomainNotificationHandler();

            unitOfWorkBaseMock.Setup(x => x.Commit()).Returns(new CommandResponse(true)).Verifiable();

            // act
            var baseUnitOfWorkTeste = new BaseUnitOfWorkTeste(unitOfWorkBaseMock.Object, domainNotificationHandler);

            var result = baseUnitOfWorkTeste.TestCommit();

            // assert
            Assert.True(result);
            unitOfWorkBaseMock.Verify(x => x.Commit(), Times.Once());
        }

        [Fact(DisplayName = "Method commit should return return false when commit fail")]
        [Trait("[Infra.CrossCutting]-BaseUnitOfWork", "UoW")]
        public void MethodCommitShouldReturnFalseWhenCommitFail()
        {
            // arrange
            var unitOfWorkBaseMock = new Mock<IUnitOfWorkBase>();
            var domainNotificationHandler = new DomainNotificationHandler();

            unitOfWorkBaseMock.Setup(x => x.Commit()).Returns(new CommandResponse(false)).Verifiable();

            // act
            var baseUnitOfWorkTeste = new BaseUnitOfWorkTeste(unitOfWorkBaseMock.Object, domainNotificationHandler);

            var result = baseUnitOfWorkTeste.TestCommit();

            // assert
            Assert.False(result);
            unitOfWorkBaseMock.Verify(x => x.Commit(), Times.Once());
        }

        [Fact(DisplayName = "Method commit should return return false when object message hanlder is null")]
        [Trait("[Infra.CrossCutting]-BaseUnitOfWork", "UoW")]
        public void MethodCommitShouldReturnFalseWhenObjectMessageHanlderIsNull()
        {
            // arrange
            var unitOfWorkBaseMock = new Mock<IUnitOfWorkBase>();
            var domainNotificationHandler = new DomainNotificationHandler();

            unitOfWorkBaseMock.Setup(x => x.Commit()).Returns(new CommandResponse(false)).Verifiable();

            // act
            var baseUnitOfWorkTeste = new BaseUnitOfWorkTeste(unitOfWorkBaseMock.Object, null);

            var result = baseUnitOfWorkTeste.TestCommit();

            // assert
            Assert.False(result);
            unitOfWorkBaseMock.Verify(x => x.Commit(), Times.Never());
        }

        [Fact(DisplayName = "Method commit should return return false when object exist notification in message hanlder")]
        [Trait("[Infra.CrossCutting]-BaseUnitOfWork", "UoW")]
        public async Task MethodCommitShouldReturnFalseWhenObjectExistNotificationInMessageHandler()
        {
            // prepare
            var unitOfWorkBaseMock = new Mock<IUnitOfWorkBase>();
            var domainNotificationHandler = new DomainNotificationHandler();

            unitOfWorkBaseMock.Setup(x => x.Commit()).Returns(new CommandResponse(false)).Verifiable();

            // act
            await domainNotificationHandler.Handle(new DomainNotification("value"), new CancellationToken());

            var baseUnitOfWorkTeste = new BaseUnitOfWorkTeste(unitOfWorkBaseMock.Object, domainNotificationHandler);

            var result = baseUnitOfWorkTeste.TestCommit();

            // assert
            Assert.False(result);
            unitOfWorkBaseMock.Verify(x => x.Commit(), Times.Never());
        }

        private class BaseUnitOfWorkTeste : BaseUnitOfWork
        {
            public BaseUnitOfWorkTeste(IUnitOfWorkBase unitOfWork, INotificationHandler<DomainNotification> notification) : base(unitOfWork, notification)
            {
            }

            public bool TestCommit()
            {
                return Commit();
            }
        }
    }
}
