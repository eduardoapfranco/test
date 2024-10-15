using Infra.CrossCutting.Repository;
using System;

namespace Domain.Entities
    {
    public class Construction : BaseEntityDates<int>
    {
        public string AppId { get; set; }
        public string Nome { get; set; }
        public string Responsavel { get; set; }
        public string Contratante { get; set; }
        public string Endereco { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Termino { get; set; }
        public decimal? Metragem { get; set; }
        public decimal? ValorEstimado { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public bool Deleted { get; set; }
        public string Image { get; set; }
    }
}
