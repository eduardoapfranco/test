using Application.AppServices.ConstructionApplication.Input;
using Application.AppServices.ConstructionApplication.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
    {
    public interface IConstructionApplication
        {
        Task<ConstructionViewModel> InsertAsync(ConstructionInput input);
        Task<ConstructionViewModel> UpdateAsync(ConstructionInput input);
        Task<IEnumerable<ConstructionViewModel>> ListAsync(int userId);
        Task<ConstructionSyncResponse> SyncAsync(int userId, List<ConstructionViewModel> appConstrunctions);
        }
    }
