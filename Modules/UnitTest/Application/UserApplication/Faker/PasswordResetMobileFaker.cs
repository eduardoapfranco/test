using Domain.Entities;
using System;
using System.Collections.Generic;

namespace UnitTest.Application.UserApplication.Faker
{
    public static class PasswordResetMobileFaker
    {
        public static PasswordReset CreatePasswordResetMobile()
        {
            return new PasswordReset(1, "teste@teste.com")
            {
                Id = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        public static IEnumerable<PasswordReset> CreateListPasswordResetMobile()
        {
            return new List<PasswordReset>()
            {
                CreatePasswordResetMobile()
            };
        }
    }
}
