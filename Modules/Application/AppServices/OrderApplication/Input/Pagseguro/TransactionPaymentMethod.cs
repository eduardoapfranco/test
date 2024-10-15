using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Application.AppServices.OrderApplication.Input.Pagseguro
    {
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class TransactionPaymentMethod
        {
        [XmlElement("type")]
        public TransactionPaymentMethodType Type { get; set; }
        [XmlElement("code")]
        public byte Code { get; set; }

        }
    }
