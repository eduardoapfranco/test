using Application.AppServices.OrderApplication.Input;
using Application.AppServices.UserApplication.ViewModel;
using Domain.Entities;
using FizzWare.NBuilder;
using FluentValidation.Results;
using System.Collections.Generic;
using Domain.Input;

namespace UnitTest.Application.OrderApplication.Faker
{
    public static class WebhookFaker
        {
        public static WebhookPagSeguroNotificationInput CreateWebhookPagSeguroNotificationInput()
            {
            var webhookInput = Builder<WebhookPagSeguroNotificationInput>.CreateNew().Build();
            webhookInput.NotificationCode = "ABC123";
            webhookInput.NotificationType = "Transaction";

            return webhookInput;
            }

        public static PagSeguroNotificationInput CreatePagSeguroNotificationInput()
            {
            var webhookInput = Builder<PagSeguroNotificationInput>.CreateNew().Build();
            webhookInput.NotificationCode = "ABC123";
            webhookInput.NotificationType = "Transaction";

            return webhookInput;
            }
        }
}
