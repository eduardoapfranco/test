using Domain.Enum;
using Infra.CrossCutting.Repository;
using System;

namespace Domain.Entities
{
    public class UserPlans : BaseEntity<int>
    {
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public decimal ValueDebit { get; set; }
        public sbyte StatusPayment { get; set; }
        public sbyte Deleted { get; set; }
        public sbyte DueInstallment { get; set; }
        public DateTime DueDateAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string IuguSignatureId { get; set; }

        public UserPlans()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public UserPlans CreateDefaultFreemium()
        {
            return new UserPlans()
            {
                Id = 1,
                PlanId = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ValueDebit = 0,
                StatusPayment = (sbyte) BoolEnum.NO,
                Deleted = (sbyte)BoolEnum.NO,
                DueInstallment = 0,
                DueDateAt = DateTime.Now
            };
        }
    }
}
