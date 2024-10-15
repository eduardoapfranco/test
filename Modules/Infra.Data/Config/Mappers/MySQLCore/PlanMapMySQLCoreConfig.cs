using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class PlanMapMySQLCoreConfig : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.ToTable("plans");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.IdGoogle).HasColumnName("id_google");
            builder.Property(c => c.IdApple).HasColumnName("id_apple");
            builder.Property(c => c.Title).HasColumnName("title");
            builder.Property(c => c.Content).HasColumnName("content");
            builder.Property(c => c.Value).HasColumnName("value");
            builder.Property(c => c.ValueSave).HasColumnName("value_save");
            builder.Property(c => c.ValueFinally).HasColumnName("value_finally");
            builder.Property(c => c.Type).HasColumnName("type");
            builder.Property(c => c.Active).HasColumnName("active");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
