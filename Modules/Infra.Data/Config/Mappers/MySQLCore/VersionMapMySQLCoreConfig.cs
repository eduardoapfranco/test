using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class VersionMapMySQLCoreConfig : IEntityTypeConfiguration<Version>
    {
        public void Configure(EntityTypeBuilder<Version> builder)
        {
            builder.ToTable("version_apps");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c._Version).HasColumnName("version");
            builder.Property(c => c.Platform).HasColumnName("platform");
            builder.Property(c => c.RequestPayment).HasColumnName("request_payment");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
