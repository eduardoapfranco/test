using System.Threading.Tasks;
using Application.AppServices.DbMobileApplication.Input;
using Application.AppServices.DbMobileApplication.ViewModel;

namespace Application.Interfaces
{
    public interface IDbMobileApplication
    {
        Task<DbMobileViewModel> CreateDBMobileAsync(DbMobileInput input, string rootPath, string folder, string path = "");

        Task<DbMobileViewModel> GetLastDBMobileGeneratedAsync(string folder);

        Task<LastUpdatedDatesViewModel> GetLastUpdatedDatesAsync();

        Task DeleteDBMobileAsync(string folder, bool deleteZip = false);
    }
}
