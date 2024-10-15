using Infra.CrossCutting.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Infra.Data.Config
{
    [ExcludeFromCodeCoverage]
    public static class ConfigPropertieDefault
    {
        internal static bool IsEntity(Type type)
        {
            var isEntity = type.BaseType == typeof(BaseEntity<>);
            if (isEntity)
                return true;
            if (type.BaseType != null)
                return IsEntity(type.BaseType);
            else
                return false;
        }
        internal static ModelBuilder AddDefaultProperties(this ModelBuilder modelBuilder)
        {
            var listEntities = modelBuilder.Model.GetEntityTypes();

            foreach (var entityType in listEntities)
            {
                if (typeof(IEntityAudited).IsAssignableFrom(entityType.ClrType))
                {
                    if (entityType.FindProperty("Id") != null)
                    {
                        modelBuilder.Entity(entityType.Name).Property<DateTime?>("CreatedAt");
                        modelBuilder.Entity(entityType.Name).Property<DateTime?>("UpdatedAt");
                    }
                }
            }

            return modelBuilder;
        }

        internal static void SaveDefaultPropertiesChanges(ChangeTracker changeTracker)
        {
            foreach (var entry in changeTracker.Entries().Where(e => (e.State == EntityState.Added) && e.Entity is IEntityAudited))
            {
                entry.Property("CreatedAt").CurrentValue = DateTime.Now;
                entry.Property("UpdatedAt").CurrentValue = DateTime.Now;
            }

            foreach (var entry in changeTracker.Entries()
                .Where(e => (e.State == EntityState.Modified) && e.Entity is IEntityAudited))
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.Now;
            }            
        }
    }
}
