using Domain.Entities;
using Domain.Input.RDStation;
using Infra.CrossCutting.Domain.Interfaces;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
    {
    public interface IRDStationDomainService : IDomainService<User, int>
        {
        Task<Conversion> PostConversionAsync(User user, Domain.Input.RDStation.RDStationInput input);
        }
    }
