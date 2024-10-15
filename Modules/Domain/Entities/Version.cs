using Infra.CrossCutting.Repository;
using System;

namespace Domain.Entities
{
    public class Version : BaseEntityDates<int>
    {
        public string _Version { get; set; }
        public string Platform { get; set; }
        public Boolean RequestPayment { get; set; }
    }
}
