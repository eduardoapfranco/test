using Application.AppServices.ChecklistApplication.Input;
using Application.AppServices.ChecklistApplication.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IChecklistApplication
    {
        Task<IEnumerable<ChecklistViewModel>> GetChecklistsLastDateUpdatedAsync(DateTime? lastDateSync);
        Task<ExportChecklistPDFViewModel> ExportChecklistsToPDF(ExportChecklistInput input);
        Task<ExportChecklistViewModel> ExportChecklists(ExportChecklistInput input);
        Task<IEnumerable<ChecklistViewModel>> SelectByCategoryIdAsync(int categoryId);
    }
}