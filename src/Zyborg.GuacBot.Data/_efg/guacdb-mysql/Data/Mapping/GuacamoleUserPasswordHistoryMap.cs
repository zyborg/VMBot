using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleUserPasswordHistoryMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPasswordHistory>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPasswordHistory> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_user_password_history");

            // key
            builder.HasKey(t => t.PasswordHistoryId);

            // properties
            builder.Property(t => t.PasswordHistoryId)
                .IsRequired()
                .HasColumnName("password_history_id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.UserId)
                .IsRequired()
                .HasColumnName("user_id")
                .HasColumnType("int");

            builder.Property(t => t.PasswordHash)
                .IsRequired()
                .HasColumnName("password_hash")
                .HasColumnType("binary(32)")
                .HasMaxLength(32);

            builder.Property(t => t.PasswordSalt)
                .HasColumnName("password_salt")
                .HasColumnType("binary(32)")
                .HasMaxLength(32);

            builder.Property(t => t.PasswordDate)
                .IsRequired()
                .HasColumnName("password_date")
                .HasColumnType("datetime(6)");

            // relationships
            builder.HasOne(t => t.GuacamoleUser)
                .WithMany(t => t.GuacamoleUserPasswordHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("guacamole_user_password_history_ibfk_1");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_user_password_history";
        }

        public struct Columns
        {
            public const string PasswordHistoryId = "password_history_id";
            public const string UserId = "user_id";
            public const string PasswordHash = "password_hash";
            public const string PasswordSalt = "password_salt";
            public const string PasswordDate = "password_date";
        }
        #endregion
    }
}
