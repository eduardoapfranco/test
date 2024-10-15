using Domain.Entities;
using Domain.Enum;
using System;
using System.Collections.Generic;

namespace UnitTest.Domain.Faker
{
    internal static class ChecklistToPDFFaker
    {
        public static Checklist Create(int id, string title, string content, ChecklistTypeEnum type)
        {
            return new Checklist()
            {
                Type = (int) type,
                Active = 1,
                CategoryId = 1,
                CheckEnable = 1,
                Content = content + " " + id,
                Id = id,
                CreatedAt = DateTime.Now,
                Order = id,
                Title = title + " " + id,
                UpdatedAt = DateTime.Now,
                UserId = 1,
                VisibleApp = 1
            };
        }

        public static List<Checklist> CreateList()
        {
            var checklists = new List<Checklist> {Create(1, "Grupo", "Conteúdo Grupo", ChecklistTypeEnum.GRUPO)};
            for (var i = 2; i <= 10; i++)
            {
                checklists.Add(Create(i, "Checklist ", "Conteúdo Checklist", ChecklistTypeEnum.NORMATIZADOS));
            }
            checklists.Add(Create(11, "Grupo", "Conteúdo Grupo", ChecklistTypeEnum.GRUPO));
            for (var i = 12; i <= 20; i++)
            {
                checklists.Add(Create(i, "Checklist ", "Conteúdo Checklist",  ChecklistTypeEnum.BOASPRATICAS));
            }
            checklists.Add(Create(21, "Grupo", "Conteúdo Grupo", ChecklistTypeEnum.GRUPO));
            for (int i = 22; i <= 30; i++)
            {
                checklists.Add(Create(i, "Checklist ", "Conteúdo Checklist", ChecklistTypeEnum.BIBLIOGRAFIA));
            }
            return checklists;
        }

    }
}
