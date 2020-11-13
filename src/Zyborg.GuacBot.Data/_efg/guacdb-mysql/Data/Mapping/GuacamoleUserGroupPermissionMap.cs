using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleUserGroupPermissionMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupPermission>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupPermission> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_user_group_permission");

            // key
            builder.HasKey(t => new { t.EntityId, t.AffectedUserGroupId, t.Permission });

            // properties
            builder.Property(t => t.EntityId)
                .IsRequired()
                .HasColumnName("entity_id")
                .HasColumnType("int");

            builder.Property(t => t.AffectedUserGroupId)
                .IsRequired()
                .HasColumnName("affected_user_group_id")
                .HasColumnType("int");

            builder.Property(t => t.Permission)
                .IsRequired()
                .HasColumnName("permission")
                .HasColumnType("enum('READ','UPDATE','DELETE','ADMINISTER')");

            // relationships
            builder.HasOne(t => t.AffectedGuacamoleUserGroup)
                .WithMany(t => t.AffectedGuacamoleUserGroupPermissions)
                .HasForeignKey(d => d.AffectedUserGroupId)
                .HasConstraintName("guacamole_user_group_permission_affected_user_group");

            builder.HasOne(t => t.GuacamoleEntity)
                .WithMany(t => t.GuacamoleUserGroupPermissions)
                .HasForeignKey(d => d.EntityId)
                .HasConstraintName("guacamole_user_group_permission_entity");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_user_group_permission";
        }

        public struct Columns
        {
            public const string EntityId = "entity_id";
            public const string AffectedUserGroupId = "affected_user_group_id";
            public const string Permission = "permission";
        }
        #endregion
    }
}
