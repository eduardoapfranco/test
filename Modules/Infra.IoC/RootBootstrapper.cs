using Infra.IoC.Application;
using Infra.IoC.Domain;
using Infra.IoC.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Infra.IoC
{
    [ExcludeFromCodeCoverage]

    public class RootBootstrapper
    {
        public void RootRegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            new ApplicationBootstraper().ChildServiceRegister(services, configuration);
            new DomainBootstraper().ChildServiceRegister(services, configuration);
            new RepositoryBootstraper().ChildServiceRegister(services, configuration);
        }
    }
}
