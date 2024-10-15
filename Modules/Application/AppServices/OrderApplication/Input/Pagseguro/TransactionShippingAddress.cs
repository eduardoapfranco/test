using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Application.AppServices.OrderApplication.Input.Pagseguro
    {
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class TransactionShippingAddress
        {
        [XmlElement("street")]
        public string Street { get; set; }
        [XmlElement("number")]
        public ushort Number { get; set; }
        [XmlElement("complement")]
        public string Complement { get; set; }
        [XmlElement("district")]
        public string District { get; set; }
        [XmlElement("postalcode")]
        public uint PostalCode { get; set; }
        [XmlElement("city")]
        public string City { get; set; }
        [XmlElement("state")]
        public string State { get; set; }
        [XmlElement("country")]
        public string Country { get; set; }
        }
    }
