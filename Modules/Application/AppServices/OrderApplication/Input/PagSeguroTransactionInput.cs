using Application.AppServices.OrderApplication.Input.Pagseguro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Application.AppServices.OrderApplication.Input
    {

    [XmlRootAttribute("transaction", Namespace = "", IsNullable = true)]
    public class PagSeguroTransactionInput
        {
        public static PagSeguroTransactionInput Load(string xml)
            {
            XmlSerializer serializer = new XmlSerializer(typeof(PagSeguroTransactionInput));
            var buffer = Encoding.UTF8.GetBytes(xml);
            PagSeguroTransactionInput pagSeguroTransaction;
            using (var stream = new MemoryStream(buffer))
                {
                pagSeguroTransaction = (PagSeguroTransactionInput)serializer.Deserialize(stream);
                }

            return pagSeguroTransaction;
            }

        public string ToXML()
            {
            var memoryStream = new MemoryStream();
            TextWriter stringWriter = new StreamWriter(memoryStream, System.Text.Encoding.UTF8);
            XmlSerializer serializer = new XmlSerializer(typeof(PagSeguroTransactionInput));
            serializer.Serialize(stringWriter, this);
            return System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

        /// <remarks/>
        [XmlElement("date")]
        public System.DateTime Date { get; set; }
        [XmlElement("code")]
        public string Code { get; set; }
        [XmlElement("reference")]
        public string Reference { get; set; }
        [XmlElement("type")]
        public byte Type { get; set; }
        [XmlElement("status")]
        public TransactionStatus Status { get; set; }
        [XmlElement("lasteventdate")]
        public System.DateTime LastEventDate { get; set; }
        [XmlElement("paymentmethod")]
        public TransactionPaymentMethod PaymentMethod { get; set; }
        [XmlElement("grossamount")]
        public decimal GrossAmount { get; set; }
        [XmlElement("discountamount")]
        public decimal DiscountAmount { get; set; }
        [XmlElement("feeamount")]
        public decimal FeeAmount { get; set; }
        [XmlElement("netamount")]
        public decimal NetAmount { get; set; }
        [XmlElement("extraamount")]
        public decimal ExtraAmount { get; set; }
        [XmlElement("installmentcount")]
        public byte InstallmentCount { get; set; }
        [XmlElement("itemcount")]
        public byte ItemCount { get; set; }
        [XmlArray("items")]
        [XmlArrayItem("item")]
        public List<TransactionItem> Items { get; set; }
        [XmlElement("sender")]
        public TransactionSender Sender { get; set; }
        [XmlElement("shipping")]
        public TransactionShipping Shipping { get; set; }

        }

    /// <remarks/>

    
   

    
    }
