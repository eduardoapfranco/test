using System.Collections.Generic;

namespace Domain.Input.Fovea
{
    public class FoveaReceipt
        {
        public string ApplicationUsername { get; set; }
        public Dictionary<string, FoveaPurchase> Purchases { get; set; }

        }
    }
