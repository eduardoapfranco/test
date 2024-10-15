using Infra.CrossCutting.Notification.Enum;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.Notification.Model;
using MediatR;

namespace Infra.CrossCutting.Notification.Implements
{
    public class SmartNotification : ISmartNotification
    {
        private readonly DomainNotificationHandler _messageHandler;

        public SmartNotification(INotificationHandler<DomainNotification> notification)
        {
            _messageHandler = (DomainNotificationHandler)notification;
        }

        public ISmartNotification Invoke()
        {
            return this;
        }

        public bool IsValid()
        {
            return !_messageHandler.HasNotifications();
        }

        public void NewNotificationConflict(string[] parameters, string message)
        {
            if (message == null)
                return;

            var completeMessage = GettingParametrizedMessage(parameters, message);

            _messageHandler.Handle(new DomainNotification(completeMessage, DomainNotificationType.Conflict), default);
        }

        public void NewNotificationBadRequest(string[] parameters, string message)
        {
            if (message == null)
                return;

            var completeMessage = GettingParametrizedMessage(parameters, message);

            _messageHandler.Handle(new DomainNotification(completeMessage, DomainNotificationType.BadRequest), default);
        }

        private static string GettingParametrizedMessage(string[] parameters, string completeMessage)
        {
            var externalCounter = 0;
            foreach (var parameter in parameters)
            {
                completeMessage = completeMessage.Replace("{0}", parameter);
                externalCounter++;

                completeMessage = completeMessage.Replace("{" + externalCounter + "}", "{0}");
            }

            return completeMessage;
        }

        public string[] EmptyPositions()
        {
            return new string[] { };
        }
    }
}
