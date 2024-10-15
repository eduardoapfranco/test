using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class ChecklistMapMySQLCoreConfig : IEntityTypeConfiguration<Checklist>
    {
        public void Configure(EntityTypeBuilder<Checklist> builder)
        {
            builder.ToTable("checklists");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.Title).HasColumnName("title");
            builder.Property(c => c.Content).HasColumnName("content");
            builder.Property(c => c.Order).HasColumnName("order");
            builder.Property(c => c.Type).HasColumnName("type");
            builder.Property(c => c.CategoryId).HasColumnName("category_id");
            builder.Property(c => c.UserId).HasColumnName("user_id");
            builder.Property(c => c.Active).HasColumnName("ativo");
            builder.Property(c => c.VisibleApp).HasColumnName("visible_app");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.Property(c => c.CheckEnable).HasColumnName("check_enable");
            builder.HasKey(k => k.Id);
        }
    }
}
