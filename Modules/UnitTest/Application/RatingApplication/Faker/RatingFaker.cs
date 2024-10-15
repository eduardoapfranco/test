using Application.AppServices.RatingApplication.Input;
using Application.AppServices.RatingApplication.ViewModel;
using Domain.Entities;
using FizzWare.NBuilder;

namespace UnitTest.Application.RatingApplication.Faker
    {
    public static class RatingFaker
    {
        public static Rating CreateRating => Builder<Rating>.CreateNew().Build();
        public static RatingInput CreateRatingInput => Builder<RatingInput>.CreateNew().Build();
        public static RatingViewModel RatingViewModel => Builder<RatingViewModel>.CreateNew().Build();
    }
}
