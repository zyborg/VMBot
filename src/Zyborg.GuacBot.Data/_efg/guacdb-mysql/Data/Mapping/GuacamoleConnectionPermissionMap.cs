using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleConnectionPermissionMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionPermission>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionPermission> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_connection_permission");

            // key
            builder.HasKey(t => new { t.EntityId, t.ConnectionId, t.Permission });

            // properties
            builder.Property(t => t.EntityId)
                .IsRequired()
                .HasColumnName("entity_id")
                .HasColumnType("int");

            builder.Property(t => t.ConnectionId)
                .IsRequired()
                .HasColumnName("connection_id")
                .HasColumnType("int");

            builder.Property(t => t.Permission)
                .IsRequired()
                .HasColumnName("permission")
                .HasColumnType("enum('READ','UPDATE','DELETE','ADMINISTER')");

            // relationships
            builder.HasOne(t => t.GuacamoleEntity)
                .WithMany(t => t.GuacamoleConnectionPermissions)
                .HasForeignKey(d => d.EntityId)
                .HasConstraintName("guacamole_connection_permission_entity");

            builder.HasOne(t => t.GuacamoleConnection)
                .WithMany(t => t.GuacamoleConnectionPermissions)
                .HasForeignKey(d => d.ConnectionId)
                .HasConstraintName("guacamole_connection_permission_ibfk_1");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_connection_permission";
        }

        public struct Columns
        {
            public const string EntityId = "entity_id";
            public const string ConnectionId = "connection_id";
            public const string Permission = "permission";
        }
        #endregion
    }
}
