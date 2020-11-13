using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleUserGroupAttributeMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupAttribute>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupAttribute> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_user_group_attribute");

            // key
            builder.HasKey(t => new { t.UserGroupId, t.AttributeName });

            // properties
            builder.Property(t => t.UserGroupId)
                .IsRequired()
                .HasColumnName("user_group_id")
                .HasColumnType("int");

            builder.Property(t => t.AttributeName)
                .IsRequired()
                .HasColumnName("attribute_name")
                .HasColumnType("varchar(128)")
                .HasMaxLength(128);

            builder.Property(t => t.AttributeValue)
                .IsRequired()
                .HasColumnName("attribute_value")
                .HasColumnType("varchar(4096)")
                .HasMaxLength(4096);

            // relationships
            builder.HasOne(t => t.GuacamoleUserGroup)
                .WithMany(t => t.GuacamoleUserGroupAttributes)
                .HasForeignKey(d => d.UserGroupId)
                .HasConstraintName("guacamole_user_group_attribute_ibfk_1");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_user_group_attribute";
        }

        public struct Columns
        {
            public const string UserGroupId = "user_group_id";
            public const string AttributeName = "attribute_name";
            public const string AttributeValue = "attribute_value";
        }
        #endregion
    }
}
