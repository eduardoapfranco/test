using Domain.ValueObjects;
using Infra.CrossCutting.UoW.Models;
using System.Collections.Generic;

namespace Application.AppServices.ChecklistApplication.ViewModel
{
    public class ExportChecklistPDFViewModel : BaseResult
    {
        public byte[] Pdf { get; set; }
    }

    public class ExportChecklistViewModel : BaseResult
    {
        public IEnumerable<ChecklistSectionExportVO> Checklists { get; set; }
    }
}
