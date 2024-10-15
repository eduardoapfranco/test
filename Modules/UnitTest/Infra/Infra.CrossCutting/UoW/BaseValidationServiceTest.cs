using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Implements;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.UoW.Models;
using Moq;
using Xunit;

namespace UnitTest.Infra.Infra.CrossCutting.UoW
{
    public class BaseValidationServiceTest
    {
        [Fact(DisplayName = "Construct should return to fill correctly field notify")]
        [Trait("[Infra.CrossCutting]-BaseValidationService", "UoW")]
        public void ConstructShouldToFillCorrectlyFieldNotify()
        {
            // arrange
            var notifyMock = new Mock<ISmartNotification>();
            var smartNotificationMock = new Mock<SmartNotification>(new DomainNotificationHandler());
            notifyMock.Setup(x => x.Invoke()).Returns(smartNotificationMock.Object).Verifiable();

            // act
            var baseValidation = new BaseValidationService(notifyMock.Object);

            // assert
            notifyMock.Verify(x => x.Invoke(), Times.Once());
        }

        [Fact(DisplayName = "Method notify errors and validation should return correctly when exists errors")]
        [Trait("[Infra.CrossCutting]-BaseValidationService", "UoW")]
        public void MethodNotifyErrorsAndValidationShouldCorrectlyWhenExistsErrors()
        {
            // arrange
            var notifyMock = new Mock<ISmartNotification>();
            var smartNotificationMock = new Mock<ISmartNotification>();

            // act
            notifyMock.Setup(x => x.Invoke()).Returns(smartNotificationMock.Object).Verifiable();
            smartNotificationMock.Setup(x => x.NewNotificationBadRequest(new string[] {}, "Id é obrigatório")).Verifiable();
            var baseValidation = new BaseValidationServiceMock(notifyMock.Object);
            var entity = new EntityMock();
            baseValidation.TestNotification(entity);

            // assert
            smartNotificationMock.Verify(x => x.NewNotificationBadRequest(new string[] {}, "Id é obrigatório"),Times.Once());
        }

        private class BaseValidationServiceMock : BaseValidationService
        {
            public BaseValidationServiceMock(ISmartNotification notify) : base(notify)
            {}

            public void TestNotification(BaseResult result)
            {
                base.NotifyErrorsAndValidation(new string[] {}, result);
            }
        }

        private class EntityMock : BaseResult
        {
            public EntityMock()
            {
                ValidationResult = new FluentValidation.Results.ValidationResult();
                ValidationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Id", "Id é obrigatório"));
            }
        }
    } 
}

