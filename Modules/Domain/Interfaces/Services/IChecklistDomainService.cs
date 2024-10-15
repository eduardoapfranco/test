using Domain.Entities;
using Domain.Enum;
using Domain.ValueObjects;
using Infra.CrossCutting.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface IChecklistDomainService : IDomainService<Checklist, int>
    {
        Task<IEnumerable<ChecklistSectionExportVO>> SelectExportSectionPDF(int categoryId, int[] ids, ExportTypeChecklistEnum type);
    }
}
