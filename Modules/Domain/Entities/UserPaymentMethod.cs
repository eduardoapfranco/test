using Infra.CrossCutting.Repository;
using System;

namespace Domain.Entities
{
    public class UserPaymentMethod : BaseEntityDates<long>
    {
        public string TransactionId { get; set; }
        public int Channel { get; set; }
        public int Type { get; set; }
        public string Token { get; set; }
        public string Description { get; set; }
        public string Flag { get; set; }
        public int LastFourDigits { get; set; }
        public sbyte Active { get; set; }
        public int UserId { get; set; }
        public string CustomerId { get; set; }

        public UserPaymentMethod()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
