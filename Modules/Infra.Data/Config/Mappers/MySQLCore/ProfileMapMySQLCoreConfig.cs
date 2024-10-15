using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class ProfileMapMySQLCoreConfig : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.ToTable("profiles");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.Title).HasColumnName("title");
            builder.Property(c => c.Content).HasColumnName("content");
            builder.Property(c => c.PlanId).HasColumnName("plan_id");
            builder.Property(c => c.Active).HasColumnName("active");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
