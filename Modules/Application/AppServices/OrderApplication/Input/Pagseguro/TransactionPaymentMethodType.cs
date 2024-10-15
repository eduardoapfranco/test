using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Application.AppServices.OrderApplication.Input.Pagseguro
    {
    public enum TransactionPaymentMethodType
        {
        [XmlEnum(Name = "1")]
        CARTAO_CREDITO,
        [XmlEnum(Name = "2")]
        BOLETO,
        [XmlEnum(Name = "3")]
        DEBITO_ONLINE_TEF,
        [XmlEnum(Name = "4")]
        SALDO_PAGSEGURO,
        [XmlEnum(Name = "5")]
        OI_PAGGO,
        [XmlEnum(Name = "7")]
        DEPOSITO_EM_CONTA
        }
    }
