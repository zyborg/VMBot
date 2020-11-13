using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleConnectionParameterMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionParameter>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionParameter> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_connection_parameter");

            // key
            builder.HasKey(t => new { t.ConnectionId, t.ParameterName });

            // properties
            builder.Property(t => t.ConnectionId)
                .IsRequired()
                .HasColumnName("connection_id")
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
            builder.HasOne(t => t.GuacamoleConnection)
                .WithMany(t => t.GuacamoleConnectionParameters)
                .HasForeignKey(d => d.ConnectionId)
                .HasConstraintName("guacamole_connection_parameter_ibfk_1");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_connection_parameter";
        }

        public struct Columns
        {
            public const string ConnectionId = "connection_id";
            public const string ParameterName = "parameter_name";
            public const string ParameterValue = "parameter_value";
        }
        #endregion
    }
}
