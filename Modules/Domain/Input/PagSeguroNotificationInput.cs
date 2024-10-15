using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Input
    {
    public class PagSeguroNotificationInput
        {
        public string NotificationCode { get; set; }
        public string NotificationType { get; set; }
        public string UrlBase { get; set; }
        public string UrlBaseOrderForm { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        }
    }
