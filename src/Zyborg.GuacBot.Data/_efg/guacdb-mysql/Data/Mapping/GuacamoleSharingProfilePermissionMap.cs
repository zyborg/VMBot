using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleSharingProfilePermissionMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfilePermission>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfilePermission> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_sharing_profile_permission");

            // key
            builder.HasKey(t => new { t.EntityId, t.SharingProfileId, t.Permission });

            // properties
            builder.Property(t => t.EntityId)
                .IsRequired()
                .HasColumnName("entity_id")
                .HasColumnType("int");

            builder.Property(t => t.SharingProfileId)
                .IsRequired()
                .HasColumnName("sharing_profile_id")
                .HasColumnType("int");

            builder.Property(t => t.Permission)
                .IsRequired()
                .HasColumnName("permission")
                .HasColumnType("enum('READ','UPDATE','DELETE','ADMINISTER')");

            // relationships
            builder.HasOne(t => t.GuacamoleEntity)
                .WithMany(t => t.GuacamoleSharingProfilePermissions)
                .HasForeignKey(d => d.EntityId)
                .HasConstraintName("guacamole_sharing_profile_permission_entity");

            builder.HasOne(t => t.GuacamoleSharingProfile)
                .WithMany(t => t.GuacamoleSharingProfilePermissions)
                .HasForeignKey(d => d.SharingProfileId)
                .HasConstraintName("guacamole_sharing_profile_permission_ibfk_1");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_sharing_profile_permission";
        }

        public struct Columns
        {
            public const string EntityId = "entity_id";
            public const string SharingProfileId = "sharing_profile_id";
            public const string Permission = "permission";
        }
        #endregion
    }
}
