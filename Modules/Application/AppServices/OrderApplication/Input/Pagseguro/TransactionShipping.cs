using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Application.AppServices.OrderApplication.Input.Pagseguro
    {
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class TransactionShipping
        {
        [XmlElement("address")]
        public TransactionShippingAddress Address { get; set; }
        [XmlElement("type")]
        public byte? Type { get; set; }
        public bool ShouldSerializeType()
            {
            return Type.HasValue;
            }
        [XmlElement("cost")]
        public decimal? Cost { get; set; }
        public bool ShouldSerializeCost()
            {
            return Cost.HasValue;
            }
        [XmlElement("addressRequired")]
        public string AddressRequired { get; set; }
        }
    }
