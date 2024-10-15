using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Input.Iugu
{
    public class PaymentMethod
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("set_as_default")]
        public string SetAsDefault { get; set; }
        [JsonProperty("errors")]
        public Dictionary<string, string[]> Errors { get; set; }

        public string GetErrors()
        {
            if (null != this.Errors && this.Errors.Count > 0)
            {
                return string.Join(";", this.Errors.Select(kv => kv.Key + "=" + string.Join(",", kv.Value)).ToArray());
            }

            return string.Empty;
        }
    }
}
