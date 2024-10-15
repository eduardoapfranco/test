using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class UserAreasMapMySQLCoreConfig : IEntityTypeConfiguration<UserAreas>
    {
        public void Configure(EntityTypeBuilder<UserAreas> builder)
        {
            builder.ToTable("user_areas");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.UserId).HasColumnName("user_id");
            builder.Property(c => c.AreaId).HasColumnName("area_id");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
