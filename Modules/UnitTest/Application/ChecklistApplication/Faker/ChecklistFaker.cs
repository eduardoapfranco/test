using Application.AppServices.ChecklistApplication.ViewModel;
using Domain.Entities;
using FizzWare.NBuilder;
using System.Collections.Generic;

namespace UnitTest.Application.ChecklistApplication.Faker
{
    public static class ChecklistFaker
    {
        public static Checklist CreateChecklist => Builder<Checklist>.CreateNew().Build();
        public static ChecklistViewModel ChecklistViewModel => Builder<ChecklistViewModel>.CreateNew().Build();

        public static IEnumerable<Checklist> CreateListChecklist()
        {
            var list = new List<Checklist>()
            {
                CreateChecklist
            };
            return list;
        }

    }
}
