using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class RatingMapMySQLCoreConfig : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.ToTable("rating_mobiles");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c._Rating).HasColumnName("rating");
            builder.Property(c => c.Comment).HasColumnName("comment");
            builder.Property(c => c.Title).HasColumnName("title");
            builder.Property(c => c.Content).HasColumnName("content");
            builder.Property(c => c.CategoryId).HasColumnName("category_id");
            builder.Property(c => c.ChecklistId).HasColumnName("checklist_id");
            builder.Property(c => c.UserId).HasColumnName("user_id");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
