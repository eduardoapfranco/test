using Application.AppServices.UserApplication.Input;
using Application.AppServices.UserApplication.ViewModel;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserApplication
    {
        Task<UserViewModel> InsertMobileAsync(UserInput input);
        Task<UserViewModel> LoginAsync(UserLoginInput input);
        Task<UserRequestPasswordResetViewModel> RequestPasswordResetAsync(UserRequestPasswordResetInput input);
        Task<UserViewModel> ResetPasswordAsync(UserResetPasswordInput input);
        Task<UserControlAccessVOViewModel> GetControlAccessAsync(int userId);
        Task<UserControlAccessVOViewModel> UpdateAsync(int userId, UserUpdateInput input);
        Task<UserViewModel> UpdatePasswordAsync(int userId, UserUpdatePasswordInput input);
    }
}
