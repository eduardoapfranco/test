using Domain.Entities;
using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Utils
{
    public static class CreateDefaultFreemium
    {
        public static Plan CreatePlan()
        {
            return new Plan()
            {
                Id = (int)PlanDefaultFreemiumEnum.Freemium,
                Title = "Freemium",
                Content = "Acesso gratuito",
                Value = 0,
                ValueSave = 0,
                ValueFinally = 0,
                Type = PlanTypesEnum.Mensal,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Active = (byte)BoolEnum.YES,
                PlanType = new PlanType()
                {
                    Id = (int)PlanTypeDefaultFreemiumEnum.Mensal,
                    Title = "MENSAL",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };
        }

        public static Profile CreateProfile()
        {
            return new Profile()
            {
                Id = (int)ProfileDefaultFreemiumEnum.Acesso_Gratuito,
                Title = "Acesso Gratuito",
                Content = "Acesso Gratuito",
                PlanId = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Active = (byte)BoolEnum.YES
            };
        }

        public static List<Category> CreateProfileCategories()
        {
            var categories = new List<Category>()
            {
                new Category()
                {
                    Id = (int) CategoriesDefaultFreemiumEnum.Servicos_Preliminares_Gerais,
                    Title = "Serviços Preliminares e Gerais",
                    Content = "<p>Os servi&ccedil;os preliminares, s&atilde;o os servi&ccedil;os que antecedem o inicio de fato de uma obra.</p>",
                    Order = 1,
                    Icon = null,
                    ParentId = null
                },
                new Category()
                {
                    Id = (int) CategoriesDefaultFreemiumEnum.Outros_Servicos,
                    Title = "Outros Serviços",
                    Content = "<p>Outros Servi&ccedil;os</p>",
                    Order = 23,
                    Icon = null,
                    ParentId = null
                }
            };
            return categories;
        }
    
        public static List<Functionality> CreateProfileFunctionalities()
        {
            return new List<Functionality>();
        }
    }
}
