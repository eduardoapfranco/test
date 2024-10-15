using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Application.AppServices.OrderApplication.Input.Pagseguro
    {
    /// <remarks/>
    public partial class TransactionItem
        {
        [XmlElement("id")]
        public int Id { get; set; }
        [XmlElement("description")]
        public string Description { get; set; }
        [XmlElement("quantity")]
        public byte Quantity { get; set; }
        [XmlElement("amount")]
        public decimal Amount { get; set; }

        }
    }
