using Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Application.AppServices.OrderApplication.Input.Pagseguro
    {
    [XmlRootAttribute("checkout", Namespace = "", IsNullable = true)]
    public class Checkout
        {

        public static Checkout Load(string xml)
            {
            XmlSerializer serializer = new XmlSerializer(typeof(Checkout));
            var buffer = Encoding.UTF8.GetBytes(xml);
            Checkout pagseguroCheckout;
            using (var stream = new MemoryStream(buffer))
                {
                pagseguroCheckout = (Checkout)serializer.Deserialize(stream);
                }

            return pagseguroCheckout;
            }

        public static Checkout Load(User user, Plan plan, WebhookPagSeguroNotificationInput webhookPagseguroNotificationInput)
            {
            Checkout pagseguroCheckout = new Checkout()
                {
                Sender = new TransactionSender()
                    {
                    Email = user.Email,
                    Name = user.Name
                    },
                Currency = "BRL",
                RedirectURL = webhookPagseguroNotificationInput.UrlRedirectOrderForm,
                NotificationURL = webhookPagseguroNotificationInput.URLNotifications,
                Shipping = new TransactionShipping() { AddressRequired = "false" },
                Reference = String.Format("{0}|{1}", user.Email, plan.Id),
                Receiver = new CheckoutReceiver() { email = webhookPagseguroNotificationInput.Email }
                };

            pagseguroCheckout.Items = new System.Collections.Generic.List<TransactionItem>()
                {
                new TransactionItem()
                    {
                    Id = plan.Id,
                    Description = plan.Content,
                    Amount = plan.ValueFinally,
                    Quantity = 1
                    }
                };

            return pagseguroCheckout;
            }

        public string ToXML()
            {
            var memoryStream = new MemoryStream();
            TextWriter stringWriter = new StreamWriter(memoryStream, System.Text.Encoding.UTF8);
            XmlSerializer serializer = new XmlSerializer(typeof(Checkout));
            serializer.Serialize(stringWriter, this);
            return System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

        public Dictionary<string, string> ToFormParameter()
            {
            var formParameters = new Dictionary<string, string>();
            formParameters.Add("currency", this.Currency);
            formParameters.Add("itemId1", this.Items.FirstOrDefault().Id.ToString());
            formParameters.Add("itemDescription1", this.Items.FirstOrDefault().Description);
            formParameters.Add("itemAmount1", this.Items.FirstOrDefault().Amount
                .ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
            formParameters.Add("itemQuantity1", this.Items.FirstOrDefault().Quantity.ToString());
            formParameters.Add("reference", this.Reference);
            formParameters.Add("senderName", this.Sender.Name);
            formParameters.Add("senderEmail", this.Sender.Email);
            formParameters.Add("notificationURL", this.NotificationURL);
            formParameters.Add("redirectURL", this.RedirectURL);
            //formParameters.Add("maxUses", "1");
            //formParameters.Add("maxAge", "3000");
            //formParameters.Add("paymentMethodGroup1", "CREDIT_CARD");
            //formParameters.Add("paymentMethodConfigKey1_1", "MAX_INSTALLMENTS_LIMIT");
            //formParameters.Add("paymentMethodConfigValue1_1", "4");
            formParameters.Add("shippingAddressRequired", this.Shipping.AddressRequired);

            return formParameters;
            }

        [XmlElement("sender")]
        public TransactionSender Sender { get; set; }
        [XmlElement("currency")]
        public string Currency { get; set; }
        [XmlElement("redirectURL")]
        public string RedirectURL { get; set; }
        [XmlElement("notificationURL")]
        public string NotificationURL { get; set; }
        [XmlElement("reference")]
        public string Reference { get; set; }
        [XmlElement("receiver")]
        public CheckoutReceiver Receiver { get; set; }
        [XmlElement("shipping")]
        public TransactionShipping Shipping { get; set; }
        [XmlArray("items")]
        [XmlArrayItem("item")]
        public List<TransactionItem> Items { get; set; }
        }

    public class CheckoutReceiver
        {
        [XmlElement("email")]
        public string email { get; set; }

        }
    }
