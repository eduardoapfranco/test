using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class ConstructionReportsMapMySQLCoreConfig : IEntityTypeConfiguration<ConstructionReports>
    {
        public void Configure(EntityTypeBuilder<ConstructionReports> builder)
        {
            builder.ToTable("construction_reports");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.Title).HasColumnName("title");
            builder.Property(c => c.ConstructionId).HasColumnName("construction_id");
            builder.Property(c => c.MimeType).HasColumnName("mime_type");
            builder.Property(c => c.PicturesQuantity).HasColumnName("pictures_quantity");
            builder.Property(c => c.Comments).HasColumnName("comments");
            builder.Property(c => c.Guarantee).HasColumnName("guarantee");
            builder.Property(c => c.BlobId).HasColumnName("blob_id");
            builder.Property(c => c.Value).HasColumnName("value");
            builder.Property(c => c.Discount).HasColumnName("discount");
            builder.Property(c => c.TypeId).HasColumnName("type_id");
            builder.Property(c => c.AssociatedDate).HasColumnName("associated_date");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
