using Domain.Entities;
using Domain.Enum;
using System;

namespace IntegrationTest.Scenarios.Auth.Faker
{
    public static class UserPlanFaker
    {
        public static UserPlans Create(int planId, int userId, DateTime startDate, sbyte statusPayment, sbyte deleted, DateTime endDate)
        {
            return new UserPlans()
            {
                CreatedAt = startDate,
                Deleted = deleted,
                DueDateAt = endDate,
                PlanId = planId,
                StatusPayment = statusPayment,
                UpdatedAt = startDate,
                UserId = userId,
                ValueDebit = 1
            };
        }

        public static UserPlans CreateUserPlanFreemium(int userId)
        {
            return Create((int)PlanWithTypeEnum.FREEMIUM_MENSAL, userId, DateTime.Now, 0, 0, DateTime.Now);
        }

        public static UserPlans CreateUserPlanPremium(int planId, int userId, DateTime startDate, DateTime endDate, sbyte statusPayment)
        {
            return Create(planId, userId, startDate, statusPayment, 0, endDate);
        }
    }
}
