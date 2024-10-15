using AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace Application.Mappers
{
    [ExcludeFromCodeCoverage]
    public class AutoMapperConfig
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(
                config => {
                    config.AddProfile<EntityToViewModelMapperProfile>();
                    config.AddProfile<InputToEntityMapperProfile>();
                    config.AddProfile<InputToViewModelMapperProfile>();
                    config.AddProfile<EntityToEntitySQLiteMapperProfile>();
                }
            );
        }
    }
}
