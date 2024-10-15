using Application.AppServices.RatingApplication.Input;
using Application.AppServices.RatingApplication.ViewModel;
using System.Threading.Tasks;

namespace Application.Interfaces
    {
    public interface IRatingApplication
        {
        Task<RatingViewModel> InsertAsync(RatingInput input);
        }
    }
