using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleConnectionAttributeMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionAttribute>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionAttribute> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_connection_attribute");

            // key
            builder.HasKey(t => new { t.ConnectionId, t.AttributeName });

            // properties
            builder.Property(t => t.ConnectionId)
                .IsRequired()
                .HasColumnName("connection_id")
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
            builder.HasOne(t => t.GuacamoleConnection)
                .WithMany(t => t.GuacamoleConnectionAttributes)
                .HasForeignKey(d => d.ConnectionId)
                .HasConstraintName("guacamole_connection_attribute_ibfk_1");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_connection_attribute";
        }

        public struct Columns
        {
            public const string ConnectionId = "connection_id";
            public const string AttributeName = "attribute_name";
            public const string AttributeValue = "attribute_value";
        }
        #endregion
    }
}
