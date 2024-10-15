using System;
using System.Collections.Generic;
using Domain.Entities;
using Domain.Enum;

namespace UnitTest.Domain.Faker
{
    internal static class PlanFaker
    {
        public static Plan CreatePlanFreemium()
        {
            return new Plan()
            {
                Type = PlanTypesEnum.Mensal,
                Active = 1,
                Id = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Content = "Freemium",
                Value = 0,
                ValueSave = 0,
                ValueFinally = 0,
                Title = "Freemium",
                PlanType = new PlanType()
                    {
                    Days = 30
                    }
                };
        }

        public static List<Plan> CreateListPlansFreemium()
        {
            return new List<Plan>()
            {
                CreatePlanFreemium()
            };
        }

        public static List<Plan> CreateListPlansPreemium()
        {
            return new List<Plan>()
            {
                CreatePlanPremium()
            };
        }

        public static List<Plan> CreateListPlansPreemiumTrial()
            {
            return new List<Plan>()
            {
                CreatePlanPremiumTrial()
            };
            }

        public static Plan CreatePlanPremium()
        {
            return new Plan()
            {
                Type = PlanTypesEnum.Mensal,
                Active = 1,
                Id = 2,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Content = "Premium",
                Value = 10,
                ValueSave = 0,
                ValueFinally = 10,
                Title = "Premium",
                PlanType = new PlanType()
                    {
                    Days = 30
                    }
                };
        }

        public static Plan CreatePlanPremiumTrial()
            {
            return new Plan()
                {
                Type = PlanTypesEnum.Trial,
                Active = 1,
                Id = 6,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Content = "Trial",
                Value = 0,
                ValueSave = 0,
                ValueFinally = 0,
                Title = "Trial",
                PlanType = new PlanType()
                    {
                    Days = 7
                    }
                };
            }

        }
}
