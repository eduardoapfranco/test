using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class ConstructionMapMySQLCoreConfig : IEntityTypeConfiguration<Construction>
    {
        public void Configure(EntityTypeBuilder<Construction> builder)
        {
            builder.ToTable("constructions");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.Nome).HasColumnName("nome");
            builder.Property(c => c.Responsavel).HasColumnName("responsavel");
            builder.Property(c => c.Contratante).HasColumnName("contratante");
            builder.Property(c => c.Endereco).HasColumnName("endereco");
            builder.Property(c => c.Inicio).HasColumnName("inicio");
            builder.Property(c => c.Termino).HasColumnName("termino");
            builder.Property(c => c.Metragem).HasColumnName("metragem");
            builder.Property(c => c.ValorEstimado).HasColumnName("valor_estimado");
            builder.Property(c => c.Status).HasColumnName("status");
            builder.Property(c => c.UserId).HasColumnName("user_id");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.Property(c => c.Deleted).HasColumnName("deleted");
            builder.Property(c => c.AppId).HasColumnName("app_id");
            builder.Property(c => c.Image).HasColumnName("image");
            builder.HasKey(k => k.Id);
        }
    }
}
