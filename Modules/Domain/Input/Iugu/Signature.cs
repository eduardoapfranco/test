using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Input.Iugu
    {
    public class Signature
        {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("plan_identifier")]
        public string PlanIdentifier { get; set; }
        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }
        [JsonProperty("two_step")]
        public bool TwoStep { get; set; }
        [JsonProperty("suspend_on_invoice_expired")]
        public bool SuspendOnInvoiceExpired { get; set; }
        [JsonProperty("only_charge_on_due_date")]
        public bool OnlyChargeOnDueDate { get; set; }
        [JsonProperty("suspended")]
        public bool Suspended { get; set; }
        [JsonProperty("recent_invoices")]
        public List<Invoice> RecentInvoices { get; set; }
        [JsonProperty("errors")]
        public Dictionary<string, string[]> Errors { get; set; }

       
        public Signature()
            {
            TwoStep = true;
            SuspendOnInvoiceExpired = true;
            OnlyChargeOnDueDate = true;
            }

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