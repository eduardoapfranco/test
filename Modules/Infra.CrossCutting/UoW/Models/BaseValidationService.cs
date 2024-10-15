using Infra.CrossCutting.Notification.Interfaces;

namespace Infra.CrossCutting.UoW.Models
{
    public class BaseValidationService
    {
        private readonly ISmartNotification _notify;

        public BaseValidationService(ISmartNotification notify)
        {
            _notify = notify.Invoke();
        }

        protected void NotifyErrorsAndValidation(string[] variablesSubstituion, BaseResult message)
        {
            foreach (var error in message.ValidationResult.Errors)
                _notify.NewNotificationBadRequest(variablesSubstituion, error.ErrorMessage);
        }
    }
}
