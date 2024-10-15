using Domain.Input.RDStation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
    {
    public interface IRDApplication
    {
        Task<IEnumerable<string>> CreateRDStationConversion(RDStationInput input);
    }
}
