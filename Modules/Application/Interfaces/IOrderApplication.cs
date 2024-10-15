using Application.AppServices.OrderApplication.Input;
using System.Threading.Tasks;

namespace Application.Interfaces
    {
    public interface IOrderApplication
    {
        Task<string> GetPagseguroOrderFormAsync(int userId, int planId, WebhookPagSeguroNotificationInput webhookPagseguroNotificationInput);
        Task<bool> ProcessPagseguroNotificationAsync(WebhookPagSeguroNotificationInput webhookPagseguroNotificationInput);
    }
}