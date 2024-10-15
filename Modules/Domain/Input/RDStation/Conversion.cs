using Newtonsoft.Json;
using System.Collections.Generic;

namespace Domain.Input.RDStation
{

    public class Conversion
        {
        [JsonProperty("event_uuid")]
        public string EventUuid { get; set; }

        [JsonProperty("event_type")]
        public string EventType { get; set; }
        [JsonProperty("event_family")]
        public string EventFamily { get; set; }

        [JsonProperty("payload")]
        public ConversionPayload Payload { get; set; }

        public Conversion()
            {
            Payload = new ConversionPayload();
            EventFamily = "CDP";
            EventType = "CONVERSION";
            }
        }

    public class ConversionPayload
        {
        [JsonProperty("conversion_identifier")]
        public string ConversionIdentifier { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("bio")]
        public string Bio { get; set; }
        [JsonProperty("job_title")]
        public string JobTitle { get; set; }
        [JsonProperty("linkedin")]
        public string LinkedinId { get; set; }
        [JsonProperty("facebook")]
        public string Facebook { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("twitter")]
        public string Twitter { get; set; }
        [JsonProperty("personal_phone")]
        public string PersonalPhone { get; set; }
        [JsonProperty("mobile_phone")]
        public string MobilePhone { get; set; }
        [JsonProperty("website")]
        public string Website { get; set; }
        [JsonProperty("client_tracking_id")]
        public string ClientTrackingId { get; set; }
        [JsonProperty("lead tracking client_id")]
        public string LeadTrackingClientId { get; set; }
        [JsonProperty("traffic_source")]
        public string TrafficSource { get; set; }
        [JsonProperty("traffic_medium")]
        public string TrafficMedium { get; set; }
        [JsonProperty("traffic_campaign")]
        public string TrafficCampaign { get; set; }
        [JsonProperty("traffic_value")]
        public string TrafficValue { get; set; }
        [JsonProperty("available_for_mailing")]
        public bool AvailableForMailing { get; set; }
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
        //[JsonProperty("legal_bases")]
        //public List<string> LegalBases { get; set; }

        public ConversionPayload()
            {
            ConversionIdentifier = "app_signup";
            AvailableForMailing = true;
            }
        }
    }