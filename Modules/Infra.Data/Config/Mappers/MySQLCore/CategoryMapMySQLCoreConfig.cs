using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class CategoryMapMySQLCoreConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.Title).HasColumnName("title");
            builder.Property(c => c.InnerCode).HasColumnName("codigo_interno");
            builder.Property(c => c.Content).HasColumnName("content");
            builder.Property(c => c.Order).HasColumnName("order");
            builder.Property(c => c.ParentId).HasColumnName("parent_id");
            builder.Property(c => c.Icon).HasColumnName("icon");
            builder.Property(c => c.UserId).HasColumnName("user_id");
            builder.Property(c => c.Active).HasColumnName("ativo");
            builder.Property(c => c.VisibleApp).HasColumnName("visible_app");
            builder.Property(c => c.Image).HasColumnName("image");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
