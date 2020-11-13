using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleEntity
    {
        public GuacamoleEntity()
        {
            #region Generated Constructor
            GuacamoleConnectionGroupPermissions = new HashSet<GuacamoleConnectionGroupPermission>();
            GuacamoleConnectionPermissions = new HashSet<GuacamoleConnectionPermission>();
            GuacamoleSharingProfilePermissions = new HashSet<GuacamoleSharingProfilePermission>();
            GuacamoleSystemPermissions = new HashSet<GuacamoleSystemPermission>();
            GuacamoleUserGroupPermissions = new HashSet<GuacamoleUserGroupPermission>();
            GuacamoleUserGroups = new HashSet<GuacamoleUserGroup>();
            GuacamoleUserPermissions = new HashSet<GuacamoleUserPermission>();
            GuacamoleUsers = new HashSet<GuacamoleUser>();
            MemberGuacamoleUserGroupMembers = new HashSet<GuacamoleUserGroupMember>();
            #endregion
        }

        #region Generated Properties
        public int EntityId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        #endregion

        #region Generated Relationships
        public virtual ICollection<GuacamoleConnectionGroupPermission> GuacamoleConnectionGroupPermissions { get; set; }

        public virtual ICollection<GuacamoleConnectionPermission> GuacamoleConnectionPermissions { get; set; }

        public virtual ICollection<GuacamoleSharingProfilePermission> GuacamoleSharingProfilePermissions { get; set; }

        public virtual ICollection<GuacamoleSystemPermission> GuacamoleSystemPermissions { get; set; }

        public virtual ICollection<GuacamoleUserGroupPermission> GuacamoleUserGroupPermissions { get; set; }

        public virtual ICollection<GuacamoleUserGroup> GuacamoleUserGroups { get; set; }

        public virtual ICollection<GuacamoleUserPermission> GuacamoleUserPermissions { get; set; }

        public virtual ICollection<GuacamoleUser> GuacamoleUsers { get; set; }

        public virtual ICollection<GuacamoleUserGroupMember> MemberGuacamoleUserGroupMembers { get; set; }

        #endregion

    }
}
