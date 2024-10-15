using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class UserPaymentMethodMapMySQLCoreConfig : IEntityTypeConfiguration<UserPaymentMethod>
    {
        public void Configure(EntityTypeBuilder<UserPaymentMethod> builder)
        {
            builder.ToTable("users_payment_methods");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.TransactionId).HasColumnName("transaction_id");
            builder.Property(c => c.Channel).HasColumnName("channel");
            builder.Property(c => c.Type).HasColumnName("type");
            builder.Property(c => c.Token).HasColumnName("token");
            builder.Property(c => c.Description).HasColumnName("description");
            builder.Property(c => c.Flag).HasColumnName("flag");
            builder.Property(c => c.LastFourDigits).HasColumnName("last_four_digits");
            builder.Property(c => c.Active).HasColumnName("active");
            builder.Property(c => c.UserId).HasColumnName("user_id");
            builder.Property(c => c.CustomerId).HasColumnName("customer_id");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
