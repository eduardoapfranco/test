using Infra.CrossCutting.UoW.Models;
using System;
using Newtonsoft.Json;

namespace Application.AppServices.VersionApplication.ViewModel
{
    public class VersionViewModel : BaseResult
    {
        public int Id { get; set; }
        [JsonProperty("version")]
        public string _Version { get; set; }
        public string Platform { get; set; }
        [JsonProperty("request_payment")]
        public bool RequestPayment { get; set; }
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
