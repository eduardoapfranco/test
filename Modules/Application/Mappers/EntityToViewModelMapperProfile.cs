using Application.AppServices.CategoryApplication.ViewModel;
using Application.AppServices.ChecklistApplication.ViewModel;
using Application.AppServices.ReminderApplication.ViewModel;
using Application.AppServices.ContentSugestionApplication.ViewModel;
using Application.AppServices.RatingApplication.ViewModel;
using Application.AppServices.UserApplication.ViewModel;
using Domain.Entities;
using System.Diagnostics.CodeAnalysis;
using Application.AppServices.VersionApplication.ViewModel;
using Application.AppServices.ConstructionApplication.ViewModel;
using Application.AppServices.AreaApplication.ViewModel;
using Application.AppServices.ConstructionReportApplication.ViewModel;

namespace Application.Mappers
{
    [ExcludeFromCodeCoverage]
    public class EntityToViewModelMapperProfile : AutoMapper.Profile
    {
        public EntityToViewModelMapperProfile()
            {
            CreateMap<User, UserViewModel>().ForMember(x => x.Token, opt => opt.Ignore());
            CreateMap<Category, CategoryViewModel>()
                .ForMember(categoryViewModel => categoryViewModel.TitleAutoComplete, b =>
                    b.MapFrom(categoryEntity => string.Format("{0} - #{1}", categoryEntity.Title, categoryEntity.Id.ToString())));
            CreateMap<Checklist, ChecklistViewModel>();
            CreateMap<Reminder, ReminderViewModel>();
            CreateMap<Rating, RatingViewModel>();
            CreateMap<ContentSugestion, ContentSugestionViewModel>();
            CreateMap<Domain.Entities.Version, VersionViewModel>();
            CreateMap<Domain.Entities.Construction, ConstructionViewModel>();
            CreateMap<Domain.Entities.ConstructionReports, ConstructionReportViewModel>();

            CreateMap<Domain.Entities.Area, AreaViewModel>();
            }
        }
    }
