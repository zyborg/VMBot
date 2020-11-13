using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleConnectionMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_connection");

            // key
            builder.HasKey(t => t.ConnectionId);

            // properties
            builder.Property(t => t.ConnectionId)
                .IsRequired()
                .HasColumnName("connection_id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.ConnectionName)
                .IsRequired()
                .HasColumnName("connection_name")
                .HasColumnType("varchar(128)")
                .HasMaxLength(128);

            builder.Property(t => t.ParentId)
                .HasColumnName("parent_id")
                .HasColumnType("int");

            builder.Property(t => t.Protocol)
                .IsRequired()
                .HasColumnName("protocol")
                .HasColumnType("varchar(32)")
                .HasMaxLength(32);

            builder.Property(t => t.ProxyPort)
                .HasColumnName("proxy_port")
                .HasColumnType("int");

            builder.Property(t => t.ProxyHostname)
                .HasColumnName("proxy_hostname")
                .HasColumnType("varchar(512)")
                .HasMaxLength(512);

            builder.Property(t => t.ProxyEncryptionMethod)
                .HasColumnName("proxy_encryption_method")
                .HasColumnType("enum('NONE','SSL')");

            builder.Property(t => t.MaxConnections)
                .HasColumnName("max_connections")
                .HasColumnType("int");

            builder.Property(t => t.MaxConnectionsPerUser)
                .HasColumnName("max_connections_per_user")
                .HasColumnType("int");

            builder.Property(t => t.ConnectionWeight)
                .HasColumnName("connection_weight")
                .HasColumnType("int");

            builder.Property(t => t.FailoverOnly)
                .IsRequired()
                .HasColumnName("failover_only")
                .HasColumnType("tinyint(1)");

            // relationships
            builder.HasOne(t => t.ParentGuacamoleConnectionGroup)
                .WithMany(t => t.ParentGuacamoleConnections)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("guacamole_connection_ibfk_1");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_connection";
        }

        public struct Columns
        {
            public const string ConnectionId = "connection_id";
            public const string ConnectionName = "connection_name";
            public const string ParentId = "parent_id";
            public const string Protocol = "protocol";
            public const string ProxyPort = "proxy_port";
            public const string ProxyHostname = "proxy_hostname";
            public const string ProxyEncryptionMethod = "proxy_encryption_method";
            public const string MaxConnections = "max_connections";
            public const string MaxConnectionsPerUser = "max_connections_per_user";
            public const string ConnectionWeight = "connection_weight";
            public const string FailoverOnly = "failover_only";
        }
        #endregion
    }
}
