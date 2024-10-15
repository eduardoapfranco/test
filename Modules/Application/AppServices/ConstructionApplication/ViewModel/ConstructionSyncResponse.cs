using System.Collections.Generic;

namespace Application.AppServices.ConstructionApplication.ViewModel
    {
    public class ConstructionSyncResponse
        {
        public IEnumerable<ConstructionViewModel> toInsert { get; set; }
        public IEnumerable<ConstructionViewModel> toUpdate { get; set; }
        public IEnumerable<string> toDelete { get; set; }
        }
    }
