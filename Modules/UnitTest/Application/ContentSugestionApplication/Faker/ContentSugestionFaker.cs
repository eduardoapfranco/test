using Application.AppServices.ContentSugestionApplication.Input;
using Application.AppServices.ContentSugestionApplication.ViewModel;
using Domain.Entities;
using FizzWare.NBuilder;

namespace UnitTest.Application.ContentSugestionApplication.Faker
    {
    public static class ContentSugestionFaker
    {
        public static ContentSugestion CreateContentSugestion => Builder<ContentSugestion>.CreateNew().Build();
        public static ContentSugestionInput CreateContentSugestionInput => Builder<ContentSugestionInput>.CreateNew().Build();
        public static ContentSugestionViewModel ContentSugestionViewModel => Builder<ContentSugestionViewModel>.CreateNew().Build();
    }
}
