using System;
using System.Diagnostics.CodeAnalysis;

namespace Infra.CrossCutting.Repository
{
    [ExcludeFromCodeCoverage]
    public class BaseEntityDates<T> : BaseEntity<T>, IEntityAudited
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class BaseEntity<T> //: IEntityAudited
    {
        public T Id { get; set; }
    }

    public interface IEntityAudited
    {
    }
}
