using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Application.AppServices.OrderApplication.Input.Pagseguro
    {
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class TransactionSenderPhone
        {
        [XmlElement("areacode")]
        public byte AreaCode { get; set; }
        [XmlElement("number")]
        public uint Number { get; set; }

        }
    }
