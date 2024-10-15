using Domain.Entities;
using Infra.CrossCutting.Domain.Interfaces;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
    {
    public interface IConstructionReportsDomainService : IDomainService<ConstructionReports, int>
        {
        public Task<ConstructionReportsTypes> getReportType(int typeId);
        }
    }
