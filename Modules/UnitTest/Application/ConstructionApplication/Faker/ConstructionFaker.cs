using Application.AppServices.ConstructionApplication.Input;
using Application.AppServices.ConstructionApplication.ViewModel;
using Domain.Entities;
using FizzWare.NBuilder;
using System.Collections.Generic;

namespace UnitTest.Application.ConstructionApplication.Faker
{
    public static class ConstructionFaker
    {
        public static Construction CreateConstruction => Builder<Construction>.CreateNew().Build();
        public static ConstructionViewModel CreateConstructionViewModel => Builder<ConstructionViewModel>.CreateNew().Build();

        public static ConstructionInput CreateConstructionInput => Builder<ConstructionInput>.CreateNew().Build();

        public static IEnumerable<Construction> CreateListConstruction()
        {
            var list = new List<Construction>()
            {
                CreateConstruction
            };
            return list;
        }

        public static IEnumerable<ConstructionViewModel> CreateListConstructionViewModel()
            {
            var list = new List<ConstructionViewModel>()
            {
                CreateConstructionViewModel
            };
            return list;
            }

        }
}
