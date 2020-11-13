using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleUserPermissionMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPermission>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPermission> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_user_permission");

            // key
            builder.HasKey(t => new { t.EntityId, t.AffectedUserId, t.Permission });

            // properties
            builder.Property(t => t.EntityId)
                .IsRequired()
                .HasColumnName("entity_id")
                .HasColumnType("int");

            builder.Property(t => t.AffectedUserId)
                .IsRequired()
                .HasColumnName("affected_user_id")
                .HasColumnType("int");

            builder.Property(t => t.Permission)
                .IsRequired()
                .HasColumnName("permission")
                .HasColumnType("enum('READ','UPDATE','DELETE','ADMINISTER')");

            // relationships
            builder.HasOne(t => t.GuacamoleEntity)
                .WithMany(t => t.GuacamoleUserPermissions)
                .HasForeignKey(d => d.EntityId)
                .HasConstraintName("guacamole_user_permission_entity");

            builder.HasOne(t => t.AffectedGuacamoleUser)
                .WithMany(t => t.AffectedGuacamoleUserPermissions)
                .HasForeignKey(d => d.AffectedUserId)
                .HasConstraintName("guacamole_user_permission_ibfk_1");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_user_permission";
        }

        public struct Columns
        {
            public const string EntityId = "entity_id";
            public const string AffectedUserId = "affected_user_id";
            public const string Permission = "permission";
        }
        #endregion
    }
}
