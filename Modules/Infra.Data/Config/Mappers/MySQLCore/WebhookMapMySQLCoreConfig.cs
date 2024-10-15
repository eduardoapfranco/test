using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class WebhookMapMySQLCoreConfig : IEntityTypeConfiguration<Webhook>
    {
        public void Configure(EntityTypeBuilder<Webhook> builder)
        {
            builder.ToTable("webhooks");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.Partner).HasColumnName("partner");
            builder.Property(c => c.Type).HasColumnName("type");
            builder.Property(c => c.Subtype).HasColumnName("subtype");
            builder.Property(c => c.Content).HasColumnName("content");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
