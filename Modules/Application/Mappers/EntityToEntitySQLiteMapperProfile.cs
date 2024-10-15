using Domain.Entities;
using Domain.Entities.Sqlite;
using System.Diagnostics.CodeAnalysis;

namespace Application.Mappers
{
    [ExcludeFromCodeCoverage]
    public class EntityToEntitySQLiteMapperProfile : AutoMapper.Profile
    {
        public EntityToEntitySQLiteMapperProfile()
        {
            CreateMap<Category, CategorySqlite>();
            CreateMap<Checklist, ChecklistSqlite>();
        }
    }
}
