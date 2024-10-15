using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.Infra.Infra.CrossCutting.Domain.Fake
{
    public static class FakeEntityDomainBase
    {
        public static EntityTestFaker Create()
        {
            return new EntityTestFaker() { Id = 1, ValueRandomTest = "Value" };
        }

        public static List<EntityTestFaker> CreateList()
        {
            var item1 = new EntityTestFaker() { Id = 1, ValueRandomTest = "Value" };
            var item2 = new EntityTestFaker() { Id = 2, ValueRandomTest = "Value" };
            var list = new List<EntityTestFaker>();
            list.Add(item1);
            list.Add(item2);
            return list;
        }
    }
}
