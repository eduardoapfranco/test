using Application.AppServices.UserApplication.ViewModel;
using Domain.ValueObjects;
using System.Collections.Generic;

namespace Application.AppServices.UserApplication.Mappers
{
    public static class UserControlAccessCategoriesViewModelMapper
    {

        public static IEnumerable<UserControlAccessCategoriesViewModel> ToViewModel(UserControlAccessVO valueObject)
        {
            var list = new List<UserControlAccessCategoriesViewModel>();
            foreach (var category in valueObject.Categories)
            {
                list.Add(new UserControlAccessCategoriesViewModel()
                {
                    Id = category.Id,
                    Content = category.Content,
                    Icon = category.Icon,
                    Order = category.Order,
                    ParentId = category.ParentId,
                    Title = category.Title
                });
            }
            return list;
        }
    }
}
