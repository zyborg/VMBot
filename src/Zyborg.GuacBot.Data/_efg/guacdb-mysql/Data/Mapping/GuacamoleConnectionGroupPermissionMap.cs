using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleConnectionGroupPermissionMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupPermission>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupPermission> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_connection_group_permission");

            // key
            builder.HasKey(t => new { t.EntityId, t.ConnectionGroupId, t.Permission });

            // properties
            builder.Property(t => t.EntityId)
                .IsRequired()
                .HasColumnName("entity_id")
                .HasColumnType("int");

            builder.Property(t => t.ConnectionGroupId)
                .IsRequired()
                .HasColumnName("connection_group_id")
                .HasColumnType("int");

            builder.Property(t => t.Permission)
                .IsRequired()
                .HasColumnName("permission")
                .HasColumnType("enum('READ','UPDATE','DELETE','ADMINISTER')");

            // relationships
            builder.HasOne(t => t.GuacamoleEntity)
                .WithMany(t => t.GuacamoleConnectionGroupPermissions)
                .HasForeignKey(d => d.EntityId)
                .HasConstraintName("guacamole_connection_group_permission_entity");

            builder.HasOne(t => t.GuacamoleConnectionGroup)
                .WithMany(t => t.GuacamoleConnectionGroupPermissions)
                .HasForeignKey(d => d.ConnectionGroupId)
                .HasConstraintName("guacamole_connection_group_permission_ibfk_1");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_connection_group_permission";
        }

        public struct Columns
        {
            public const string EntityId = "entity_id";
            public const string ConnectionGroupId = "connection_group_id";
            public const string Permission = "permission";
        }
        #endregion
    }
}
