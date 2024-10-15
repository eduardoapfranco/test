using Infra.CrossCutting.Repository;
using System.Diagnostics.CodeAnalysis;

namespace UnitTest.Infra.Infra.CrossCutting.Domain.Fake
{
    [ExcludeFromCodeCoverage]
    public class EntityTestFaker : BaseEntity<int>
    {
        public string ValueRandomTest { get; set; }
    }
}
