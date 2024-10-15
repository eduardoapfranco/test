using Infra.CrossCutting.Notification.Enum;
using MediatR;
using Newtonsoft.Json;

namespace Infra.CrossCutting.Notification.Model
{
    public class DomainNotification : INotification
    {
        public string Value { get; }

        [JsonIgnore]
        public DomainNotificationType DomainNotificationType { get; }

        public DomainNotification(
            string value,
            DomainNotificationType type = DomainNotificationType.Conflict)
        {
            Value = value;
            DomainNotificationType = type;
        }
    }
}
