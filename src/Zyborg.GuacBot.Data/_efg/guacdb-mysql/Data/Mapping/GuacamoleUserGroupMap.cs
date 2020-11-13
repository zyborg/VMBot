using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleUserGroupMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroup>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroup> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_user_group");

            // key
            builder.HasKey(t => t.UserGroupId);

            // properties
            builder.Property(t => t.UserGroupId)
                .IsRequired()
                .HasColumnName("user_group_id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.EntityId)
                .IsRequired()
                .HasColumnName("entity_id")
                .HasColumnType("int");

            builder.Property(t => t.Disabled)
                .IsRequired()
                .HasColumnName("disabled")
                .HasColumnType("tinyint(1)");

            // relationships
            builder.HasOne(t => t.GuacamoleEntity)
                .WithMany(t => t.GuacamoleUserGroups)
                .HasForeignKey(d => d.EntityId)
                .HasConstraintName("guacamole_user_group_entity");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_user_group";
        }

        public struct Columns
        {
            public const string UserGroupId = "user_group_id";
            public const string EntityId = "entity_id";
            public const string Disabled = "disabled";
        }
        #endregion
    }
}
