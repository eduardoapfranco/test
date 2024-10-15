using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class ContentSugestionMapMySQLCoreConfig : IEntityTypeConfiguration<ContentSugestion>
    {
        public void Configure(EntityTypeBuilder<ContentSugestion> builder)
        {
            builder.ToTable("ratings");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.Type).HasColumnName("tipo");
            builder.Property(c => c.Title).HasColumnName("titulo");
            builder.Property(c => c.Content).HasColumnName("conteudo");
            builder.Property(c => c.CategoryId).HasColumnName("category_id");
            builder.Property(c => c.ChecklistId).HasColumnName("checklist_id");
            builder.Property(c => c.UserId).HasColumnName("user_id");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
