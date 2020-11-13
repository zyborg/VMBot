using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Mapping
{
    public partial class GuacamoleUserMap
        : IEntityTypeConfiguration<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUser>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUser> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("guacamole_user");

            // key
            builder.HasKey(t => t.UserId);

            // properties
            builder.Property(t => t.UserId)
                .IsRequired()
                .HasColumnName("user_id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.EntityId)
                .IsRequired()
                .HasColumnName("entity_id")
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

            builder.Property(t => t.Disabled)
                .IsRequired()
                .HasColumnName("disabled")
                .HasColumnType("tinyint(1)");

            builder.Property(t => t.Expired)
                .IsRequired()
                .HasColumnName("expired")
                .HasColumnType("tinyint(1)");

            builder.Property(t => t.AccessWindowStart)
                .HasColumnName("access_window_start")
                .HasColumnType("time(6)");

            builder.Property(t => t.AccessWindowEnd)
                .HasColumnName("access_window_end")
                .HasColumnType("time(6)");

            builder.Property(t => t.ValidFrom)
                .HasColumnName("valid_from")
                .HasColumnType("date");

            builder.Property(t => t.ValidUntil)
                .HasColumnName("valid_until")
                .HasColumnType("date");

            builder.Property(t => t.Timezone)
                .HasColumnName("timezone")
                .HasColumnType("varchar(64)")
                .HasMaxLength(64);

            builder.Property(t => t.FullName)
                .HasColumnName("full_name")
                .HasColumnType("varchar(256)")
                .HasMaxLength(256);

            builder.Property(t => t.EmailAddress)
                .HasColumnName("email_address")
                .HasColumnType("varchar(256)")
                .HasMaxLength(256);

            builder.Property(t => t.Organization)
                .HasColumnName("organization")
                .HasColumnType("varchar(256)")
                .HasMaxLength(256);

            builder.Property(t => t.OrganizationalRole)
                .HasColumnName("organizational_role")
                .HasColumnType("varchar(256)")
                .HasMaxLength(256);

            // relationships
            builder.HasOne(t => t.GuacamoleEntity)
                .WithMany(t => t.GuacamoleUsers)
                .HasForeignKey(d => d.EntityId)
                .HasConstraintName("guacamole_user_entity");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "";
            public const string Name = "guacamole_user";
        }

        public struct Columns
        {
            public const string UserId = "user_id";
            public const string EntityId = "entity_id";
            public const string PasswordHash = "password_hash";
            public const string PasswordSalt = "password_salt";
            public const string PasswordDate = "password_date";
            public const string Disabled = "disabled";
            public const string Expired = "expired";
            public const string AccessWindowStart = "access_window_start";
            public const string AccessWindowEnd = "access_window_end";
            public const string ValidFrom = "valid_from";
            public const string ValidUntil = "valid_until";
            public const string Timezone = "timezone";
            public const string FullName = "full_name";
            public const string EmailAddress = "email_address";
            public const string Organization = "organization";
            public const string OrganizationalRole = "organizational_role";
        }
        #endregion
    }
}
