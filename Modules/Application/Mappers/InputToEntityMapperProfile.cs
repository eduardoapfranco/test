using Application.AppServices.ConstructionApplication.Input;
using Application.AppServices.ConstructionApplication.ViewModel;
using Application.AppServices.ContentSugestionApplication.Input;
using Application.AppServices.RatingApplication.Input;
using Application.AppServices.UserApplication.Input;
using Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Application.Mappers
{
    [ExcludeFromCodeCoverage]
    public class InputToEntityMapperProfile : AutoMapper.Profile
    {
        public InputToEntityMapperProfile()
        {
            CreateMap<UserInput, User> ();
            CreateMap<RatingInput, Rating>();
            CreateMap<ContentSugestionInput, ContentSugestion>();
            CreateMap<ConstructionInput, Construction>();
            CreateMap<ConstructionViewModel, Construction>();
            }
    }
}
