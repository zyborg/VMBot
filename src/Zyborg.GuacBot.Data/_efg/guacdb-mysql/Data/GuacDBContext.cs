using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Zyborg.GuacBot.GuacDB.Data
{
    public partial class GuacDBContext : DbContext
    {
        public GuacDBContext(DbContextOptions<GuacDBContext> options)
            : base(options)
        {
        }

        #region Generated Properties
        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionAttribute> GuacamoleConnectionAttributes { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupAttribute> GuacamoleConnectionGroupAttributes { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupPermission> GuacamoleConnectionGroupPermissions { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup> GuacamoleConnectionGroups { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> GuacamoleConnectionHistories { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionParameter> GuacamoleConnectionParameters { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionPermission> GuacamoleConnectionPermissions { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection> GuacamoleConnections { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleEntity> GuacamoleEntities { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileAttribute> GuacamoleSharingProfileAttributes { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileParameter> GuacamoleSharingProfileParameters { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfilePermission> GuacamoleSharingProfilePermissions { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile> GuacamoleSharingProfiles { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSystemPermission> GuacamoleSystemPermissions { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserAttribute> GuacamoleUserAttributes { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupAttribute> GuacamoleUserGroupAttributes { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupMember> GuacamoleUserGroupMembers { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupPermission> GuacamoleUserGroupPermissions { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroup> GuacamoleUserGroups { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory> GuacamoleUserHistories { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPasswordHistory> GuacamoleUserPasswordHistories { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPermission> GuacamoleUserPermissions { get; set; }

        public virtual DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUser> GuacamoleUsers { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Generated Configuration
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleConnectionAttributeMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleConnectionGroupAttributeMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleConnectionGroupMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleConnectionGroupPermissionMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleConnectionHistoryMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleConnectionMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleConnectionParameterMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleConnectionPermissionMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleEntityMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleSharingProfileAttributeMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleSharingProfileMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleSharingProfileParameterMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleSharingProfilePermissionMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleSystemPermissionMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleUserAttributeMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleUserGroupAttributeMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleUserGroupMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleUserGroupMemberMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleUserGroupPermissionMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleUserHistoryMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleUserMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleUserPasswordHistoryMap());
            modelBuilder.ApplyConfiguration(new Zyborg.GuacBot.GuacDB.Data.Mapping.GuacamoleUserPermissionMap());
            #endregion
        }
    }
}
