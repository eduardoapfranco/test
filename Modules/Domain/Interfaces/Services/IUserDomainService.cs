using Domain.Entities;
using Domain.Input;
using Domain.ValueObjects;
using Infra.CrossCutting.Domain.Interfaces;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface IUserDomainService : IDomainService<User, int>
    {
        Task<int> RequestPasswordResetAsync(RequestPasswordResetInput input);
        Task<bool> ResetPasswordAsync(ResetPasswordInput input);
        Task<User> GetUserByEmailAsync(User user);
        Task<User> GetUserAsync(int userId);
        Task<bool> UpdateAreasAsync(int userId, int[] AreasIds);
        Task<UserControlAccessVO> GetControlAccessAsync(int userId);
    }
}
