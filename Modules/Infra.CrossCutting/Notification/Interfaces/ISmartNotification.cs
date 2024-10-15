namespace Infra.CrossCutting.Notification.Interfaces
{
    public interface ISmartNotification
    {
        ISmartNotification Invoke();

        bool IsValid();

        void NewNotificationConflict(string[] parameters, string message);

        void NewNotificationBadRequest(string[] parameters, string message);

        string[] EmptyPositions();
    }
}
