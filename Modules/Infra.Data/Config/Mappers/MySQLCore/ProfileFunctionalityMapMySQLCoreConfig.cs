using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Config.Mappers.MySQLCore
{
    [ExcludeFromCodeCoverage]
    public class ProfileFunctionalityMapMySQLCoreConfig : IEntityTypeConfiguration<ProfileFunctionality>
    {
        public void Configure(EntityTypeBuilder<ProfileFunctionality> builder)
        {
            builder.ToTable("profiles_functionalities");
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.ProfileId).HasColumnName("profile_id");
            builder.Property(c => c.FunctionalityId).HasColumnName("functionality_id");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.HasKey(k => k.Id);
        }
    }
}
