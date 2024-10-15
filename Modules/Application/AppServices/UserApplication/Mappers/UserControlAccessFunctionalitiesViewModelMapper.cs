using Application.AppServices.UserApplication.ViewModel;
using Domain.ValueObjects;
using System.Collections.Generic;

namespace Application.AppServices.UserApplication.Mappers
{
    public static class UserControlAccessFunctionalitiesViewModelMapper
    {

        public static IEnumerable<UserControlAccessFunctionalitiesViewModel> ToViewModel(UserControlAccessVO valueObject)
        {
            var list = new List<UserControlAccessFunctionalitiesViewModel>();
            foreach (var functionality in valueObject.Functionalities)
            {
                list.Add(new UserControlAccessFunctionalitiesViewModel()
                {
                   Id = functionality.Id,
                   Title = functionality.Title,
                   Content = functionality.Content
                });
            }
            return list;
        }
    }
}
