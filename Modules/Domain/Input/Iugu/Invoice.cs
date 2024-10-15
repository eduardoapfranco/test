using Newtonsoft.Json;
using System;

namespace Domain.Input.Iugu
    {
    public class Invoice
        {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("due_date")]
        public DateTime? DueDate { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("total")]
        public string Total { get; set; }
        [JsonProperty("secure_url")]
        public string SecureUrl { get; set; }

        }
    }