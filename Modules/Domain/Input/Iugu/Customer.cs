using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Input.Iugu
    {
    public class Customer
        {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("errors")]
        public Dictionary<string, string[]> Errors { get; set; }

        public string getErrors()
            {
            if (null != this.Errors && this.Errors.Count > 0)
                {
                return string.Join(";", this.Errors.Select(kv => kv.Key + "=" + string.Join(",", kv.Value)).ToArray());
                }

            return string.Empty;
            }
        }
}
