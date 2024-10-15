using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class ProfileCategoryMapMySQLCoreConfig : IEntityTypeConfiguration<ProfileCategory>
    {
        public void Configure(EntityTypeBuilder<ProfileCategory> builder)
        {
            builder.ToTable("profiles_categories");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.ProfileId).HasColumnName("profile_id");
            builder.Property(c => c.CategoryId).HasColumnName("category_id");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
