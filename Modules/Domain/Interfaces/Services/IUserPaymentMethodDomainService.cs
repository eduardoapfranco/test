using System.Threading.Tasks;
using Domain.Entities;
using Infra.CrossCutting.Domain.Interfaces;

namespace Domain.Interfaces.Services
{
    public interface IUserPaymentMethodDomainService : IDomainService<UserPaymentMethod, long>
    {
        Task<UserPaymentMethod> InsertAndInactiveAsync(UserPaymentMethod entity);
    }
}
