using Infra.CrossCutting.UoW.Models;
using System;
using Newtonsoft.Json;

namespace Application.AppServices.ConstructionApplication.ViewModel
{
    public class ConstructionViewModel : BaseResult
        {
      
        public int Id { get; set; }
        [JsonProperty("appId")]
        public string AppId { get; set; }
        [JsonProperty("userId")]
        public int UserId { get; set; }
        public string Nome { get; set; }
        public string Responsavel { get; set; }
        public string Contratante { get; set; }
        public string Endereco { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Termino { get; set; }
        public decimal? Metragem { get; set; }
        [JsonProperty("valor_estimado")]
        public decimal? ValorEstimado { get; set; }
        public string Status { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        [JsonProperty("deleted")]
        public bool Deleted { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
        public bool IsNewer(ConstructionViewModel that)
            {
            if (!this.UpdatedAt.HasValue || !that.UpdatedAt.HasValue)
                return true;

            return this.UpdatedAt.Value > that.UpdatedAt.Value;
            }
        }
}
