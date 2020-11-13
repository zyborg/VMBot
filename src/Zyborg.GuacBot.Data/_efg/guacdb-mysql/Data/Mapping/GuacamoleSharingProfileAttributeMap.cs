using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleSharingProfileAttributeMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileAttribute>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileAttribute> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_sharing_profile_attribute");

            // key
            builder.HasKey(t => new { t.SharingProfileId, t.AttributeName });

            // properties
            builder.Property(t => t.SharingProfileId)
                .IsRequired()
                .HasColumnName("sharing_profile_id")
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
            builder.HasOne(t => t.GuacamoleSharingProfile)
                .WithMany(t => t.GuacamoleSharingProfileAttributes)
                .HasForeignKey(d => d.SharingProfileId)
                .HasConstraintName("guacamole_sharing_profile_attribute_ibfk_1");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_sharing_profile_attribute";
        }

        public struct Columns
        {
            public const string SharingProfileId = "sharing_profile_id";
            public const string AttributeName = "attribute_name";
            public const string AttributeValue = "attribute_value";
        }
        #endregion
    }
}
