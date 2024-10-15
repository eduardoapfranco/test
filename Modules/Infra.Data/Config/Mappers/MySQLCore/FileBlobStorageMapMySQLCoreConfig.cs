using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class FileBlobStorageMapMySQLCoreConfig : IEntityTypeConfiguration<FileBlobStorage>
    {
        public void Configure(EntityTypeBuilder<FileBlobStorage> builder)
        {
            builder.ToTable("files_blob_storage");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.Title).HasColumnName("title");
            builder.Property(c => c.BlobId).HasColumnName("blob_id");
            builder.Property(c => c.Zip).HasColumnName("zip");
            builder.Property(c => c.Size).HasColumnName("size");
            builder.Property(c => c.Type).HasColumnName("type");
            builder.Property(c => c.Origin).HasColumnName("origin");            
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
