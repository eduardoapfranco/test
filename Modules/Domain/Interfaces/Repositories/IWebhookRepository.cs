using Domain.Entities;
using Infra.CrossCutting.Repository.Interfaces;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
    {
    public interface IWebhookRepository : IRepository<Webhook, int>
    {
    }
}
