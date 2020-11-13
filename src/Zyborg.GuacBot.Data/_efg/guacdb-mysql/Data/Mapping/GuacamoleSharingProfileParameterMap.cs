using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleSharingProfileParameterMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileParameter>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileParameter> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_sharing_profile_parameter");

            // key
            builder.HasKey(t => new { t.SharingProfileId, t.ParameterName });

            // properties
            builder.Property(t => t.SharingProfileId)
                .IsRequired()
                .HasColumnName("sharing_profile_id")
                .HasColumnType("int");

            builder.Property(t => t.ParameterName)
                .IsRequired()
                .HasColumnName("parameter_name")
                .HasColumnType("varchar(128)")
                .HasMaxLength(128);

            builder.Property(t => t.ParameterValue)
                .IsRequired()
                .HasColumnName("parameter_value")
                .HasColumnType("varchar(4096)")
                .HasMaxLength(4096);

            // relationships
            builder.HasOne(t => t.GuacamoleSharingProfile)
                .WithMany(t => t.GuacamoleSharingProfileParameters)
                .HasForeignKey(d => d.SharingProfileId)
                .HasConstraintName("guacamole_sharing_profile_parameter_ibfk_1");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_sharing_profile_parameter";
        }

        public struct Columns
        {
            public const string SharingProfileId = "sharing_profile_id";
            public const string ParameterName = "parameter_name";
            public const string ParameterValue = "parameter_value";
        }
        #endregion
    }
}
