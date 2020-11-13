using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleSharingProfileMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_sharing_profile");

            // key
            builder.HasKey(t => t.SharingProfileId);

            // properties
            builder.Property(t => t.SharingProfileId)
                .IsRequired()
                .HasColumnName("sharing_profile_id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.SharingProfileName)
                .IsRequired()
                .HasColumnName("sharing_profile_name")
                .HasColumnType("varchar(128)")
                .HasMaxLength(128);

            builder.Property(t => t.PrimaryConnectionId)
                .IsRequired()
                .HasColumnName("primary_connection_id")
                .HasColumnType("int");

            // relationships
            builder.HasOne(t => t.PrimaryGuacamoleConnection)
                .WithMany(t => t.PrimaryGuacamoleSharingProfiles)
                .HasForeignKey(d => d.PrimaryConnectionId)
                .HasConstraintName("guacamole_sharing_profile_ibfk_1");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_sharing_profile";
        }

        public struct Columns
        {
            public const string SharingProfileId = "sharing_profile_id";
            public const string SharingProfileName = "sharing_profile_name";
            public const string PrimaryConnectionId = "primary_connection_id";
        }
        #endregion
    }
}
