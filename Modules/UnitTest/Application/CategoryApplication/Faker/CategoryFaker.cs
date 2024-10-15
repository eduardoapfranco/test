using Application.AppServices.CategoryApplication.ViewModel;
using Domain.Entities;
using FizzWare.NBuilder;
using System.Collections.Generic;

namespace UnitTest.Application.CategoryApplication.Faker
{
    public static class CategoryFaker
    {
        public static Category CreateCategory => Builder<Category>.CreateNew().Build();
        public static CategoryViewModel CategoryViewModel => Builder<CategoryViewModel>.CreateNew().Build();

        public static IEnumerable<Category> CreateListCategory()
        {
            var list = new List<Category>()
            {
                CreateCategory
            };
            return list;
        }

    }
}
