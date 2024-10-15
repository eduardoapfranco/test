using Infra.CrossCutting.Notification.Enum;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Implements;
using Infra.CrossCutting.Notification.Model;
using System.Linq;
using System.Threading;
using Xunit;

namespace UnitTest.Infra.Infra.CrossCutting.Notification
{
    public class SmartNotificationTest
    {
        [Fact(DisplayName = "Should return false when notification bad request with string substitution")]
        [Trait("[Infra.CrossCutting]-SmartNotification", "Notification")]
        public void ShouldFalseWhenNotificationBadRequestWithStringSubstitution()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            var smartNotification = new SmartNotification(notification);

            // act
            smartNotification.NewNotificationBadRequest(new string[] { "Jhony" }, "Olá {0}!");

            // assert
            Assert.False(smartNotification.IsValid());
        }

        [Fact(DisplayName = "Should return true if not exists notifications")]
        [Trait("[Infra.CrossCutting]-SmartNotification", "Notification")]
        public void ShouldTrueIfNotExistsNotifications()
        {
            // arrange
            var notification = new DomainNotificationHandler();

            // act
            var smartNotification = new SmartNotification(notification);

            // assert
            Assert.True(smartNotification.IsValid());
        }

        [Fact(DisplayName = "Should return is valid true when notification bad request with message null")]
        [Trait("[Infra.CrossCutting]-SmartNotification", "Notification")]
        public void ShouldIsValidTrueWhenNotificationBadRequestWithMessageNull()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            var smartNotification = new SmartNotification(notification);

            // act
            smartNotification.NewNotificationBadRequest(new string[] { "Jhony" }, null);

            // assert
            Assert.True(smartNotification.IsValid());
        }

        [Fact(DisplayName = "Should return false if not exists notifications")]
        [Trait("[Infra.CrossCutting]-SmartNotification", "Notification")]      
        public void ShouldFalseIfNotExistsNotifications()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            
            // act
            notification.Handle(new DomainNotification("", DomainNotificationType.BadRequest), new CancellationToken());
            var smartNotification = new SmartNotification(notification);

            // assert
            Assert.False(smartNotification.IsValid());
        }

        [Fact(DisplayName = "Should return add notification bad request")]
        [Trait("[Infra.CrossCutting]-SmartNotification", "Notification")]
        public void ShouldAddNotificationBadRequest()
        {
            // prepare
            var notification = new DomainNotificationHandler();     
            var smartNotification = new SmartNotification(notification);
            
            // act
            smartNotification.NewNotificationBadRequest(new[] { "A", "B" }, "Erro!");
            
            //assert
            Assert.False(smartNotification.IsValid());
        }

        [Fact(DisplayName = "Should return add notification conflict")]
        [Trait("[Infra.CrossCutting]-SmartNotification", "Notification")]
        public void ShouldAddNotificationConflit()
        {
            // prepare
            var notification = new DomainNotificationHandler();
            var smartNotification = new SmartNotification(notification);
            
            // act
            smartNotification.NewNotificationConflict(new[] { "A", "B" }, "Erro!");
            
            // assert
            Assert.False(smartNotification.IsValid());
        }

        [Fact(DisplayName = "Should return is valid true when notification Conflict with message null")]
        [Trait("[Infra.CrossCutting]-SmartNotification", "Notification")]
        public void ShouldIsValidTrueWhenNotificationConflictWithMessageNull()
        {
            // arrange
            var notification = new DomainNotificationHandler();
            var smartNotification = new SmartNotification(notification);

            // act
            smartNotification.NewNotificationConflict(new string[] { "Jhony" }, null);

            // assert
            Assert.True(smartNotification.IsValid());
        }

        [Fact(DisplayName = "Invoke should return return your instance")]
        [Trait("[Infra.CrossCutting]-SmartNotification", "Notification")]
        public void InvokeShouldReturnYourInstance()
        {
            // prepare
            var notification = new DomainNotificationHandler();
            var smartNotification = new SmartNotification(notification);
            
            // act
            var instancia = smartNotification.Invoke();
            
            // assert
            Assert.Equal(smartNotification, instancia);
        }

        [Fact(DisplayName = "Should to give dispose")]
        [Trait("[Infra.CrossCutting]-SmartNotification", "Notification")]
        public void ShouldToGiveDispose()
        {
            // prepare
            var notification = new DomainNotificationHandler();

            // act
            notification.Handle(new DomainNotification("", DomainNotificationType.BadRequest), new CancellationToken());
            notification.Handle(new DomainNotification("ConflictValue"), new CancellationToken());
            notification.Handle(new DomainNotification("", DomainNotificationType.BadRequest), new CancellationToken());

            // assert
            Assert.True(notification.HasNotifications());

            // prepare
            var notificacao = notification.GetNotifications().FirstOrDefault(x => x.DomainNotificationType == DomainNotificationType.Conflict);
            
            // assert
            Assert.Equal("ConflictValue", notificacao.Value);
            notification.Dispose();
            Assert.False(notification.HasNotifications());
        }

        [Fact(DisplayName = "Should return return empty positions")]
        [Trait("[Infra.CrossCutting]-SmartNotification", "Notification")]
        public void ShouldReturnEmptyPositions()
        {
            // arrange
            var notification = new DomainNotificationHandler();

            // act
            var smartNotification = new SmartNotification(notification);
            var emptyuPositions = smartNotification.EmptyPositions();


            // assert
            Assert.IsType<string[]>(emptyuPositions);
        }
    }
}
