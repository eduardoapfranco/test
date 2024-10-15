using Application.AppServices.ConstructionApplication.Input;
using Application.AppServices.ConstructionApplication.ViewModel;
using Domain.Entities;
using Domain.Enum;
using System;

namespace IntegrationTest.Scenarios.Construction.Faker
{
    public static class ConstructionFaker
    {
        public static ConstructionInput CreateInput()
        {
            return new ConstructionInput()
                {
                AppId = Guid.NewGuid().ToString()
                ,Nome = "Obra"
                ,Status = "Em Andamento"
                ,CreatedAt = DateTime.Now
                ,UpdatedAt = DateTime.Now
                ,Inicio = DateTime.Now
                ,Termino = DateTime.Now
                ,Responsavel = "Responsavel"
                ,Contratante = "Contratante"
                };
            }

        public static ConstructionViewModel CreateViewModel()
            {
            return new ConstructionViewModel()
                {
                AppId = Guid.NewGuid().ToString(),
                Nome = "Obra",
                Status = "Em Andamento",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Inicio = DateTime.Now,
                Termino = DateTime.Now,
                Responsavel = "Responsavel",
                Contratante = "Contratante"
                };
            }
        }
}
