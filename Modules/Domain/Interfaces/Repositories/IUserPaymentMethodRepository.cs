using Domain.Entities;
using Infra.CrossCutting.Repository.Interfaces;

namespace Domain.Interfaces.Repositories
{
    public interface IUserPaymentMethodRepository : IRepository<UserPaymentMethod, long>
    {
    }
}
