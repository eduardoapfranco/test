using Domain.Entities;
using System;

namespace UnitTest.Domain.Faker
{
    internal static class UserPlanFaker
    {
        public static UserPlans CreateUserPlan()
        {
            return new UserPlans()
            {
                UserId = 1,
                PlanId = 1,
                DueDateAt = DateTime.Now,
                ValueDebit = 0,
                StatusPayment = 0,
                Deleted = 0,
                DueInstallment = 0
            };
        }
    }

}
