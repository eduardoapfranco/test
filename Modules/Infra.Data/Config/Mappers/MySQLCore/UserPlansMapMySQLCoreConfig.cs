using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class UserPlansMapMySQLCoreConfig : IEntityTypeConfiguration<UserPlans>
    {
        public void Configure(EntityTypeBuilder<UserPlans> builder)
        {
            builder.ToTable("user_plans");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.UserId).HasColumnName("user_id");
            builder.Property(c => c.PlanId).HasColumnName("plan_id");
            builder.Property(c => c.ValueDebit).HasColumnName("value_debit");
            builder.Property(c => c.StatusPayment).HasColumnName("status_payment");
            builder.Property(c => c.Deleted).HasColumnName("deleted");
            builder.Property(c => c.DueInstallment).HasColumnName("due_installment");
            builder.Property(c => c.DueDateAt).HasColumnName("due_date_at");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.Property(c => c.IuguSignatureId).HasColumnName("iugu_signature_id");
            builder.HasKey(k => k.Id);
        }
    }
}
