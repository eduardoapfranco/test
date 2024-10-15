using Application.AppServices.ChecklistApplication.Input;
using Application.AppServices.ConstructionApplication.Input;
using Application.AppServices.ConstructionApplication.ViewModel;
using Application.AppServices.ConstructionReportApplication.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
    {
    public interface IConstructionReportApplication
        {
        public Task<ConstructionReportViewModel> InsertAsync(int userId, ExportChecklistInput input, string path);
        public Task<IEnumerable<ConstructionReportViewModel>> ListReportsAsync(int userId, string constructionAppId, string path);
        public Task<ConstructionReportViewModel> GetReportAsync(int userId, int constructionReportId, string path);
        public Task<bool> DeleteReportAsync(int userId, int constructionReportId, string path);
        }
    }
