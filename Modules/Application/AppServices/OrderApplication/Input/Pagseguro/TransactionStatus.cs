using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Application.AppServices.OrderApplication.Input.Pagseguro
    {
    public enum TransactionStatus
        {
        [XmlEnum(Name = "1")]
        AGUARDANDO_PAGAMENTO,
        [XmlEnum(Name = "2")]
        EM_ANALISE,
        [XmlEnum(Name = "3")]
        PAGA,
        [XmlEnum(Name = "4")]
        DISPONIVEL,
        [XmlEnum(Name = "5")]
        EM_DISPUTA,
        [XmlEnum(Name = "6")]
        DEVOLVIDA,
        [XmlEnum(Name = "7")]
        CANCELADA,
        [XmlEnum(Name = "8")]
        DEBITADO,
        [XmlEnum(Name = "9")]
        RETENCAO_TEMPORARIA
        }
    }
