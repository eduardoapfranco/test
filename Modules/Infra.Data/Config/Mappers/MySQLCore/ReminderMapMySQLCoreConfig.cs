using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Infra.Data.Config.Mappers.MySQLCore
    {
    [ExcludeFromCodeCoverage]

    public class ReminderMapMySQLCoreConfig : IEntityTypeConfiguration<Reminder>
        {
        public void Configure(EntityTypeBuilder<Reminder> builder)
            {
            builder.ToTable("reminders2");

            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.AppId).HasColumnName("app_id");
            builder.Property(c => c.UserId).HasColumnName("user_id");
            builder.Property(c => c.Title).HasColumnName("title");
            builder.Property(c => c.Description).HasColumnName("description");
            builder.Property(c => c.StartTime).HasColumnName("start_time");
            builder.Property(c => c.EndTime).HasColumnName("end_time");
            builder.Property(c => c.LastUpdated).HasColumnName("last_updated");
            builder.Property(c => c.Deleted).HasColumnName("deleted");

            builder.HasKey(k => k.Id);
            }
        }
    }
