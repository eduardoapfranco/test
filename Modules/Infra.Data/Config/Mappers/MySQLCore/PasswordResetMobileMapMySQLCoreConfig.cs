using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class PasswordResetMobileMapMySQLCoreConfig : IEntityTypeConfiguration<PasswordReset>
    {
        public void Configure(EntityTypeBuilder<PasswordReset> builder)
        {
            builder.ToTable("password_reset_mobiles");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.UserId).HasColumnName("user_id");
            builder.Property(c => c.UserEmail).HasColumnName("user_email");
            builder.Property(c => c.CheckerNumber).HasColumnName("checker_number");
            builder.Property(c => c.Active).HasColumnName("active");
            builder.Property(c => c.Used).HasColumnName("used");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
