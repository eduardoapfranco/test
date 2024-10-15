using Application.AppServices.DbMobileApplication.Input;
using Application.AppServices.DbMobileApplication.ViewModel;
using Application.AppServices.UserApplication.Input;
using Application.AppServices.UserApplication.ViewModel;
using System.Diagnostics.CodeAnalysis;
using Application.AppServices.DbMobileApplication.Input;
using Application.AppServices.DbMobileApplication.ViewModel;
using Application.AppServices.RatingApplication.Input;
using Application.AppServices.RatingApplication.ViewModel;
using Application.AppServices.ContentSugestionApplication.Input;
using Application.AppServices.ContentSugestionApplication.ViewModel;
using Application.AppServices.ConstructionApplication.Input;
using Application.AppServices.ConstructionApplication.ViewModel;

namespace Application.Mappers
{
    [ExcludeFromCodeCoverage]
    public class InputToViewModelMapperProfile : AutoMapper.Profile
    {
        public InputToViewModelMapperProfile()
        {
            CreateMap<UserInput, UserViewModel>().ForMember(x => x.Token, opt => opt.Ignore());
            CreateMap<UserLoginInput, UserViewModel>().ForMember(x => x.Token, opt => opt.Ignore());
            CreateMap<DbMobileInput, DbMobileViewModel>();
            CreateMap<RatingInput, RatingViewModel>();
            CreateMap<ContentSugestionInput, ContentSugestionViewModel>();
            CreateMap<ConstructionInput, ConstructionViewModel>();
            CreateMap<UserUpdateInput, UserControlAccessVOViewModel>();
            }
    }
}
