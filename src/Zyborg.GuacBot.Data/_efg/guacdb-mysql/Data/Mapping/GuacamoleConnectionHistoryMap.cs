using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleConnectionHistoryMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_connection_history");

            // key
            builder.HasKey(t => t.HistoryId);

            // properties
            builder.Property(t => t.HistoryId)
                .IsRequired()
                .HasColumnName("history_id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.UserId)
                .HasColumnName("user_id")
                .HasColumnType("int");

            builder.Property(t => t.Username)
                .IsRequired()
                .HasColumnName("username")
                .HasColumnType("varchar(128)")
                .HasMaxLength(128);

            builder.Property(t => t.RemoteHost)
                .HasColumnName("remote_host")
                .HasColumnType("varchar(256)")
                .HasMaxLength(256);

            builder.Property(t => t.ConnectionId)
                .HasColumnName("connection_id")
                .HasColumnType("int");

            builder.Property(t => t.ConnectionName)
                .IsRequired()
                .HasColumnName("connection_name")
                .HasColumnType("varchar(128)")
                .HasMaxLength(128);

            builder.Property(t => t.SharingProfileId)
                .HasColumnName("sharing_profile_id")
                .HasColumnType("int");

            builder.Property(t => t.SharingProfileName)
                .HasColumnName("sharing_profile_name")
                .HasColumnType("varchar(128)")
                .HasMaxLength(128);

            builder.Property(t => t.StartDate)
                .IsRequired()
                .HasColumnName("start_date")
                .HasColumnType("datetime(6)");

            builder.Property(t => t.EndDate)
                .HasColumnName("end_date")
                .HasColumnType("datetime(6)");

            // relationships
            builder.HasOne(t => t.GuacamoleUser)
                .WithMany(t => t.GuacamoleConnectionHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("guacamole_connection_history_ibfk_1");

            builder.HasOne(t => t.GuacamoleConnection)
                .WithMany(t => t.GuacamoleConnectionHistories)
                .HasForeignKey(d => d.ConnectionId)
                .HasConstraintName("guacamole_connection_history_ibfk_2");

            builder.HasOne(t => t.GuacamoleSharingProfile)
                .WithMany(t => t.GuacamoleConnectionHistories)
                .HasForeignKey(d => d.SharingProfileId)
                .HasConstraintName("guacamole_connection_history_ibfk_3");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_connection_history";
        }

        public struct Columns
        {
            public const string HistoryId = "history_id";
            public const string UserId = "user_id";
            public const string Username = "username";
            public const string RemoteHost = "remote_host";
            public const string ConnectionId = "connection_id";
            public const string ConnectionName = "connection_name";
            public const string SharingProfileId = "sharing_profile_id";
            public const string SharingProfileName = "sharing_profile_name";
            public const string StartDate = "start_date";
            public const string EndDate = "end_date";
        }
        #endregion
    }
}
