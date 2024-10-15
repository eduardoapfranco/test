using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class PlanTypeMapMySQLCoreConfig : IEntityTypeConfiguration<PlanType>
    {
        public void Configure(EntityTypeBuilder<PlanType> builder)
        {
            builder.ToTable("plans_types");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.Title).HasColumnName("title");
            builder.Property(c => c.Days).HasColumnName("days");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
