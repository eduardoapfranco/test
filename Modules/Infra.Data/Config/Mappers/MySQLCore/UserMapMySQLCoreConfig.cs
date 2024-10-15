using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class UserMapMySQLCoreConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.Name).HasColumnName("name");
            builder.Property(c => c.Email).HasColumnName("email");
            builder.Property(c => c.Cpf).HasColumnName("cpf");
            builder.Property(c => c.Rg).HasColumnName("rg");
            builder.Property(c => c.BirthDate).HasColumnName("data_nascimento");
            builder.Property(c => c.Password).HasColumnName("password");
            builder.Property(c => c.IsAdmin).HasColumnName("is_admin");
            builder.Property(c => c.Status).HasColumnName("status");
            builder.Property(c => c.PhoneNumber1).HasColumnName("telefone1");
            builder.Property(c => c.PhoneNumber2).HasColumnName("telefone2");
            builder.Property(c => c.Address).HasColumnName("endereco");
            builder.Property(c => c.AddressNumber).HasColumnName("numero");
            builder.Property(c => c.Neighborhood).HasColumnName("bairro");
            builder.Property(c => c.AddressComplement).HasColumnName("complemento");
            builder.Property(c => c.ZipCode).HasColumnName("cep");
            builder.Property(c => c.City).HasColumnName("cidade");
            builder.Property(c => c.State).HasColumnName("estado");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.Property(c => c.TokenNotification).HasColumnName("token_notification");
            builder.Property(c => c.IuguCustomerId).HasColumnName("iugu_customer_id");
            builder.Property(c => c.WebSite).HasColumnName("website");
            builder.Property(c => c.ActArea).HasColumnName("area_atuacao");
            builder.Property(c => c.Avatar).HasColumnName("avatar");
            builder.Property(c => c.Company).HasColumnName("empresa");
            builder.Property(c => c.RDConversionID).HasColumnName("rd_conversion_id");
            builder.Property(c => c.LastLoginDate).HasColumnName("last_login_date");
            builder.HasKey(k => k.Id);
        }
    }
}
