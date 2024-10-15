using Domain.Enum;
using Infra.CrossCutting.Repository;
using System;

namespace Domain.Entities
{
    public class UserAreas : BaseEntity<int>
    {
        public int UserId { get; set; }
        public int AreaId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserAreas()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

    }
}
