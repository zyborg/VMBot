using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleConnectionGroupMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_connection_group");

            // key
            builder.HasKey(t => t.ConnectionGroupId);

            // properties
            builder.Property(t => t.ConnectionGroupId)
                .IsRequired()
                .HasColumnName("connection_group_id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.ParentId)
                .HasColumnName("parent_id")
                .HasColumnType("int");

            builder.Property(t => t.ConnectionGroupName)
                .IsRequired()
                .HasColumnName("connection_group_name")
                .HasColumnType("varchar(128)")
                .HasMaxLength(128);

            builder.Property(t => t.Type)
                .IsRequired()
                .HasColumnName("type")
                .HasColumnType("enum('ORGANIZATIONAL','BALANCING')")
                .HasDefaultValueSql("'ORGANIZATIONAL'");

            builder.Property(t => t.MaxConnections)
                .HasColumnName("max_connections")
                .HasColumnType("int");

            builder.Property(t => t.MaxConnectionsPerUser)
                .HasColumnName("max_connections_per_user")
                .HasColumnType("int");

            builder.Property(t => t.EnableSessionAffinity)
                .IsRequired()
                .HasColumnName("enable_session_affinity")
                .HasColumnType("tinyint(1)");

            // relationships
            builder.HasOne(t => t.ParentGuacamoleConnectionGroup)
                .WithMany(t => t.ParentGuacamoleConnectionGroups)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("guacamole_connection_group_ibfk_1");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_connection_group";
        }

        public struct Columns
        {
            public const string ConnectionGroupId = "connection_group_id";
            public const string ParentId = "parent_id";
            public const string ConnectionGroupName = "connection_group_name";
            public const string Type = "type";
            public const string MaxConnections = "max_connections";
            public const string MaxConnectionsPerUser = "max_connections_per_user";
            public const string EnableSessionAffinity = "enable_session_affinity";
        }
        #endregion
    }
}
