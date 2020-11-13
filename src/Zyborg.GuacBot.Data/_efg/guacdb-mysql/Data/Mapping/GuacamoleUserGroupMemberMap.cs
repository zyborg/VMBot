using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleUserGroupMemberMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupMember>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupMember> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_user_group_member");

            // key
            builder.HasKey(t => new { t.UserGroupId, t.MemberEntityId });

            // properties
            builder.Property(t => t.UserGroupId)
                .IsRequired()
                .HasColumnName("user_group_id")
                .HasColumnType("int");

            builder.Property(t => t.MemberEntityId)
                .IsRequired()
                .HasColumnName("member_entity_id")
                .HasColumnType("int");

            // relationships
            builder.HasOne(t => t.MemberGuacamoleEntity)
                .WithMany(t => t.MemberGuacamoleUserGroupMembers)
                .HasForeignKey(d => d.MemberEntityId)
                .HasConstraintName("guacamole_user_group_member_entity_id");

            builder.HasOne(t => t.GuacamoleUserGroup)
                .WithMany(t => t.GuacamoleUserGroupMembers)
                .HasForeignKey(d => d.UserGroupId)
                .HasConstraintName("guacamole_user_group_member_parent_id");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_user_group_member";
        }

        public struct Columns
        {
            public const string UserGroupId = "user_group_id";
            public const string MemberEntityId = "member_entity_id";
        }
        #endregion
    }
}
